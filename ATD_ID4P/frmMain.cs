using ATD_ID4P.Class;
//using ATD_ID4P.Model;
using ATD_ID4P.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using ATD_ID4P.Model;

namespace ATD_ID4P
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
        }

        private bool Dark_DisplayMode = Properties.Settings.Default.UI_DarkMode;

        private readonly string logpath = Application.StartupPath + "\\Log\\HistoryLog.txt";
        private frmOrder fOrder = null;
        private frmProduct fProduct = null;
        private frmSetting fSetting = null;
        private frmDailyReport fDailyReport = null;
        private frmOrderReport fOrderReport = null;
        private LogCls Log = new LogCls();

        private void FrmMain_Load(object sender, EventArgs e)
        {

            CheckkApplicationIsAlreadyRunning();
            // - Check if another instance of the program is already running
            SetDisplayMode();
            // Set the Colour following object according to DarkMode Setting Variable:
            // - frmMain.BackColor
            // - pnlMenuBar.BackColor
            // - pnlContent.BackColor
            // - all Controls of pnlMenuBar
            UICls.ShowWaiting(true, "Please wait.", "System starting...");
            // - Display loading box (use frmWaiting)
            InitUI();
            // Set the Colour of following object according to DarkMode Setting Variable:
            // - all Controls of pnlMenuBar when mouseHover
            // - all ToolStripMenuItem of cmsReport when mouseHover
            // - btnCloseMenu when mouseHover, mouseLeave
            // - btnMinimizeMenu.mouseHover, mouseLeave
            ShowOrderForm();
            // - Create frmOrder
            // - Get Order info from "tblOrder"
            // - put in pnlContent of frmMain
            Log.WriteLog("Complete Sequence! ID4P has started on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), LogType.Notes);
            UICls.ShowWaiting(false);
            // - Close displayed loading box
        }

        #region Main Initialize Methods
        private void CheckkApplicationIsAlreadyRunning()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                string buf_Msg = "The system detect that another instance of ID4P is already running.\n"+
                                    "Please close that instance and try open ID4P again.";
                string buf_Topic = "Error! - Another program instance is running.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }
        }
        private void SetDisplayMode()
        {
            if (Dark_DisplayMode == false)
            {
                this.BackColor = Color.White;

                foreach (Control ctrl in pnlMenuBar.Controls)
                {
                    if (ctrl.GetType().ToString() == "System.Windows.Forms.Button")
                    {
                        Button btn = (Button)ctrl;
                        btn.BackColor = Color.SteelBlue;
                        //btn.ForeColor = Color.Black;
                    }
                }

                pnlMenuBar.BackColor = Color.SteelBlue;
                pnlContent.BackColor = Color.White;

            }

        }
        private void InitUI()
        {
            String ColorCode1 = (Dark_DisplayMode == false ? "#4682b4" : "#000000");
            String ColorCode2 = (Dark_DisplayMode == false ? "#669ce8" : "#787878");
            //Load Default Event
            foreach (Control ctrl in pnlMenuBar.Controls)
            {
                if (ctrl.GetType().ToString() == "System.Windows.Forms.Button")
                {
                    Button btn = (Button)ctrl;
                    btn.MouseHover += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray, ColorTranslator.FromHtml(ColorCode2)));
                    btn.MouseLeave += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray, ColorTranslator.FromHtml(ColorCode1)));
                }
            }

            foreach (ToolStripMenuItem tsm in cmsReport.Items)
            {
                tsm.MouseEnter += new EventHandler((s, ea) => UICls.Tsm_MouseHover(s, ea, Color.LightGray, ColorTranslator.FromHtml(ColorCode2)));
                tsm.MouseLeave += new EventHandler((s, ea) => UICls.Tsm_MouseHover(s, ea, Color.LightGray, ColorTranslator.FromHtml(ColorCode1)));
            }

            btnCloseMenu.MouseHover += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));
            btnCloseMenu.MouseLeave += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));

            btnMinimizeMenu.MouseHover += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));
            btnMinimizeMenu.MouseLeave += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));

        }
        #endregion

        private void btnOrderMenu_Click(object sender, EventArgs e)
        {
            ShowOrderForm();
        }
        public void ShowOrderForm()
        {
            // 1. Check if instance of frmOrder is already create
            // 2.a the instance is not existed
            //      - Create new instance of frmOrder
            //        * Top-Level = false: this form will be contained within a form or another container control
            //      - Make form visible to user
            //      - Create DataTable
            //      - Add Order info from "tblOrder" to the DataTable 
            //      - put frmOrder instance in PnlContent of frmMain
            //      - Maximize frmOrder size
            // 2.b the instance is already existed
            //      - find instance of frmOrder in pnlContent of frmMain
            //      2.b.a frmOrder is found
            //          - Make the found frmOrder display on top of all child form in pnlConent
            //      2.b.b frmOrder is not found
            if (fOrder == null)
            {
                fOrder = new frmOrder()
                {
                    TopLevel = false,
                    Name = "frmOrder"
                };
                fOrder.Show();
                fOrder.OrderGrid_ImportOrderData();
                pnlContent.Controls.Add(fOrder);
                fOrder.WindowState = FormWindowState.Maximized;
            }
            else
            {
                frmOrder _Frm = (frmOrder)UICls.FindFormInPanel(pnlContent, "frmOrder");
                if (_Frm != null)
                {
                    pnlContent.Controls.SetChildIndex(_Frm, 0);
                }
                else
                {
                    fOrder = null;
                    ShowOrderForm();
                }
            }
        }

        private void btnProductMenu_Click(object sender, EventArgs e)
        {
            ShowProductForm();
        }
        public void ShowProductForm(string pMaterialNo = "")
        {
            if (fProduct == null)
            {
                fProduct = new frmProduct()
                {
                    TopLevel = false,
                    Name = "frmProduct"
                };
                fProduct.pMaterialNo = pMaterialNo;
                fProduct.Show(); // 
                // return from frmProduct_Load()

                //  'InvalidOrder' is set to true when
                //  'frmProduct.LoadData()' couldn't find any matching MaterialNO in (frmProduct - Loaddata())
                //  - LocalDB:  [tblMaster]
                //  - PM2, PM3: MO_Spec
                if (fProduct.InvalidOrder == false)
                {
                    pnlContent.Controls.Add(fProduct);
                    fProduct.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    var DResult = MessageBox.Show("Couldn't find matching Material Number\r\nDo you want to manually key the Order Data?", "Error! - Material Number not found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (DResult == DialogResult.Yes)
                    {
                        pnlContent.Controls.Add(fProduct);
                        fProduct.WindowState = FormWindowState.Maximized;
                    }
                }
            }
            else
            {
                frmProduct _Frm = (frmProduct)UICls.FindFormInPanel(pnlContent, "frmProduct");
                // normally if frmOrder.Newdata click and come here it will result in null value
                if (_Frm != null)
                {
                    _Frm.pMaterialNo = pMaterialNo;
                    pnlContent.Controls.SetChildIndex(_Frm, 0);
                    _Frm.CrossFromLoaddata();
                }
                else
                {
                    fProduct = null;
                    ShowProductForm(pMaterialNo);
                }
            }
        }

        private void btnTieSheetMenu_Click(object sender, EventArgs e)
        {
            using (frmTiesheet f = new frmTiesheet())
            {
                f.TopMost = true;
                f.ShowDialog();
            }
        }

        private void btnMenuReport_Click(object sender, EventArgs e)
        {
            // ( the control to display the context menu strip relative to, the location to display the context menu strip at)
            cmsReport.Show(btnMenuReport, new Point(0, btnMenuReport.Height));
        }
        private void OrderReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOrderReportForm();
        }
        private void ShowOrderReportForm()
        {
            if (fOrderReport == null)
            {
                fOrderReport = new frmOrderReport()
                {
                    TopLevel = false,
                    Name = "frmOrderReport"
                };
                fOrderReport.Show();
                pnlContent.Controls.Add(fOrderReport);
                fOrderReport.WindowState = FormWindowState.Maximized;
            }
            else
            {
                frmOrderReport _Frm = (frmOrderReport)UICls.FindFormInPanel(pnlContent, "frmOrderReport");
                if (_Frm != null)
                {
                    pnlContent.Controls.SetChildIndex(_Frm, 0);
                }
                else
                {
                    fOrderReport = null;
                    ShowOrderReportForm();
                }
            }
        }
        private void DailyReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowDailyReportForm();
        }
        private void ShowDailyReportForm()
        {
            if (fDailyReport == null)
            {
                fDailyReport = new frmDailyReport()
                {
                    TopLevel = false,
                    Name = "frmDailyReport"
                };
                fDailyReport.Show();
                pnlContent.Controls.Add(fDailyReport);
                fDailyReport.WindowState = FormWindowState.Maximized;
            }
            else
            {
                frmDailyReport _Frm = (frmDailyReport)UICls.FindFormInPanel(pnlContent, "frmDailyReport");
                if (_Frm != null)
                {
                    pnlContent.Controls.SetChildIndex(_Frm, 0);
                }
                else
                {
                    fDailyReport = null;
                    ShowDailyReportForm();
                }
            }
        }

        private void btnAdminMenu_Click(object sender, EventArgs e)
        {
            ShowSettingForm();
        }
        private void ShowSettingForm()
        {
            string buf_Topic;
            string buf_Msg;
            try
            {
                // Check Admin
                // Input Password, P/W is store settings.settings "AdminCode"
                using (frmSettingLogin fSL = new frmSettingLogin())
                {
                    // Determine if the OK button was clicked on the dialog box
                    if (fSL.ShowDialog() != DialogResult.OK) // Show Dialog box
                        return;

                    string _AdminCode = fSL.AdminCode;

                    if (!_AdminCode.Equals(Properties.Settings.Default.Admin_Password))
                    {
                        MessageBox.Show("AdminCode is invalid.", "Administrator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (fSetting == null)
                {
                    fSetting = new frmSetting()
                    {
                        // TopLevel:
                        // - True: Top-Level Control: not contain in another control appear in it own window.
                        // - False: contained within a form or another container control
                        TopLevel = false,
                        Name = "frmSetting"
                    };
                    pnlContent.Controls.Add(fSetting); // If don't have this fSetting will don't have a parent control and error
                    fSetting.Show(); // Show This Form
                    fSetting.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    frmSetting _Frm = (frmSetting)UICls.FindFormInPanel(pnlContent, "frmSetting");
                    if (_Frm != null)
                    {
                        pnlContent.Controls.SetChildIndex(_Frm, 0); // index 0 mean this form will move to the top most in the parent control
                    }
                    else
                    {
                        // Something happen fSetting form have already been createก but it isn't in a panel
                        // Delete this form and create another form
                        fSetting = null;
                        ShowSettingForm();

                    }
                }
            }
            catch(Exception ex) {
                buf_Topic = "Error! - Click Setting Button.";
                buf_Msg = "Abnormal Error occured after clicking Setting Button.\n"+
                            "Please capture this screen and send to the developer.\n\n"+
                            "Error Detail:\n"+ex;
                MessageBox.Show(buf_Msg,buf_Topic,MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void btnAboutMenu_Click(object sender, EventArgs e)
        {

            // Checked
            // using statement ensure that classes that implement the IDisposable interface call their dispose method
            // it guarantees that the dispose method will be callded, even if the code throws an exception.
            using (frmAboutID4P f = new frmAboutID4P())
            {
                f.ShowDialog();
            }
        }

        private void btnMinimizeMenu_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCloseMenu_Click(object sender, EventArgs e)
        {
            Log.WriteLog("Complete Sequence! ID4P has closed on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), LogType.Notes);
            //InsertLogToDatabase();
            //SaveLogToDB(); it make the Program Freeze when close
            Application.Exit();
        }
        public void SaveLogToDB()
        {
            SqlCls Sql = new SqlCls();
            string Query = "SELECT * FROM [tblHistoryLog] WHERE LogID = -1";

            SqlTransaction STrans = null;
            SqlConnection SaveLogConn = null;
            try
            {
                using (SaveLogConn = new SqlConnection(Sql.connString))
                {
                    DataTable dtLog = new DataTable();
                    SqlDataAdapter daLog = new SqlDataAdapter();
                    SqlCommand cmdTemp = new SqlCommand();

                    SaveLogConn.Open();
                    STrans = SaveLogConn.BeginTransaction(IsolationLevel.Serializable, "SaveLogData");

                    dtLog = Sql.GetDataTableFromSqlwithCUD(ref daLog, Query, SaveLogConn, STrans);

                    //Read Log
                    string logText = File.ReadAllText(logpath);
                    foreach (string rText in logText.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(rText))
                        {
                            DataRow rLog = dtLog.Rows.Add();
                            int i = 0;
                            int splitIndex = 1;
                            foreach (string cell in rText.Split(';'))
                            {
                                if (splitIndex > 4)
                                {
                                    rLog[4] += cell;
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("th-TH");
                                        DateTime LogDate = Convert.ToDateTime(cell, _cultureInfo);
                                        rLog[1] = LogDate;
                                    }
                                    else
                                        rLog[i + 1] = cell;
                                }
                                splitIndex++;
                                i++;
                            }
                        }
                    }
                    daLog.Update(dtLog);

                    STrans.Commit();

                    SaveLogConn.Close();
                    ClearLogFile();
                }
            }
            catch (Exception e)
            {
                if (STrans != null && STrans.Connection != null)
                {
                    STrans.Rollback("SaveLogData");
                }

                if (SaveLogConn.State != ConnectionState.Closed)
                    SaveLogConn.Close();

                Log.WriteLog("Error! - While trying to save log to database: " + e.Message, LogType.Fail);
            }
        }
        public void ClearLogFile()
        {
            string logFilePathHistoty = Application.StartupPath + "\\Log\\HistoryLog" + DateTime.Now.ToString("yyyy-MM-dd HH mm ss tt") + ".txt";
            DateTime Checkdate_Now = DateTime.Now;
            try
            {
                if (File.Exists(logpath))
                    File.Copy(logpath, logFilePathHistoty);

                string[] HistoryFileName = Directory.GetFiles(Application.StartupPath + "\\Log\\", "*.txt");
                for (int i = 0; i < HistoryFileName.Length; i++)
                {
                    DateTime modification = File.GetLastWriteTime(HistoryFileName[i]);
                    TimeSpan ts = Checkdate_Now - modification;
                    int differenceInDays = ts.Days;

                    if (differenceInDays > 7)
                    {
                        File.Delete(HistoryFileName[i]);
                    }

                }

                File.Create(logpath);
            }
            catch (Exception)
            {

            }
        }

        public void Disable_MainMenu(bool disOrder = false, bool disProduct = false, bool disReport = false, bool disAdmin = false)
        { 
            btnOrderMenu.Enabled = !disOrder;
            btnProductMenu.Enabled = !disProduct;
            btnMenuReport.Enabled = !disReport;
            btnAdminMenu.Enabled = !disAdmin;
        }

    }
}