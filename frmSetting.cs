using ATD_ID4P.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.LinkLabel;

namespace ATD_ID4P
{
    public partial class frmSetting : Form
    {
        private LogCls _Log = new LogCls();
        private int RobotModel = Properties.Settings.Default.RobotModel;

        public frmSetting()
        {
            InitializeComponent();
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {
            var buf_fMain = UICls.FindOpenForm("frmMain");
            if (buf_fMain != null)
            {
                frmMain fMain = (frmMain)buf_fMain;
                fMain.Disable_MainMenu(true, true, true, true);
            }
            InitUI();
            LoadAppConfig();
            LoadPluginConfig();
            ShowRobotConnection();
            ShowPLCConnection();
            GetLogData();
        }

        private void InitUI()
        {
            Button[] BTs = new Button[] { btnRefreshLog, btnOpenLogFile, btnCheckConnRobot, btnCheckConnPLC,
                                            btnSaveConfig, btnResetConfig, btnApplyPlugin, btnResetPlugin, btnLineTest };

            foreach (Button BT in BTs)
            {
                BT.MouseHover += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.Yellow));
                BT.MouseLeave += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));
            }
        }

        private void LoadAppConfig()
        {
            // Assign Configuration Value from Settings.Settings to object in this form.
            txbSystemName.Text = Properties.Settings.Default.SystemName;

            if (RobotModel == 1)
            {
                txbRobotIP.Text = Properties.Settings.Default.IP_Robot;
                txbRobotIP.Enabled = true;
            }
            else
            {
                txbRobotIP.Text = "Connected through PLC";
                txbRobotIP.Enabled = false;
            }
            txbPLCIP.Text = Properties.Settings.Default.IP_PLC;
            
            if (Properties.Settings.Default.PM3_ERP == true)
            {
                rdoPM2.Checked = false;
                rdoPM3.Checked = true;
            }
            else
            {
                rdoPM2.Checked = true;
                rdoPM3.Checked = false;
            }
            
            numBDGapWFinger.Value = Properties.Settings.Default.BDGap_wFinger;
            numBDGapWOFinger.Value = Properties.Settings.Default.BDGap_woFinger;

            numMinBDLFinger.Value = Properties.Settings.Default.Finger_Min_BDLength;
            numMaxBDLFinger.Value = Properties.Settings.Default.Finger_Max_BDLength;
            numMinBDWFinger.Value = Properties.Settings.Default.Finger_Min_BDWidth;
            numMaxBDWFinger.Value = Properties.Settings.Default.Finger_Max_BDWidth;

            nmuStackMaxBundleHeight.Value = Properties.Settings.Default.Grip2BD_Max_BDHeight;
            
            numSQPlateH.Value = Properties.Settings.Default.SQFrame_Height;

            rdoDarkMode.Checked = Properties.Settings.Default.UI_DarkMode;
            rdoLightMode.Checked = !rdoDarkMode.Checked;

            numPercentOnPallet.Value = Properties.Settings.Default.Max_idvBD_OVH_Percent;
        }
        private void LoadPluginConfig()
        {
            //Line
            txbLineToken.Text = Properties.Settings.Default.PlugIn_LineNoti_Token;
            rdoLineON.Checked = Properties.Settings.Default.PlugIn_LineNoti;
            rdoLineOFF.Checked = (Properties.Settings.Default.PlugIn_LineNoti != true) ? true : false;

            //Grafana
            txbGrafanaAPIUrl.Text = Properties.Settings.Default.PlugIn_Grafana_Url;
            rdoGrafanaON.Checked = Properties.Settings.Default.PlugIn_Grafana;
            rdoGrafanaOFF.Checked = (Properties.Settings.Default.PlugIn_Grafana != true) ? true : false;

        }

        private void btnCheckConnRobot_Click(object sender, EventArgs e)
        {
            bool Online = ShowRobotConnection();
            if (Online == false)
            {
                string buf_Msg = "Cannot connect to the Robot.\n"+
                                    (RobotModel == 1?
                                    "Please check LAN communication between the computer and Palletizer's CC1 Cabinet(Robot)":
                                    "Please check LAN communication between the computer and Palletizer's CC2 Cabinet(PLC)");
                string buf_Topic = "Error! - Can not connect to the Robot";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private bool ShowRobotConnection()
        {
            bool IsRobotActive;
            if (RobotModel == 1)
            {
                ABBCommu ABBCom = new ABBCommu();
                IsRobotActive = ABBCom.CheckRobotIsActive(txbRobotIP.Text);
            }
            else
            { 
                PLCCommu PLCCom = new PLCCommu(); // Use PLC Connection Instead because Kawa Model don't need to connect to Robot
                if (PLCCom.CheckPLCIsActive(txbPLCIP.Text))
                    IsRobotActive = PLCCom.CheckRobotIsActive(); // this method only check for the RobotInAuto Status
                else
                    IsRobotActive = false;
            }
            lblRobotActive.Text = (IsRobotActive == false ? "Offline" : "Online");
            lblRobotActive.ForeColor = (IsRobotActive == false ? Color.Red : Color.LimeGreen);

            return IsRobotActive;
        }

        private void btnCheckConnPLC_Click(object sender, EventArgs e)
        {
            bool Online = ShowPLCConnection();
            if (Online == false)
            {
                string buf_Msg = "Cannot connect to the PLC.\n" +
                                    "Please check LAN communication between the computer and Palletizer's CC2 Cabinet(PLC)";
                string buf_Topic = "Error! - Can not connect to the Robot";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private bool ShowPLCConnection()
        {
            bool IsPLCActive;
            PLCCommu PLCCom = new PLCCommu();

            IsPLCActive = PLCCom.CheckPLCIsActive(txbPLCIP.Text);
            lblPLCActive.Text = (IsPLCActive == false ? "Offline" : "Online");
            lblPLCActive.ForeColor = (IsPLCActive == false ? Color.Red : Color.LimeGreen);

            return IsPLCActive;
        }

        private void btnRefreshLog_Click(object sender, EventArgs e)
        {
            GetLogData();
        }
        private void GetLogData()
        {
            try
            {
                txbLogs.Text = _Log.GetAllLog();
                txbLogs.SelectionStart = txbLogs.Text.Length;
                txbLogs.ScrollToCaret();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK);
            }
        }

        private void btnOpenLogFile_Click(object sender, EventArgs e)
        {
            ShowLogFile();
        }
        private void ShowLogFile()
        {
            try
            {
                string LogPath = _Log.GetLogFilePath();
                if (File.Exists(LogPath))
                    System.Diagnostics.Process.Start(LogPath);
                else
                {
                    string buf_Msg = "Cannot find Logs file at:\n"+
                                        LogPath;
                    string buf_Topic = "Error! - Can not find Log file.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                _Log.WriteLog(e.Message, LogType.Fail);
            }
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            // \n mean new line 
            // but should use Environment.NewLine instead because it return the newline string of the current OS
            // which mean your code will work on every platform
            // string message1 = "line1\nline2"
            // string message2 = "line1"+Environment.NewLine+"line2"
            // @"" allow us to include any charactor in string
            string buf_Msg = "*** ATTENTION ***\n"+
                                "this will only save Configuration setting not Plugins option.\n"+
                                "if you have changed the PlugIns settings and haven't save it yet.\n"+
                                "Please click NO and click APPLY button under Plugin panel first.";
            string buf_Topic = "Warning! - Plugins setting reminder.";
            if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                buf_Msg = "The application will save both new Configuration settings to the System and restart the Program.\n" +
                                    "Click YES to proceed and restart the program.\n" +
                                    "or\n" +
                                    "Click NO to Cancel and return to setting.\n\n" +
                                    "To reset to previous setting Click NO then Click RESET button";
                buf_Topic = "Save Setting Confirmation.";
                if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Properties.Settings.Default.IP_Robot = txbRobotIP.Text;
                    Properties.Settings.Default.IP_PLC = txbPLCIP.Text;
                    Properties.Settings.Default.PM3_ERP = (rdoPM2.Checked ? false : true);

                    Properties.Settings.Default.UI_DarkMode = (rdoLightMode.Checked ? false : true);

                    Properties.Settings.Default.BDGap_woFinger = (int)numBDGapWOFinger.Value;
                    Properties.Settings.Default.BDGap_wFinger = (int)numBDGapWFinger.Value;

                    Properties.Settings.Default.Finger_Min_BDLength = (int)numMinBDLFinger.Value;
                    Properties.Settings.Default.Finger_Max_BDLength = (int)numMaxBDLFinger.Value;

                    Properties.Settings.Default.Finger_Min_BDWidth = (int)numMinBDWFinger.Value;
                    Properties.Settings.Default.Finger_Max_BDWidth = (int)numMaxBDWFinger.Value;

                    Properties.Settings.Default.Grip2BD_Max_BDHeight = (int)nmuStackMaxBundleHeight.Value;
                    Properties.Settings.Default.SQFrame_Height = (int)numSQPlateH.Value;
                    Properties.Settings.Default.Max_idvBD_OVH_Percent = (int)numPercentOnPallet.Value;
                    Properties.Settings.Default.Save();

                    restartFile();
                }
            }
        }
        private void restartFile()
        {
            string RestartFile = Application.StartupPath + "\\Log\\RestartApp.bat";
            string[] lines = { "@echo off", "taskkill /f /im ATD_ID4P.exe", "Start " + Application.StartupPath + "\\ATD_ID4P.exe", "exit" };

            var ReFile = File.Create(RestartFile);
            ReFile.Close();

            File.WriteAllLines(RestartFile, lines);
            System.Diagnostics.Process.Start(RestartFile);
        }

        private void btnResetConfig_Click(object sender, EventArgs e)
        {
            string buf_Msg = "the Configuration setting will be reset to previous value.\n"+
                                "Click YES to proceed\n"+
                                "Click NO to cancel";
            string buf_Topic = "Reset Configuration setting confirmation.";
            if (MessageBox.Show(buf_Msg,buf_Topic, MessageBoxButtons.YesNo,MessageBoxIcon.Error) == DialogResult.Yes)
            {
                LoadAppConfig();
            }
        }

        private void btnApplyPlugin_Click(object sender, EventArgs e)
        {
            string buf_Msg = "";
            string buf_Msg2 = "";
            string buf_Topic = "";
            bool buf_fail = false; 
            if (string.IsNullOrEmpty(txbLineToken.Text) && rdoLineON.Checked)
            {
                buf_Msg = "Line Notification function is selected but doesn't have TOKEN\n"+
                            "Please assign TOEKN and try again, click TEST to see how to acquire TOKEN.";
                buf_fail = true;
            }
            if (string.IsNullOrEmpty(txbGrafanaAPIUrl.Text) && rdoGrafanaON.Checked)
            {
                buf_Msg2 = "Grafana Dashboard function is selected but doesn't have Url\n" +
                            "Please assign Url and try again.";
                buf_fail = true;
            }

            if (buf_fail)
            {
                buf_Topic = "Error! - TOEKN or Url is empty";
                MessageBox.Show(buf_Msg + buf_Msg2, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                buf_Msg = "The Application will save new Line's TOKEN and Grafana's Url to the System.\n" +
                                    "Click YES to proceed.\n" +
                                    "or" +
                                    "Click NO to cancel.";
                buf_Topic = "Save Plugins Confirmation";
                if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Properties.Settings.Default.PlugIn_LineNoti = rdoLineON.Checked;
                    Properties.Settings.Default.PlugIn_LineNoti_Token = txbLineToken.Text;
                    Properties.Settings.Default.PlugIn_Grafana = rdoGrafanaON.Checked;
                    Properties.Settings.Default.PlugIn_Grafana_Url = txbGrafanaAPIUrl.Text;
                    Properties.Settings.Default.Save();
                }
            }

        }
        private void btnResetPlugin_Click(object sender, EventArgs e)
        {
            string buf_Msg = "the Plugins setting will be reset to previous value.\n" +
                                "Click YES to proceed\n" +
                                "Click NO to cancel";
            string buf_Topic = "Reset Plugins setting confirmation.";
            if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                LoadPluginConfig();
            }
        }

        private void rdoGrafanaON_CheckedChanged(object sender, EventArgs e)
        {
            //RadioButton cb = (RadioButton)sender;
            //cb.Checked = true;  // This will not change the value of the "Checked" property in the original object (sender)
            //sender.Checked = false;  // This will change the value of the "Checked" property in both the original object (sender) and in "cb"

            RadioButton cb = (RadioButton)sender;
            txbGrafanaAPIUrl.ReadOnly = !cb.Checked;
        }

        private void btnLineTest_Click(object sender, EventArgs e)
        {
            // sender is Type Object which is a general using type so u need to case it to RadioButton
            string Token = Properties.Settings.Default.PlugIn_LineNoti_Token;
            if (string.IsNullOrEmpty(Token))
            {
                string buf_Msg = "Please insert TOKEN for Line notification\n" +
                                    "for instruction on how to get it\n\n" +
                                    "Please visit: https://notify-bot.line.me/my/ \n" +
                                    "Log-in -> click on your profile -> click My page ->\n"+
                                    "click Generate token -> select you group line.";
                string buf_Topic = "Error! - No Line notification Token";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LineNotifyCls Line = new LineNotifyCls(Token);
            Line.SendNotify("Hello from "+txbSystemName.Text+"'s ID4P");
        }
        private void rdoLineON_CheckedChanged(object sender, EventArgs e)
        {
            // sender: Refer to the object that raised the event (in this case rdbLineON)
            RadioButton cb = (RadioButton)sender;
            txbLineToken.ReadOnly = !cb.Checked;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            string buf_Msg = "The application will close setting window without saving any new seting value.\n" +
                                "Click YES to proceed.\n" +
                                "or\n" +
                                "Click NO to Cancel and return to setting.\n\n" +
                                "If you want to save your setting click NO then click SAVE button";
            string buf_Topic = "Save Setting Confirmation.";
            if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                var buf_fMain = UICls.FindOpenForm("frmMain");
                if (buf_fMain != null)
                {
                    frmMain fMain = (frmMain)buf_fMain;
                    fMain.Disable_MainMenu(true, false, false, false);
                }
                this.Close();
                this.Dispose();
            }
        }
    }
}
