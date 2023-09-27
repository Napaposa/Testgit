using ATD_ID4P.Class;
using ATD_ID4P.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATD_ID4P
{
    public partial class frmSplitSheet : Form
    {
        public string OrderID_Split;
        public string MatNo_Split;
        public int SheetPerBundle;
        public int OrderState;
        //private Int32 AmountSheet;
        private int indexSO = 20; //จำนวนการ Split SO
        //private int MaxSheet = 100000; //จำนวนน Sheet มากสุด
        private LogCls Log = new LogCls();
        private SqlCls Sql = new SqlCls();
        private OrdersBundleModel OBD = new OrdersBundleModel();

        public frmSplitSheet()
        {
            InitializeComponent();
        }

        private void frmSplitSheet_Load(object sender, EventArgs e)
        {
            lblJobDetail.Text = "MatrialNo. : " + MatNo_Split + "  Amount of Sheet per Bundle: " + SheetPerBundle;
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            dgvSplitSheet.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);
            dgvSplitSheet.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSplitSheet.EnableHeadersVisualStyles = false;
            dgvSplitSheet.RowHeadersVisible = false;
        }

        private void LoadData()
        {
            string loc_SheetPerSO = "";
            string loc_SOID = "";
            string Query = String.Format(@"SELECT SplitSheet, SOSplit from tblOrder where ID = {0} and Material_No = '{1}'", OrderID_Split, MatNo_Split);
            var dtSO = Sql.GetDataTableFromSql(Query);
            if (dtSO != null && dtSO.Rows.Count > 0)
            {
                loc_SheetPerSO = dtSO.Rows[0]["SplitSheet"].ToString();
                loc_SOID = dtSO.Rows[0]["SOSplit"].ToString();
            }

            if (!string.IsNullOrEmpty(loc_SheetPerSO) && !string.IsNullOrEmpty(loc_SOID))
            {
                // Split string in to OrderBundleModel OB
                // then assign OB value into OrdersBundleModel list OBs
                OBD.GetOrderBundlefromString(loc_SheetPerSO, SheetPerBundle, loc_SOID);
            }
            else
            {
                OBD.OBs = new List<OrderBundleModel>();
            }

            bdsSplitSheet.DataSource = OBD.OBs;
            dgvSplitSheet.DataSource = bdsSplitSheet;
            dgvSplitSheet.AutoResizeColumns();
            int LastRow = (OBD.OBs.Count > 0 ? OBD.OBs.Count - 1 : 0);
            if (LastRow == 0)
                bdsSplitSheet.AddNew();
            dgvSplitSheet.CurrentCell = dgvSplitSheet.Rows[LastRow].Cells[1];

            if (OrderState == 1 || OrderState == 2)
            {
                btnSave.Enabled = false;
                dgvSplitSheet.Enabled = false;
            }
            else
            {
                btnSave.Enabled = true;
                dgvSplitSheet.Enabled = true;
            }
        }

        private void dgvSplitSheet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dgvSplitSheet_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(dgvSplitSheet_KeyPress);
            if (dgvSplitSheet.CurrentCell.ColumnIndex == 1) //AmountSheet
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(dgvSplitSheet_KeyPress);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool IsComplete = SaveData();

            if (IsComplete == true)
            {
                this.Close();
                this.Dispose();
            }
        }

        private void BindingSource1_AddingNew(object sender, AddingNewEventArgs e)
        {
            OrderBundleModel _OB = new OrderBundleModel();
            e.NewObject = _OB;
            var NoMax = (OBD.OBs.Count > 0 ? OBD.OBs.Max(x => x.No) : 0);
            _OB.No = NoMax + 1;
            if (OBD.OBs.Count > 0)
            {
                var _LOB = OBD.OBs[OBD.OBs.Count - 1];
                if (_LOB != null)
                    _OB.AmountSheet = _LOB.AmountSheet;
            }
            _OB.SheetPerBundle = SheetPerBundle;
            _OB.SONo = _OB.No.ToString();
        }

        private void BindingSource1_CurrentItemChanged(object sender, EventArgs e)
        {
            OrderBundleModel _OB = (OrderBundleModel)((BindingSource)sender).Current;
            //--Verify Data--
            //1.Check MAX
            //if(OBD.OBs.Sum(x => x.AmountSheet) > MaxSheet)
            //{
            //    MessageBox.Show("จำนวน sheet มากเกินไป (ค่าสูงสุด "+MaxSheet.ToString("N0")+")", "จำนวน Sheet ไม่ถูกต้อง!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    _OB.AmountSheet = 0;
            //    return;
            //}
            int _MaxSheet = 20000;
            if (_OB.AmountSheet > _MaxSheet)
            {
                string buf_Msg = "Sheet per SO cannot be more than " + _MaxSheet.ToString("N0");
                string buf_Topic = "Error! - Exceed Maximum Sheet Per SO.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
                _OB.AmountSheet = 0;
                return;
            }
            _OB.GetAmountBundle();
            lblTotalSheet.Text = string.Format("Total Sheet: {0:N0}", OBD.OBs.Sum(x => x.AmountSheet));
        }

        private bool SaveData()
        {
            bool IsComplete = true;

            //--Clear Zero Add Last--
            for (int i = OBD.OBs.Count - 1; i > 0; i--)
            {
                OrderBundleModel _OB = OBD.OBs[i];
                if (_OB.AmountSheet == 0 || _OB.AmountBundle == 0)
                    OBD.OBs.RemoveAt(i);
                else
                    break;
            }

            //Check Zero in Middle
            var CheckZero = OBD.OBs.Where(x => x.AmountSheet == 0 || x.AmountBundle == 0).Count();
            var AllZero = OBD.OBs.Sum(x => x.AmountSheet);
            if (CheckZero > 0 && AllZero > 0)
            {
                string buf_Msg = "Some of the Value is 0, Please Verify the input value again.";
                string buf_Topic = "Error! - 0 value is not allowed.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            string SplitSoText = OBD.GetOrderSheetText();
            string SOSplit = OBD.GetSOText();
            string Query = string.Format(@"UPDATE tblOrder SET SplitSheet = '{1}', SOSplit = '{2}',AutoLotEnd = '{3}'  WHERE ID = {0}", OrderID_Split, SplitSoText, SOSplit, chkbxAutoEnd.Checked);
            bool CanClear = Sql.ExcSQL(Query);
            if (CanClear == false)
            {
                string buf_Msg = "Can not save Split Sheet to the Database.";
                string buf_Topic = "Error! - Can not save Split Sheet to tblOrder.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log.WriteLog("Error! - Can not save Split Sheet to tblOrder.", LogType.Fail);
                return false;
            }

            return IsComplete;
        }

        private void DgvSplitSheet_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (dgvSplitSheet.Rows.Count > indexSO)
            {
                //MessageBox.Show("จำนวน SO เกินกว่าที่ตั้งไว้");
                dgvSplitSheet.AllowUserToAddRows = false;
            }
        }

        private void DgvSplitSheet_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void DgvSplitSheet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvSplitSheet.Rows.Count >= 20)
                {
                    string buf_Msg = "Only 20 SO is allow per Order.";
                    string buf_Topic = "Error! - Maximum Split SO reached.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // If Selected Cell is at ColumnIndex 1 move to Selected Cell at Column Index 3 of the same row instead
                if (dgvSplitSheet.CurrentCell.ColumnIndex == 1)
                {
                    dgvSplitSheet.CurrentCell = dgvSplitSheet[3, dgvSplitSheet.CurrentCell.RowIndex];
                    return;
                }

                if (dgvSplitSheet.CurrentRow.Index == dgvSplitSheet.Rows.Count - 1)
                {
                    bdsSplitSheet.AddNew();
                    dgvSplitSheet.CurrentCell = dgvSplitSheet[1, dgvSplitSheet.CurrentCell.RowIndex];
                    e.Handled = true;
                }
            }
        }
    }


}
