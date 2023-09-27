using ATD_ID4P.Class;
using System;
using System.Data;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;

// Check GetData() again for SQL query

namespace ATD_ID4P.Report
{
    public partial class frmOrderReport : Form
    {
        private SqlCls SqlEx = new SqlCls();
        private ExcelCls _Excel = new ExcelCls();

        public frmOrderReport()
        {
            InitializeComponent();
        }

        private void frmOrderRrport_Load(object sender, EventArgs e)
        {
            SetDisplayMode();
        }
        private void SetDisplayMode()
        {
            if (Properties.Settings.Default.UI_DarkMode == false)
            {
                pnlMenu.BackColor = Color.White;
                lblOrderDateTopic.ForeColor = Color.Black;
                lblWorkTimeTopic.ForeColor = Color.Black;
                lblOrderStateTopic.ForeColor = Color.Black;
                rdoAllTime.ForeColor = Color.Black;
                rdoDayTime.ForeColor = Color.Black;
                rdoNightTime.ForeColor = Color.Black;
                rdoAllState.ForeColor = Color.Black;
                rdoCompleteState.ForeColor = Color.Black;
                rdoUncompleteState.ForeColor = Color.Black;
            }

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            GetData();
        }
        private void GetData()
        {
            System.Globalization.CultureInfo _cultureEnInfo = new System.Globalization.CultureInfo("en-US");

            DateTime StartDate = Convert.ToDateTime(this.dtpStart.Value, _cultureEnInfo); //dtpStart.Value;
            DateTime EndDate = Convert.ToDateTime(this.dtpEnd.Value, _cultureEnInfo);  //dtpEnd.Value;
            string State = GetSelectState(); // Check OrderState Option CheckBox
            string StateCondition = "";
            switch (State)
            {
                case "All": { StateCondition = "'0', '1', '2', '3', '9'"; break; }
                case "Complete": { StateCondition = "'3'"; break; }
                case "UnComplete": { StateCondition = "'0', '1', '2', '9'"; break; }
            }
            string WorkTime = GetSelectWorkTime(); // Check WorkTime Option CheckBox
            string MinTime = "00:00:00";
            string MaxTime = "00:00:00";
            switch (WorkTime)
            {
                case "All": { MinTime = "00:00:00"; MaxTime = "23:59:59"; break; }
                case "Day": { MinTime = "07:30:00"; MaxTime = "19:30:00"; break; }
                case "Night": { MinTime = "19:30:01"; MaxTime = "07:29:59"; EndDate = EndDate.AddDays(1); break; }
            }

            string Query = @"SELECT 
	                            StampDate,
	                            Material_No [MaterialNo],
	                            Product_Code,
	                            Pattern_Code [Pattern],
	                            BDPerGrip [Stack],
	                            TS_everyXLY [TieSheetEvery],
	                            CASE WHEN OrderState = 3 THEN 'COMPLETE' ELSE 
		                            CASE WHEN OrderState = 2 THEN 'USING' ELSE
			                            CASE WHEN OrderState = 1 THEN 'BUFFER' ELSE
				                            CASE WHEN OrderState = 9 THEN 'ERROR' ELSE '' END
			                            END
		                            END
	                            END OrderState,
	                            ISNULL(Amount, 0) BundleAmount,
	                            (BDPerLY*LYPerPallet) [BundlePerPallet],
	                            (BDPerLY*LYperPallet*PiecePerBD) [SheetPerPallet],
                                ISNULL(TotalSheet, 0) TotalSheet,
	                            CASE WHEN ISNULL(Amount, 0) > 0 THEN CASE WHEN Amount > (BDPerLY*LYPerPallet) THEN ISNULL(TotalPallet,0) ELSE 1 END ELSE 0 END TotalPallet,
	                            ISNULL(AvgUsedTime, 0) AvgUsedTimePerPallet,
	                            CompletedDate,
                                ISNULL(TotalWorkingTime,0) UsedTime
                                -- ISNULL(Lot_No,'') Lot_No
                            FROM 
	                            tblOrder
                            WHERE
                                FORMAT(StampDate, 'yyyy-MM-dd') BETWEEN '@StartDate' AND '@EndDate'
                                AND (FORMAT(StampDate, 'HH:mm:ss') BETWEEN '@MinTime' AND '@MaxTime' @NightCondition)
	                            AND OrderState IN (@OrderState)
                            ORDER BY
	                            StampDate DESC";

            //Query = Query.Replace("@StartDate", StartDate.ToString("yyyy-MM-dd ", _cultureEnInfo) + MinTime).Replace("@EndDate", EndDate.ToString("yyyy-MM-dd ", _cultureEnInfo) + MaxTime);
            Query = Query.Replace("@StartDate", StartDate.ToString("yyyy-MM-dd", _cultureEnInfo)).Replace("@EndDate", EndDate.ToString("yyyy-MM-dd", _cultureEnInfo));
            if (WorkTime == "Night")
            {
                Query = Query.Replace("@MinTime", MinTime).Replace("@MaxTime", "23:59:59");
                Query = Query.Replace("@NightCondition", "OR FORMAT(StampDate, 'HH:mm:ss') BETWEEN '00:00:00' AND '07:29:59'");
            }
            else
            {
                Query = Query.Replace("@MinTime", MinTime).Replace("@MaxTime", MaxTime);
                Query = Query.Replace("@NightCondition", "");
            }
            Query = Query.Replace("@OrderState", StateCondition);

            DataTable dt = SqlEx.GetDataTableFromSql(Query);
            dgvMain.DataSource = dt;
            dgvMain.Refresh();
            dgvMain.AutoResizeColumns();
            lblRowAmount.Text = "Row: " + dt.Rows.Count;
        }
        private string GetSelectState()
        {
            // Check OrderState Option CheckBox
            string State = "All";

            if (rdoAllState.Checked) { State = "All"; return State; }
            if (rdoCompleteState.Checked) { State = "Complete"; return State; }
            if (rdoUncompleteState.Checked) { State = "UnComplete"; return State; }

            return State;
        }
        private string GetSelectWorkTime()
        {
            // Check WorkTime Option CheckBox
            string WorkTime = "All";

            if (rdoAllTime.Checked) { WorkTime = "All"; return WorkTime; }
            if (rdoDayTime.Checked) { WorkTime = "Day"; return WorkTime; }
            if (rdoNightTime.Checked) { WorkTime = "Night"; return WorkTime; }

            return WorkTime;
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                UICls.ShowWaiting();
                GetData();
                DataTable dt = (DataTable)dgvMain.DataSource;
                if (dt.Rows.Count == 0 || dt == null)
                {
                    UICls.ShowWaiting(false);
                    return;
                }

                System.Globalization.CultureInfo _cultureEnInfo = new System.Globalization.CultureInfo("en-US");
                DateTime StartDate = Convert.ToDateTime(this.dtpStart.Value, _cultureEnInfo); //dtpStart.Value;
                DateTime EndDate = Convert.ToDateTime(this.dtpEnd.Value, _cultureEnInfo);  //dtpEnd.Value;
                string OrderDate = string.Format("{0} - {1}", StartDate.ToString("yyyy-MM-dd", _cultureEnInfo), EndDate.ToString("yyyy-MM-dd", _cultureEnInfo));
                string WorkTime = GetSelectWorkTime();
                OrderDate = OrderDate + " [" + WorkTime + "]";
                _Excel.Init();
                UICls.ShowWaiting(true, "Excel", "Creating...");
                _Excel.OpenWorkBook(Application.StartupPath + @"\ReportTemplate\OrderReport_Template.xltx");
                _Excel.WriteCell(OrderDate, 1, 2);
                _Excel.WriteCell(GetSelectState(), 3, 2);
                _Excel.DataTableToWorkSheet(dt, false, 6, 1);
                UICls.ShowWaiting(false);
                _Excel.SaveFile("", string.Format("OrderReport_{0}_{1}", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")));
            }
            catch (Exception)
            {
                UICls.ShowWaiting(false);
            }
        }
    }
}
