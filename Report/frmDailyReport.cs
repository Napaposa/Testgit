using ATD_ID4P.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
//*using ATD_ID4P.Model; // unnecessary

namespace ATD_ID4P.Report
{
    public partial class frmDailyReport : Form
    {
        private SqlCls _Sql = new SqlCls();
        private ExcelCls _Excel = new ExcelCls();
        //private clsUI UICls = new clsUI();

        public frmDailyReport()
        {
            InitializeComponent();
        }

        private void frmDailyReport_Load(object sender, EventArgs e)
        {
            SetDisplayMode();
        }
        private void SetDisplayMode()
        {
            if (Properties.Settings.Default.UI_DarkMode == false)
            {
                this.BackColor = Color.White;
                pnlMenu.BackColor = Color.White;
                lblOrderDate.ForeColor = Color.Black;
            }

        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void BtnGetData_Click(object sender, EventArgs e)
        {
            GetData(null, null);
        }
        private void GetData(DateTime? _StartDate = null, DateTime? _EndDate = null)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("th-TH");

            DateTime StartDate = Convert.ToDateTime((_StartDate == null ? this.dtpStart.Value : _StartDate.Value), _cultureInfo);
            DateTime EndDate = Convert.ToDateTime((_EndDate == null ? this.dtpEnd.Value : _EndDate.Value), _cultureInfo);
            EndDate = EndDate.AddDays(1);

            string Query = @"SELECT 
	                            FORMAT(CompletedDate, 'dd/MM/yyyy HH:mm') FinishTime,
	                            Material_No Material,
	                            Pattern_Code Pattern,
	                            TotalSheet,
	                            -- (BDPerLY*LYPerPallet*PiecePerBD) SheetPerPallet,
	                            -- TotalPallet,
	                            -- AvgUsedTime,	
                                BDPerGRip Stack,
	                            PiecePerBD SheetPerBundle,
	                            CASE WHEN ISNULL((BDPerLY*LYPerPallet*PiecePerBD),0) = 0 OR ISNULL(AvgUsedTime,0) = 0 THEN 0 ELSE CONVERT(decimal(18,2), Round((BDPerLY*LYPerPallet*PiecePerBD) / AvgUsedTime, 3)) END Speed,
	                            TotalWorkingTime WorkingTime
                            FROM 
	                            tblOrder 
                            WHERE 
	                            OrderState = '3'
                                AND AvgUsedTime > 0
	                            AND CompletedDate BETWEEN '@StartDate 07:30:00' AND '@EndDate 07:29:59'
	                            AND TotalSheet > ISNULL((BDPerLY*LYPerPallet*PiecePerBD),0)";

            Query = Query.Replace("@StartDate", StartDate.ToString("yyyy-MM-dd", _cultureInfo)).Replace("@EndDate", EndDate.ToString("yyyy-MM-dd", _cultureInfo));

            DataTable dt = _Sql.GetDataTableFromSql(Query);
            dgvMain.DataSource = dt;
            dgvMain.Refresh();
            dgvMain.AutoResizeColumns();
        }


        private void BtnExportToExcel_Click(object sender, EventArgs e)
        {
            CreateReport();
        }
        public void CreateReport(DateTime? _StartDate = null, DateTime? _EndDate = null, string SavePath = "", bool HiddenMode = false)
        {
            try
            {
                if (HiddenMode == false)
                    UICls.ShowWaiting();
                GetData(_StartDate, _EndDate);
                DataTable dt = (DataTable)dgvMain.DataSource;
                if (dt.Rows.Count == 0 || dt == null)
                {
                    if (HiddenMode == false)
                        UICls.ShowWaiting(false);
                    return;
                }

                System.Globalization.CultureInfo _cultureEnInfo = new System.Globalization.CultureInfo("en-US");
                DateTime StartDate = Convert.ToDateTime((_StartDate == null ? this.dtpStart.Value : _StartDate.Value), _cultureEnInfo);
                DateTime EndDate = Convert.ToDateTime((_EndDate == null ? this.dtpEnd.Value : _EndDate.Value), _cultureEnInfo);
                string OrderDate = string.Format("{0} - {1}", StartDate.ToString("yyyy-MM-dd", _cultureEnInfo), EndDate.ToString("yyyy-MM-dd", _cultureEnInfo));
                _Excel.Init();
                if (HiddenMode == false)
                    UICls.ShowWaiting(true, "Excel", "Creating...");
                _Excel.OpenWorkBook(Application.StartupPath + @"\ReportTemplate\DailyReport_Template.xltx");
                _Excel.WriteCell(OrderDate, 1, 2);
                _Excel.DataTableToWorkSheet(dt, false, 5, 1);
                if (HiddenMode == false)
                    UICls.ShowWaiting(false);
                _Excel.SaveFile(SavePath, string.Format("DailyReport_{0}_{1}", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")));
            }
            catch (Exception)
            {
                if (HiddenMode == false)
                    UICls.ShowWaiting(false);
            }
        }

    }
}
