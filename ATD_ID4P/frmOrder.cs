using ATD_ID4P.Class;
using ATD_ID4P.Model;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ATD_ID4P
{
    public partial class frmOrder : Form
    {
        public frmOrder()
        {
            InitializeComponent();
        }

        private SqlCls Sql = new SqlCls();
        public PLCCommu PLCCom = new PLCCommu();
        private ABBCommu ABBCom = new ABBCommu();
        private RBWorkingModel Rob_WorkData = new RBWorkingModel();
        private Model.ClientDataModel ClientData = new Model.ClientDataModel();
        private string _RobotIP = Properties.Settings.Default.IP_Robot;
        private string _PLCIP = Properties.Settings.Default.IP_PLC;
        private frmTiesheet frmTieSheet = new frmTiesheet();
        private LogCls Log = new LogCls();
        private string LastMatNo = "";
        private Bundle3DCls running_Order3DBD = new Bundle3DCls();

        private int RobotModel = Properties.Settings.Default.RobotModel;

        private PatternModel Running_ptm = new PatternModel();
        private OrderModel Running_odm = new OrderModel();
        private PatternModel Selected_ptm = new PatternModel();
        private OrderModel Selected_odm = new OrderModel();
        //private UIFeedDataModel Running_FeedData = new UIFeedDataModel();
        private UIFeedDataModel Latest_FeedData = new UIFeedDataModel();

        //color display set 
        private Color colorState_PLC_ROB = new Color();//Top@
        private Color ColorBtn = new Color();
        //public static string TieSheet;
        private int TotalErrorRobotFeed = 0;
        private bool ConfirmStartError = false;
        private bool IsOnGetRobotFeed = false;

        private bool flag_ResetData = false; // will be use to distinguish Normal LotEnd and LotEnd from Reset Data
                                             // PLC need LotEnd Sequence from Reset Data to complete the sequence and unblock sendding data.

        public enum OrderState
        {
            Using = 2,
            Buffer = 1,
            Error = 9,
            Normal = 0
        }

        private void FrmOrder_Load(object sender, EventArgs e)
        {
            SetDisplayMode();
            // - Set the Colour objects according to DarkMode Setting Variable (via displaymode variable)

            InitUI();
            // - Set the Colour objects according to DarkMode Setting Variable (via displaymode variable)
            // - Create button for SplitSO in Order's DataTable if EnableSplitSO setting variable is true

            SetMCIPText();
            // - Dispaly IP of the connected Robot and PLC

            //Get Last ClientData @Top
            ClientData.LoadTSSizeOptions();
            // - Get Latest Client Data from ClientData.json
            lblTieSheetSize.Text = ClientData.Selected_TSText;
            lblSizePallet.Text = "1000x1200x155";
            lblPalletType.Text = "Wooden";
            // - Update TieSheet Size display text
            // - Update Pallet Size dispaly text
            // - Update Pallet Type dispaly text


            //CheckPLC Connection
            PLCCom.CheckPLCIsActive(_PLCIP);
            ShowPLCConnection(PLCCom.PLCActive);
            // - Check PLC Connection and assign to PLCCom variable
            // - Update lblPLCActive object according to return status

            if (RobotModel == 1)
            {
                ABBCom.CheckRobotIsActive(_RobotIP);
                ShowRobotConnection(ABBCom.RobotActive);
            }
            else
            {
                ShowRobotConnection(PLCCom.PLCActive && PLCCom.CheckRobotIsActive());
            }
            // - Check Robot Connection and assign to RobotActive variable
            // - Update lblRobotActive object according to return status
            // * For Kawa Model there are no connect from ID4P to the Robot

            OrderGrid_ImportOrderData();
            // - Update Order info from "tblOrder" to the DataTable 
            //*OrderGrid_GetCurrentOrderData(false, true);
            DataGridViewRow buf_CheckRow = OrderGrid_FindOrderByState(OrderState.Using, true, false, true);
            // - Update current order TextBox

            //Start Check LotEnd if have running order
            if (Get_OrderState(buf_CheckRow) == (int)OrderState.Using)
            {
                tmrLotEnd.Start();
            }
            // - Check if there is a Running Order in DataGrid
            // ** every 5s ID4P will read PC_LotEnd_Started from PLC if PLCCom.PLCActive is true (from Check_PLC_LotEnd via TmrLotEnd_Tick)
            // -> If PC_LotEnd_Started is TRUE
            //      - get order data and amount of placed bundle / pallet time from PLC
            //      - get Material NO and ID of current running order from tblOrder
            //      - update new ExtSq_Adj
            //      - update current Master Data to tblOrder
            //          - SheetHeight (Bundle Height)   * also update to tblMaster
            //          - Soft Placing                  * also update to tblMaster
            //          - Amount of Placed Bundles
            //      - send PC_LotEnd_Ack to PLC if update complete
            //      - removed this order from Data Grid
            //      - start Confirm Start Timer
            //      - stop Lot End Timer

            //Check Buffer Order
            else if (OrderGrid_FindOrderByState(OrderState.Buffer, true) != null)
                tmrConfirmStart.Start();
            // - Check if there is a Buffer Order in DataGrid 
            // - if True Start Timer
            // ** every 5s ID4P will read PC_Start_Done from PLC (from Check_PLC_ConfirmStart via TmrConfirmStart_Tick)
            // -> if PC_Start_Done is TRUE
            //      - Change the order OrderState to 1 (Running)
            //      - Start LotEnd Timer to check for LotEnd Command
            //      - Update Order info from "tblOrder" to the DataTable (GetOrderData)
            //      - Stop Confirm Start Timer
            //      - Update current order TextBox (OrderGrid_GetCurrentOrderData)
            // -> if PC_Start_Done is FALSE
            //      - If ConfirmStartError is true then return immediatly
            //      - else
            //      - Check PC_CommuError from PLC
            //          -> if true
            //              - set ConfirmStartError to True
            //              - Stop Confirm Start Timer
            //              - Block Send Data


            //Start Timer
            //tmrLotEnd.Start();
            tmrRobotFeed.Start();
            // - Read running data every 5s (from GetRobotFeed_Event via TmrRobotFeed_Tick)
            // - Use Data to Update 3D Model (only 1st time of this Material NO)
            // - Update Current Order Text Box, 3D, Progress Bar, Percent Progress (via RobotFeedUI_LoadValue)

        }

        #region LoadMethod
        private void SetDisplayMode()
        {
            if (Properties.Settings.Default.UI_DarkMode == false)
            {
                lblPallet_Title.ForeColor = Color.Black;
                lblTieSheetSize_Title.ForeColor = Color.Black;
                lblFinger_Title.ForeColor = Color.Black;
                lblRobotSpeed_Title.ForeColor = Color.Black;
                colorState_PLC_ROB = ColorTranslator.FromHtml("#0c6aab");
                btnEditTS.BackColor = Color.RoyalBlue;
                btnEditTS.ForeColor = Color.White;
                Color ColorLb1 = ColorTranslator.FromHtml("#8eaee6");
                Color ForeColor = Color.Black;
                ColorBtn = ColorTranslator.FromHtml("#377cab");

                pnlSimulate.BackColor = Color.LightGray;
                this.BackColor = Color.White;

                pnlTieSheetSize.BackColor = ColorLb1;
                lblTieSheetSize.BackColor = ColorLb1;
                lblTieSheetSize.ForeColor = ForeColor;


                lblSizePallet.ForeColor = Color.Green;
                pnlPallet.BackColor = ColorLb1;

                //*pnlSquaringStatus.BackColor = ColorLb1;
                pnlRobSpeed.BackColor = ColorLb1;
                pnlGripperFinger.BackColor = ColorLb1;

                btnNewOrder.BackColor = ColorBtn;
                btnRefresh.BackColor = ColorBtn;
                btnSendData.BackColor = ColorBtn;
                btnResetData.BackColor = ColorBtn;

                //*lblSQX.ForeColor = ForeColor;
                //*lblSQY.ForeColor = ForeColor;

                lblPalletType.ForeColor = ForeColor;

                groupBox11.ForeColor = ForeColor;

                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl.GetType().ToString() == "System.Windows.Forms.Label")
                    {
                        Label Lb = (Label)ctrl;
                        Lb.ForeColor = ForeColor;
                    }
                }
                //label1.ForeColor = ForeColor;
                //label3.ForeColor = ForeColor;
                //label5.ForeColor = ForeColor;
                // lblRobotIP.ForeColor = ForeColor;
                pnlRobotStatus.BackColor = ColorTranslator.FromHtml("#0c6aab");
                pnlPLCStatus.BackColor = ColorTranslator.FromHtml("#0c6aab");
                pnlSimRun.BackColor = ColorTranslator.FromHtml("#a3a3a3");

                lblPalletPercent.ForeColor = Color.Black;

            }
            else
            {
                //ColorBtn = Color.Black;
                colorState_PLC_ROB = Color.Black;
                ForeColor = Color.White;
                ColorBtn = Color.Black;

            }
        }
        private void InitUI()
        {
            //Load Default Event
            btnNewOrder.MouseHover += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.Yellow));
            btnNewOrder.MouseLeave += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));

            btnSendData.MouseHover += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.Yellow));
            btnSendData.MouseLeave += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));

            btnRefresh.MouseHover += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.Yellow));
            btnRefresh.MouseLeave += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));

            dgvOrderList.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            dgvOrderList.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvOrderList.EnableHeadersVisualStyles = false;
            dgvOrderList.RowHeadersVisible = false;

            dgvOrderList.Columns.Insert(0, new UICls.DeleteColumn());

            //Top add 22-07-2020 for Split Sheet
            DataGridViewButtonColumn btnSOAmount = new DataGridViewButtonColumn();
            {
                btnSOAmount.Text = "...";
                btnSOAmount.Name = "Amount";
                btnSOAmount.UseColumnTextForButtonValue = true;
                btnSOAmount.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.AllCells;
                btnSOAmount.FlatStyle = FlatStyle.Popup;
                btnSOAmount.CellTemplate.Style.BackColor = Color.YellowGreen;
                btnSOAmount.DisplayIndex = 1;
                btnSOAmount.CellTemplate.Style.Font = new Font("Tahoma", 10);
                btnSOAmount.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                btnSOAmount.CellTemplate.Style.ForeColor = Color.Black;
            }
            dgvOrderList.Columns.Insert(1, btnSOAmount);
            //-----------------------------------
        }
        private void SetMCIPText()
        {
            if (RobotModel == 1)
            {
                lblRobotIP.Text = Properties.Settings.Default.IP_Robot;
            }
            else
            {
                lblRobotIP.Text = "Through PLC";
            }
            lblPLCIP.Text = Properties.Settings.Default.IP_PLC;
        }

        private void ShowPLCConnection(bool Connected = false)
        {

            if (Connected == false)
            {
                lblPLCActive.ForeColor = Color.Red;
                BackgroundWorker BW = new BackgroundWorker();
                BW.DoWork += DoWork;
                BW.RunWorkerAsync();
                void DoWork(object sender, DoWorkEventArgs e)
                {
                    int delay = 300; // 0.250 second
                    //int interval = 4000;
                    while (!e.Cancel)
                    {
                        Thread.Sleep(delay);
                        if (pnlPLCStatus.InvokeRequired)
                        {
                            pnlPLCStatus.Invoke((Action)Blink);
                        }
                        else
                        {
                            Blink();
                        }

                        int LastCheck = Convert.ToInt32(pnlPLCStatus.Tag);
                        if (LastCheck == 10)
                        {
                            LastCheck = 0;
                            PLCCom.CheckPLCIsActive(_PLCIP);
                        }
                        LastCheck++;
                        pnlPLCStatus.Tag = LastCheck;

                        if (PLCCom.PLCActive == true)
                        {
                            e.Cancel = true;
                            pnlPLCStatus.BackColor = colorState_PLC_ROB;
                            lblPLCActive.ForeColor = Color.SpringGreen;
                        }
                    }
                }
                void Blink()
                {
                    if (pnlPLCStatus.BackColor == colorState_PLC_ROB)
                        pnlPLCStatus.BackColor = Color.White;
                    else
                        pnlPLCStatus.BackColor = colorState_PLC_ROB;
                }
            }
            else
            {
                pnlPLCStatus.BackColor = colorState_PLC_ROB;
                lblPLCActive.ForeColor = Color.SpringGreen;
            }
        }
        private void ShowRobotConnection(bool Connected = false)
        {
            //Connected = true; //set for not connect to robot

            if (Connected == false)
            {
                lblRobotActive.ForeColor = Color.Red;
                BackgroundWorker BW = new BackgroundWorker();
                BW.DoWork += DoWork;
                BW.RunWorkerAsync();
                void DoWork(object sender, DoWorkEventArgs e)
                {
                    int delay = 300; // 0.250 second

                    while (!e.Cancel)
                    {
                        Thread.Sleep(delay);
                        if (pnlRobotStatus.InvokeRequired)
                        {
                            pnlRobotStatus.Invoke((Action)Blink);
                        }
                        else
                        {
                            Blink();
                        }

                        if (RobotModel == 1)
                        {
                            int LastCheck = Convert.ToInt32(pnlRobotStatus.Tag);
                            if (LastCheck == 10)
                            {
                                LastCheck = 0;
                                ABBCom.CheckRobotIsActive(_RobotIP);
                            }
                            LastCheck++;
                            pnlRobotStatus.Tag = LastCheck;
                            if (ABBCom.RobotActive == true)
                            {
                                e.Cancel = true;
                                pnlRobotStatus.BackColor = colorState_PLC_ROB;
                                lblRobotActive.ForeColor = Color.SpringGreen;
                            }
                        }
                        else
                        {
                            if (PLCCom.PLCActive && PLCCom.CheckRobotIsActive())
                            {
                                e.Cancel = true;
                                pnlRobotStatus.BackColor = colorState_PLC_ROB;
                                lblRobotActive.ForeColor = Color.SpringGreen;
                            }
                        }
                    }
                }
                void Blink()
                {
                    if (pnlRobotStatus.BackColor == colorState_PLC_ROB)
                        pnlRobotStatus.BackColor = Color.White;
                    else
                        pnlRobotStatus.BackColor = colorState_PLC_ROB;
                }
            }
            else
            {
                pnlRobotStatus.BackColor = colorState_PLC_ROB;
                lblRobotActive.ForeColor = Color.SpringGreen;
            }
        }

        public void OrderGrid_ImportOrderData()
        {
            if (dgvOrderList.InvokeRequired)
            {
                dgvOrderList.Invoke((Action)OrderGrid_ImportOrderData);
                return;
            }

            string Query = @"SELECT
                             StampDate OrderDate,
                             Material_No MaterialNo,	
                                Lot_No MO,
                             BDPerGrip BundlePerGrip,
                             TS_everyXLY TieSheet,
                             FoldWork_W SheetWidth,
                             FoldWork_L SheetLength,
                                FoldWork_T SheetHeight,
                                HeightRatio,
                                LYPerPallet LayerPerPallet,
                             FoldWork_Weight SheetWeight,
                                PiecePerBD SheetPerBundle,
                                TopSheet,
                                BtmSheet,
                             ID,
                             FixBDFace,
                             Product_Code ProductCode,
                                Pattern_Code,
                                Pallet_Type PalletType,
                                Pallet_W PalletWidth,
                                Pallet_L PalletLength,
                                Pallet_H PalletHeight,
                                BDPerLY BundlePerLayer,                                
                                OrderState,
                                Squaring,
                                SQ_Extra ExtraSQ,
                                SQ_OpenX,
                                SQ_OpenY,
                                SQ_CloseX,
                                SQ_CloseY,
                                GripperFinger,
                                RotatePattern,
                                SwitchBDSize,
                                Peri_CloseL,
                                Peri_CloseW,
                                SpecialRotate,
                                TS_B4LastLY,
                                TS_SQLayer,
                                StackerLiftBD,
                                AntiBounce StackerAntiBounce,
                                ExtraPickDepth,
                                PlacingMode,
                                DischargeMode,
                                PatternSeq,
                                SplitSheet,
                                SOSplit,
                                AutoLotEnd
                            FROM 
                             tblOrder
                            WHERE
                             OrderState != 3
                            ORDER BY
                             OrderState DESC";
            DataTable dtOrder = Sql.GetDataTableFromSql(Query);
            dgvOrderList.DataSource = dtOrder;
            dgvOrderList.Refresh();
            dgvOrderList.AutoResizeColumns();
            OrderGrid_UpdateRowColor();
            OrderGrid_DisableSort();
            btnRefresh.Focus();
            dgvOrderList.ClearSelection();
        }

        private DataGridViewRow OrderGrid_FindOrderByState(OrderState buf_OrderState, bool buf_GetData, bool GetDataOnly = false, bool Show3D = false)
        {
            DataGridViewRow result = null;
            string State = Convert.ToString((int)buf_OrderState);
            // 1. Check row of dgvOrderList
            // 2.a row is existed
            //      2.a.1 Find the 1st row that OrderState is 2 (Running)
            //      2.a.2 Store that in rOrder
            //      2.a.3.a rOrder is not null 
            //              - Get info of this order
            //              - Update text value of the following item from 
            //                  - txbMaterialCurrent
            //                  - txbStackCurrent
            //                  - txbTieSheetCurrent
            try
            {
                if (dgvOrderList == null)
                    return result;

                if (buf_GetData)
                {
                    if (dgvOrderList.InvokeRequired)
                    {
                        dgvOrderList.Invoke(new Action(delegate () { OrderGrid_FindOrderByState(buf_OrderState, buf_GetData, GetDataOnly, Show3D); }));
                        return result;
                    }
                }

                if (dgvOrderList.Rows != null && dgvOrderList.Rows.Count > 0)
                {
                    DataGridViewRow buf_MatchedStateRow = dgvOrderList.Rows
                                                .Cast<DataGridViewRow>()
                                                .Where(r => r.Cells["OrderState"].Value.ToString().Equals(State)).FirstOrDefault();

                    if (buf_MatchedStateRow != null)
                    {
                        if (buf_GetData)
                        {
                            if (buf_OrderState == OrderState.Buffer || buf_OrderState == OrderState.Using)
                            {
                                Running_ptm = GetPatternOrder(buf_MatchedStateRow, (GetDataOnly == true ? null : pnlSimRun), Show3D);
                                Running_odm = GetOrder(buf_MatchedStateRow);
                                Running_odm.TieSheetWidth = Latest_FeedData.TieSheet_Width;
                                Running_odm.TieSheetLength = Latest_FeedData.TieSheet_Length;
                            }
                        }
                        result = buf_MatchedStateRow;
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message != "Sequence contains no elements")
                    Log.WriteLog("Error! - While trying to find order by state: " + e.Message, LogType.Fail);
            }
            return result;
        }

        #endregion

        private void OrderGrid_UpdateRowColor()
        {
            Color RunningColor = Color.SpringGreen;
            Color BufferColor = Color.Yellow;
            Color NormalColor;
            Color ErrorColor = Color.Red;

            Color DefaultFontColor;
            Color ActiveFontColor = Color.Black;

            NormalColor = (Properties.Settings.Default.UI_DarkMode == false ? Color.White : Color.Black);//Top@
            //ActiveFontColor = (displaymode == false ? Color.Black : Color.White);//Top@
            DefaultFontColor = (Properties.Settings.Default.UI_DarkMode == false ? Color.Black : Color.LightGray);//Top@
            dgvOrderList.ColumnHeadersDefaultCellStyle.BackColor = (Properties.Settings.Default.UI_DarkMode == false ? Color.SteelBlue : Color.Black);//Top@


            foreach (DataGridViewRow r in dgvOrderList.Rows)
            {
                string OrderState = r.Cells["OrderState"].Value.ToString();
                switch (OrderState)
                {
                    case "9":
                        {
                            r.DefaultCellStyle.BackColor = ErrorColor;
                            r.DefaultCellStyle.ForeColor = ActiveFontColor;
                            break;
                        }
                    case "2":
                        {
                            r.DefaultCellStyle.BackColor = RunningColor;
                            r.DefaultCellStyle.ForeColor = ActiveFontColor;
                            break;
                        }
                    case "1":
                        {
                            r.DefaultCellStyle.BackColor = BufferColor;
                            r.DefaultCellStyle.ForeColor = ActiveFontColor;
                            break;
                        }
                    default:
                        {
                            r.DefaultCellStyle.BackColor = NormalColor;
                            r.DefaultCellStyle.ForeColor = DefaultFontColor;
                            break;
                        }
                }
            }
        }
        private void OrderGrid_RemoveFinishOrder(string OrderID)
        {
            try
            {
                if (dgvOrderList == null || dgvOrderList.Rows == null || dgvOrderList.Rows.Count == 0)
                    return;

                DataGridViewRow row = dgvOrderList.Rows
                                .Cast<DataGridViewRow>()
                                .Where(r => r.Cells["ID"].Value.ToString().Equals(OrderID)).First();
                if (row != null)
                {
                    dgvOrderList.Rows.Remove(row);
                }

                dgvOrderList.Refresh();
            }
            catch (Exception e)
            {
                if (e.Message != "Sequence contains no elements")
                    Log.WriteLog("Error! - Cannot remove completed Order in the Grid." + e.Message, LogType.Fail);
            }
        }
        private void OrderGrid_DisableSort()
        {
            foreach (DataGridViewColumn column in dgvOrderList.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void dgvOrderList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrderList.SelectedRows != null && dgvOrderList.SelectedRows.Count == 1)
            {
                DataGridViewRow buf_SelectedRow = dgvOrderList.SelectedRows[0];
                Get_OrderData(buf_SelectedRow);
            }
        }
        private int Get_OrderState(DataGridViewRow buf_SelectedRow)
        {
            if (buf_SelectedRow == null)
                return -1;
            return Convert.ToInt16(buf_SelectedRow.Cells["OrderState"].Value);
        }
        private void Get_OrderData(DataGridViewRow buf_SelectedRow)
        {
            int buf_OrderState = Get_OrderState(buf_SelectedRow);
            Selected_ptm = GetPatternOrder(buf_SelectedRow, pnlSimRun, true);
            Selected_odm = GetOrder(buf_SelectedRow);
            if (buf_OrderState == (int)OrderState.Normal)
            {
                lblRobotSpeed_Title.Text = "Speed";
                lblSim3DMode.Text = "Selected Order";
                lblSim3DMode.ForeColor = Color.Tomato;
                grpbxFeedData.Text = "Selected Order";
                grpbxFeedData.ForeColor = Color.Tomato;
                lblDispSO.Text = "SO Amount";
                lblDispLYCount.Text = "Layer/Pallet";

                Selected_odm.TieSheetWidth = ClientData.Selected_TSWidth;
                Selected_odm.TieSheetLength = ClientData.Selected_TSLength;
            }
            else if (buf_OrderState == (int)OrderState.Buffer)
            {
                lblRobotSpeed_Title.Text = "Speed";
                lblSim3DMode.Text = "Uploaded Order";
                lblSim3DMode.ForeColor = Color.Black;
                grpbxFeedData.Text = "Uploaded Order";
                grpbxFeedData.ForeColor = Color.Black;
                lblDispSO.Text = "SO Amount";
                lblDispLYCount.Text = "Layer/Pallet";

                Selected_odm.TieSheetWidth = Latest_FeedData.TieSheet_Width;
                Selected_odm.TieSheetLength = Latest_FeedData.TieSheet_Length;
            }
            else if (buf_OrderState == (int)OrderState.Using)
            {
                lblRobotSpeed_Title.Text = chbRealTime.Checked ? "Actual Speed" : "Speed";
                lblSim3DMode.Text = "Running Order";
                lblSim3DMode.ForeColor = Color.Black;
                grpbxFeedData.Text = "Running Order";
                grpbxFeedData.ForeColor = Color.Black;
                lblDispSO.Text = "#SO";
                lblDispLYCount.Text = "Current Layer";

                Selected_odm.TieSheetWidth = Latest_FeedData.TieSheet_Width;
                Selected_odm.TieSheetLength = Latest_FeedData.TieSheet_Length;
            }

            UpdateFeed_Static(Selected_odm, Selected_ptm);
            UpdateFeed_Dynamic(grpbxFeedData.Text, chbRealTime.Checked, Selected_odm, Selected_ptm, Latest_FeedData);
        }

        private void UpdateFeed_Static(OrderModel buf_odm, PatternModel buf_ptm)
        {
            string buf_DispTSLY = "";
            string buf_DispTSLY1 = "";
            string buf_DispTSLY2 = "";
            string buf_DispTSLY3 = "";
            //int testttt = buf_odm.ArrTieSheet.IndexOf("[0");
            if (buf_odm.ArrTieSheet.IndexOf("[0") != -1)
            {
            }
            else
            {
                buf_DispTSLY = buf_odm.ArrTieSheet.Remove(0, 1);
                buf_DispTSLY = buf_DispTSLY.Remove(buf_DispTSLY.Length - 1, 1);
                int buf_1stZero = buf_DispTSLY.IndexOf(",0,");
                if (buf_1stZero > 0)
                    buf_DispTSLY = buf_DispTSLY.Substring(0, buf_1stZero);

                if (buf_DispTSLY.Length > 30)
                {
                    if (buf_DispTSLY[29] == ',')
                    {
                        buf_DispTSLY1 = buf_DispTSLY.Substring(0, 29);
                        buf_DispTSLY2 = buf_DispTSLY.Substring(30);

                    }
                    else
                    {
                        if (buf_DispTSLY[28] == ',')
                        {
                            buf_DispTSLY1 = buf_DispTSLY.Substring(0, 28);
                            buf_DispTSLY2 = buf_DispTSLY.Substring(29);
                        }
                        else if (buf_DispTSLY[27] == ',')
                        {
                            buf_DispTSLY1 = buf_DispTSLY.Substring(0, 27);
                            buf_DispTSLY2 = buf_DispTSLY.Substring(28);
                        }
                    }

                    if (buf_DispTSLY2.Length > 30)
                    {
                        if (buf_DispTSLY2[29] == ',')
                        {
                            buf_DispTSLY3 = buf_DispTSLY2.Substring(30);

                        }
                        else
                        {
                            if (buf_DispTSLY2[28] == ',')
                            {
                                buf_DispTSLY3 = buf_DispTSLY2.Substring(29);
                            }
                            else if (buf_DispTSLY2[27] == ',')
                            {
                                buf_DispTSLY3 = buf_DispTSLY2.Substring(28);
                            }
                        }
                    }
                }
                else
                {
                    buf_DispTSLY1 = buf_DispTSLY;
                }
            }
            txbTSLY1_10.Enabled = buf_DispTSLY1 == "";
            txbTSLY11_20.Enabled = buf_DispTSLY2 == "";
            txbTSLY21_30.Enabled = buf_DispTSLY3 == "";

            //Finger
            if (!lblFingerRequire.InvokeRequired)
            {
                lblFingerRequire.Text = (buf_odm.FingerRequired ? "ON" : "OFF");
                lblFingerRequire.ForeColor = (buf_odm.FingerRequired ? Color.SpringGreen : Color.Red);
            }
            else
                lblFingerRequire.Invoke(new Action(() =>
                {
                    lblFingerRequire.Text = (buf_odm.FingerRequired ? "ON" : "OFF");
                    lblFingerRequire.ForeColor = (buf_odm.FingerRequired ? Color.SpringGreen : Color.Red);
                }));



            //PalletSide
            lblPalletType.Text = buf_odm.PalletType;
            lblSizePallet.Text = buf_odm.PalletWidth + "x" + buf_odm.PalletLength + "x" + buf_odm.PalletHeight;

            //Material No
            txbDispMatNo.Text = buf_odm.MaterialNo;

            //Bundle Per Grip
            txbDispBDPerGrip.Text = buf_odm.BDPerGrip.ToString();

            // TieSheet Layer 1 - 10
            if (!txbTSLY1_10.InvokeRequired)
                txbTSLY1_10.Text = buf_DispTSLY1;
            else
                txbTSLY1_10.Invoke(new Action(() =>
                {
                    txbTSLY1_10.Text = buf_DispTSLY1;
                }));

            // TieSheet Layer 11 - 20
            if (!txbTSLY11_20.InvokeRequired)
                txbTSLY11_20.Text = buf_DispTSLY2;
            else
                txbTSLY11_20.Invoke(new Action(() =>
                {
                    txbTSLY11_20.Text = buf_DispTSLY2;
                }));

            // TieSheet Layer 21 - 30
            if (!txbTSLY21_30.InvokeRequired)
                txbTSLY21_30.Text = buf_DispTSLY3;
            else
                txbTSLY21_30.Invoke(new Action(() =>
                {
                    txbTSLY21_30.Text = buf_DispTSLY3;
                }));

            //Pattern Sequence
            chkbxDispPat1.Checked = buf_ptm.SwitchPattern[2] == '1';
            chkbxDispPat2.Checked = buf_ptm.SwitchPattern[4] == '1';
            chkbxDispPat3.Checked = buf_ptm.SwitchPattern[6] == '1';
            chkbxDispPat4.Checked = buf_ptm.SwitchPattern[8] == '1';
        }

        private void UpdateFeed_Dynamic(string SimMode, bool RealTime, OrderModel buf_odm, PatternModel buf_ptm, UIFeedDataModel LatestFeed)
        {
            // LatestFeed
            // Will be updated for the 1st time when ID4P Side's Send New Data is completed.
            //      we update it here even if it is not running yet because we want to store uploaded TS Size
            //      if we use ClientData.SelectedTS to show, it might be possible that the user change TSOption after Send New Data
            //      which we not effect the Sent Data.
            // Then it will be updated when tmrRobotFeed is ticked.

            int Speed = 0;
            int BD_Amount = 0;
            int SO_Amount = buf_odm.Arr_SOID.Length;
            //int SOBD_Amount[] = 0;
            int LY_Amount = buf_odm.LayerPerPallet;
            int LY_Count = LatestFeed.Layer_Count;
            int BD_Count = LatestFeed.OrderBundle_Count;
            int SO_Count = SimMode == "Current Order" ? 0 : 1;
            int SOBD_Count = LatestFeed.SOBD_Count;
            int SOBD_Amount = buf_odm.Arr_SheetPerSO[SO_Count < 1 ? 0 : SO_Count - 1];
            int Pat_Count = LatestFeed.Pattern_Count;
            int Pallet_Count = LatestFeed.Pallet_Count;
            int TieSheet_Width = SimMode == "Selected Order" ? ClientData.Selected_TSWidth : LatestFeed.TieSheet_Width;
            int TieSheet_Length = SimMode == "Selected Order" ? ClientData.Selected_TSWidth : LatestFeed.TieSheet_Width;
            int PGB_Max = buf_odm.BundlePerLayer * buf_odm.LayerPerPallet;
            int PGB_Value = RealTime ? LatestFeed.PalletBD_Count[LatestFeed.Pallet_Count - 1] : 0;
            string Ordinal_Suffix = Pallet_Count == 1 ? "st" : Pallet_Count == 2 ? "nd" : Pallet_Count == 3 ? "rd" : "th";
            string PalletPercent_Text = "";
            if (RealTime)
                PalletPercent_Text = string.Format("{0}" + Ordinal_Suffix + " Pallet: {1} % ( {2} / {3} )", Pallet_Count, PGB_Value * 100 / PGB_Max, PGB_Value, PGB_Max);
            else
                PalletPercent_Text = string.Format("{0}" + Ordinal_Suffix + " Pallet", Pallet_Count);

            // Robot Speed
            if (!lblSpeed.InvokeRequired)
                lblSpeed.Text = Speed.ToString();
            else
                lblSpeed.Invoke(new Action(() =>
                {
                    lblSpeed.Text = Speed.ToString();
                }));

            // Bundle Count
            if (!txbDispBDCount.InvokeRequired)
                txbDispBDCount.Text = SimMode == "Running Order" ? BD_Count.ToString() : BD_Amount.ToString();
            else
                txbDispBDCount.Invoke(new Action(() =>
                {
                    txbDispBDCount.Text = SimMode == "Running Order" ? BD_Count.ToString() : BD_Amount.ToString();
                }));


            // SO Index
            if (!txbDispSO.InvokeRequired)
                txbDispSO.Text = SO_Count.ToString() + " / " + SO_Amount.ToString();
            else
                txbDispSO.Invoke(new Action(() =>
                {
                    txbDispSO.Text = SO_Count.ToString() + " / " + SO_Amount.ToString();
                }));

            // SO Bundle
            if (!txbDispSOBD.InvokeRequired)
                txbDispSOBD.Text = SimMode == "Current Order" ? SOBD_Count.ToString() + " / " + SOBD_Amount.ToString() : SOBD_Amount.ToString();
            else
                txbDispSOBD.Invoke(new Action(() =>
                {
                    txbDispSOBD.Text = SimMode == "Current Order" ? SOBD_Count.ToString() + " / " + SOBD_Amount.ToString() : SOBD_Amount.ToString();
                }));

            // Layer Count
            if (!txbDispLYCount.InvokeRequired)
                txbDispLYCount.Text = SimMode == "Running Order" ? (LY_Count.ToString() + " / " + LY_Amount.ToString()) : LY_Amount.ToString();
            else
                txbDispLYCount.Invoke(new Action(() =>
                {
                    txbDispLYCount.Text = SimMode == "Running Order" ? (LY_Count.ToString() + " / " + LY_Amount.ToString()) : LY_Amount.ToString();
                }));

            // TieSheet Width Size
            if (!txbDispTSW.InvokeRequired)
                txbDispTSW.Text = TieSheet_Width.ToString();
            else
                txbDispTSW.Invoke(new Action(() =>
                {
                    txbDispTSW.Text = TieSheet_Width.ToString();
                }));

            // TieSheet Length Size
            if (!txbDispTSL.InvokeRequired)
                txbDispTSL.Text = TieSheet_Length.ToString();
            else
                txbDispTSL.Invoke(new Action(() =>
                {
                    txbDispTSL.Text = TieSheet_Length.ToString();
                }));

            // Step Count
            if (!chkbxDispPat1.InvokeRequired)
            {
                chkbxDispPat1.ForeColor = Pat_Count == 1 ? Color.Yellow : Color.White;
            }
            else
                chkbxDispPat1.Invoke(new Action(() =>
                {
                    chkbxDispPat1.ForeColor = Pat_Count == 1 ? Color.Yellow : Color.White;
                }));


            if (!chkbxDispPat2.InvokeRequired)
            {
                chkbxDispPat2.ForeColor = Pat_Count == 2 ? Color.Yellow : Color.White;
            }
            else
                chkbxDispPat2.Invoke(new Action(() =>
                {
                    chkbxDispPat2.ForeColor = Pat_Count == 2 ? Color.Yellow : Color.White;
                }));

            if (!chkbxDispPat3.InvokeRequired)
            {
                chkbxDispPat3.ForeColor = Pat_Count == 3 ? Color.Yellow : Color.White;
            }
            else
                chkbxDispPat3.Invoke(new Action(() =>
                {
                    chkbxDispPat3.ForeColor = Pat_Count == 3 ? Color.Yellow : Color.White;
                }));

            if (!chkbxDispPat4.InvokeRequired)
            {
                chkbxDispPat4.ForeColor = Pat_Count == 4 ? Color.Yellow : Color.White;
            }
            else
                chkbxDispPat4.Invoke(new Action(() =>
                {
                    chkbxDispPat4.ForeColor = Pat_Count == 4 ? Color.Yellow : Color.White;
                }));
            /*
                        // 3D Picture
                        if (Running_ptm.Pattern != null && OrderGrid_FindOrderByState(OrderState.Using, false) != null)
                        {
                            int LastPalletNo = Convert.ToInt32(lblPalletPercent.Tag);
                            if (!ptb3DOrder.InvokeRequired)
                                Simulation3D(running_Order3DBD, Running_ptm, PlacedCurrentPallet, (PalletNo != LastPalletNo ? true : false));
                            else
                                ptb3DOrder.Invoke(new Action(() => { Simulation3D(running_Order3DBD, Running_ptm, PlacedCurrentPallet, (PalletNo != LastPalletNo ? true : false)); }));
                        }*/

            // Progress bar percent text
            if (!lblPalletPercent.InvokeRequired)
            {
                lblPalletPercent.Text = PalletPercent_Text;
                lblPalletPercent.Tag = 0;
            }
            else
                lblPalletPercent.Invoke(new Action(() =>
                {
                    lblPalletPercent.Text = PalletPercent_Text;
                    lblPalletPercent.Tag = 0;
                }));

            // Progress Bar under 3D Picture
            if (!pgbPlacedBundle.InvokeRequired)
            {
                pgbPlacedBundle.Maximum = PGB_Max;
                pgbPlacedBundle.Value = PGB_Value;
            }
            else
                pgbPlacedBundle.Invoke(new Action(() =>
                {
                    pgbPlacedBundle.Maximum = PGB_Max;
                    pgbPlacedBundle.Value = PGB_Value;
                }));


        }

        private PatternModel GetPatternOrder(DataGridViewRow rOrder, Panel pnl = null, bool Show3D = false)
        {
            PatternModel ptm = new PatternModel();
            try
            {
                if (rOrder != null)
                {
                    //--Simulation--
                    ptm.Pattern = rOrder.Cells["Pattern_Code"].Value.ToString();
                    ptm.UpdatePatternDetail();
                    bool buf_SwitchBDSize = Convert.ToBoolean(rOrder.Cells["SwitchBDSize"].Value.ToString());
                    ptm.SwithBDSize = buf_SwitchBDSize;
                    //ptm.BundleWidth = (ptm.SwithBDSize == false ? Convert.ToInt32(rOrder.Cells["SheetWidth"].Value.ToString()) : Convert.ToInt32(rOrder.Cells["SheetLength"].Value.ToString()));
                    //ptm.BundleLength = (ptm.SwithBDSize == false ? Convert.ToInt32(rOrder.Cells["SheetLength"].Value.ToString()) : Convert.ToInt32(rOrder.Cells["SheetWidth"].Value.ToString()));
                    ptm.BundleWidth = (!ptm.SwithBDSize ? Convert.ToInt32(rOrder.Cells["SheetWidth"].Value.ToString()) : Convert.ToInt32(rOrder.Cells["SheetLength"].Value.ToString())); // Mod condition ver3.0 2023-05-05
                    ptm.BundleLength = (!ptm.SwithBDSize ? Convert.ToInt32(rOrder.Cells["SheetLength"].Value.ToString()) : Convert.ToInt32(rOrder.Cells["SheetWidth"].Value.ToString())); // Mod condition ver3.0 2023-05-05
                    ptm.PalletWidth = Convert.ToInt32(rOrder.Cells["PalletWidth"].Value.ToString());
                    ptm.PalletLength = Convert.ToInt32(rOrder.Cells["PalletLength"].Value.ToString());
                    bool buf_GripperFinger = Convert.ToBoolean(rOrder.Cells["GripperFinger"].Value.ToString());
                    ptm.BundleGap = buf_GripperFinger ? Properties.Settings.Default.BDGap_wFinger : Properties.Settings.Default.BDGap_woFinger;
                    bool buf_FixBundleFace = Convert.ToBoolean(rOrder.Cells["FixBDFace"].Value.ToString());
                    ptm.FixBundleFace = buf_FixBundleFace;
                    Int32 buf_RotatePattern = -1;
                    Int32.TryParse(rOrder.Cells["RotatePattern"].Value.ToString(), out buf_RotatePattern);
                    ptm.RotatePattern = buf_RotatePattern == -1 ? false : true; // *** change source variable to boolean ver3.0 2023-05-09
                    bool buf_SpecialRotate = Convert.ToBoolean(rOrder.Cells["SpecialRotate"].Value.ToString());
                    ptm.SpecialRotate = buf_SpecialRotate;
                    ptm.GetPlacementInfo();
                    ptm.SwitchPattern = rOrder.Cells["PatternSeq"].Value.ToString();
                    ptm.SQ_XaxisValue = Convert.ToInt32(rOrder.Cells["SQ_OpenX"].Value);
                    ptm.SQ_YaxisValue = Convert.ToInt32(rOrder.Cells["SQ_OpenY"].Value);
                    ptm.SQ_XaxisValueWithOutGap = Convert.ToInt32(rOrder.Cells["SQ_CloseX"].Value);
                    ptm.SQ_YaxisValueWithOutGap = Convert.ToInt32(rOrder.Cells["SQ_CloseY"].Value);

                    #region Simulation & UIs
                    if (pnl != null)
                    {
                        int buf_Layer = 1;
                        if (ptm.SwitchPattern[0] == '1')
                        {
                            if (ptm.SwitchPattern[2] == '1')
                                buf_Layer = 1;
                            else
                                if (ptm.SwitchPattern[4] == '1')
                                buf_Layer = 2;
                            else
                                    if (ptm.SwitchPattern[6] == '1')
                                buf_Layer = 3;
                            else
                                        if (ptm.SwitchPattern[8] == '1')
                                buf_Layer = 4;
                        }
                        SimulatePattern(pnl, ptm, rdoPressSquaring.Checked, lblUDHAlarm, buf_Layer);
                    }

                    int buf_LayerPerPallet = Convert.ToInt32(rOrder.Cells["LayerPerPallet"].Value.ToString());
                    bool buf_FingerRequired = Convert.ToBoolean(rOrder.Cells["GripperFinger"].Value.ToString());
                    string buf_PalletType = rOrder.Cells["PalletType"].Value.ToString();
                    string buf_PalletWidth = rOrder.Cells["PalletWidth"].Value.ToString();
                    string buf_PalletLength = rOrder.Cells["PalletLength"].Value.ToString();
                    string buf_PalletHeight = rOrder.Cells["PalletHeight"].Value.ToString();


                    if (Show3D != false)
                    {
                        Bundle3DCls BD3D = new Bundle3DCls();
                        Simulation3D(BD3D, ptm, (ptm.BundlePerLayer * buf_LayerPerPallet), true);
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - " + e.Message, LogType.Fail);
            }

            return ptm;
        }
        private OrderModel GetOrder(DataGridViewRow rOrder)
        {
            OrderModel loc_odm = new OrderModel();

            try
            {
                loc_odm.MaterialNo = rOrder.Cells["MaterialNo"].Value.ToString();
                loc_odm.BDPerGrip = Convert.ToInt32(rOrder.Cells["BundlePerGrip"].Value.ToString());
                loc_odm.BundleWidth = Convert.ToDouble(rOrder.Cells["SheetWidth"].Value.ToString());
                loc_odm.BundleLength = Convert.ToDouble(rOrder.Cells["SheetLength"].Value.ToString());
                loc_odm.BundleHeight = Convert.ToDouble(rOrder.Cells["SheetHeight"].Value.ToString());
                loc_odm.HeightRatio = Convert.ToInt32(rOrder.Cells["HeightRatio"].Value.ToString());
                loc_odm.BundleWeight = Convert.ToDouble(rOrder.Cells["SheetWeight"].Value.ToString());
                //odm.SideGuide = 0; //Not found
                loc_odm.TopSheet = Convert.ToBoolean(rOrder.Cells["TopSheet"].Value.ToString());
                loc_odm.BottomSheet = Convert.ToInt32(rOrder.Cells["BtmSheet"].Value.ToString());
                //Convert.ToBoolean(rOrder.Cells["BtmSheet"].Value.ToString());
                loc_odm.LayerPerPallet = Convert.ToInt32(rOrder.Cells["LayerPerPallet"].Value.ToString());
                loc_odm.BundlePerLayer = Convert.ToInt32(rOrder.Cells["BundlePerLayer"].Value.ToString());
                loc_odm.SheetPerBundle = Convert.ToInt32(rOrder.Cells["SheetPerBundle"].Value.ToString());
                loc_odm.BundleHeight = Math.Round(loc_odm.BundleHeight * loc_odm.SheetPerBundle, 0); // BundleHeight was assign a value of rOrder's Sheet Height
                loc_odm.BundleWeight = Math.Round(loc_odm.BundleWeight * loc_odm.SheetPerBundle, 2); // BundleWeight was assign a value of rOrder's Sheet Weight

                //pallet size @Top 13-4-2020       
                loc_odm.PalletWidth = Convert.ToInt32(rOrder.Cells["PalletWidth"].Value.ToString());
                loc_odm.PalletLength = Convert.ToInt32(rOrder.Cells["PalletLength"].Value.ToString());
                loc_odm.PalletHeight = Convert.ToInt32(rOrder.Cells["PalletHeight"].Value.ToString());
                loc_odm.PalletType = rOrder.Cells["PalletType"].Value.ToString();

                //Tie Sheet Special
                bool buf_TSB4LastLY = false;
                bool.TryParse(rOrder.Cells["TS_B4LastLY"].Value.ToString(), out buf_TSB4LastLY);
                loc_odm.TS_B4LastLY = buf_TSB4LastLY;
                bool buf_TSLOWSQLayer = false;
                bool.TryParse(rOrder.Cells["TS_SQLayer"].Value.ToString(), out buf_TSLOWSQLayer);
                loc_odm.TS_LowSQLayer = buf_TSLOWSQLayer;
                //Tie Sheet
                loc_odm.TSEveryXLY = Convert.ToInt32(rOrder.Cells["TieSheet"].Value.ToString());
                loc_odm.GetArrTieSheet();

                //Squaring
                bool buf_Squaring = false;
                bool.TryParse(rOrder.Cells["Squaring"].Value.ToString(), out buf_Squaring);
                loc_odm.Squaring = buf_Squaring;
                bool buf_FingerUse = false;
                bool.TryParse(rOrder.Cells["GripperFinger"].Value.ToString(), out buf_FingerUse);
                loc_odm.FingerRequired = buf_FingerUse;

                //Layer Perimeter for ExtSQ
                int buf_Peri_CloseL = 0;
                int.TryParse(rOrder.Cells["Peri_CloseL"].Value.ToString(), out buf_Peri_CloseL);
                int buf_Peri_CloseW = 0;
                int.TryParse(rOrder.Cells["Peri_CloseW"].Value.ToString(), out buf_Peri_CloseW);
                loc_odm.Peri_CloseL = buf_Peri_CloseL;
                loc_odm.Peri_CloseW = buf_Peri_CloseW;

                //SwapSide
                bool buf_SwitchBDSize = false;
                bool.TryParse(rOrder.Cells["SwitchBDSize"].Value.ToString(), out buf_SwitchBDSize);
                loc_odm.SwitchBDSize = buf_SwitchBDSize;

                //SpliSO
                loc_odm.SplitSOText = rOrder.Cells["SplitSheet"].Value.ToString();
                loc_odm.SOSplit = rOrder.Cells["SOSplit"].Value.ToString();
                string[] buf_SOID = loc_odm.SOSplit.Split(',');
                string[] buf_SheetPerSO = loc_odm.SplitSOText.Split(',');
                loc_odm.Arr_SOID = new int[buf_SOID.Length];
                loc_odm.Arr_SheetPerSO = new int[buf_SheetPerSO.Length];
                for (int i = 0; i < buf_SOID.Length; i++)
                {
                    if (!String.IsNullOrEmpty(buf_SOID[0]) && !String.IsNullOrEmpty(buf_SheetPerSO[0]))
                    {
                        loc_odm.Arr_SheetPerSO[i] = Convert.ToInt32(buf_SheetPerSO[i]);
                        loc_odm.Arr_SOID[i] = Convert.ToInt32(buf_SOID[i]);
                    }
                    else
                    {
                        loc_odm.Arr_SheetPerSO[i] = 0;
                        loc_odm.Arr_SOID[i] = 0;
                    }
                }


                //OrderItem
                loc_odm.OrderItem = rOrder.Cells["MO"].Value.ToString();

                //ProductCode
                loc_odm.ProductCode = rOrder.Cells["ProductCode"].Value.ToString();


                //Special NoLiftStack & AntiBounce
                bool buf_STKLiftBD = false;
                bool.TryParse(rOrder.Cells["StackerLiftBD"].Value.ToString(), out buf_STKLiftBD);
                loc_odm.STKLiftBD = buf_STKLiftBD;
                bool buf_STKAntiBounce = false;
                bool.TryParse(rOrder.Cells["StackerAntiBounce"].Value.ToString(), out buf_STKAntiBounce);
                loc_odm.STKAntiBounce = buf_STKAntiBounce;

                //ExtraSQ
                bool buf_ExtraSQ = false;
                bool.TryParse(rOrder.Cells["ExtraSQ"].Value.ToString(), out buf_ExtraSQ);
                loc_odm.ExtraSQ = buf_ExtraSQ;

                //Placing Mode
                int buf_PlacingMode = 0;
                int.TryParse(rOrder.Cells["PlacingMode"].Value.ToString(), out buf_PlacingMode);
                loc_odm.PlacingMode = buf_PlacingMode;

                //Discharge Mode
                int buf_DischargeMode = 0;
                int.TryParse(rOrder.Cells["DischargeMode"].Value.ToString(), out buf_DischargeMode);
                loc_odm.DischargeMode = buf_DischargeMode;

                //Extra Pick Depth
                bool buf_ExtraPickDepth = false;
                bool.TryParse(rOrder.Cells["ExtraPickDepth"].Value.ToString(), out buf_ExtraPickDepth);
                loc_odm.ExtraPickDepth = buf_ExtraPickDepth;
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - " + e.Message, LogType.Fail);
            }

            return loc_odm;
        }

        private Laying[] GetPlacementfromDB(string MaterialNo)
        {
            Laying[] result;
            string Query = string.Format("SELECT * FROM [tblPlacement] where [Material_No] = '{0}' ORDER BY Step", MaterialNo);
            DataTable dtPlacement = Sql.GetDataTableFromSql(Query);
            if (dtPlacement == null || dtPlacement.Rows.Count == 0)
                result = null;
            else
            {
                result = new Laying[dtPlacement.Rows.Count];
                int Index = 0;
                foreach (DataRow r in dtPlacement.Rows)
                {
                    result[Index].PatternName = r["PatternName"].ToString();
                    result[Index].Step = Convert.ToInt32(r["Step"]);
                    //result[Index].Row = Convert.ToInt32(r["Row"]);
                    result[Index].Row = 0;
                    result[Index].X = Convert.ToInt32(r["X"]);
                    result[Index].Y = Convert.ToInt32(r["Y"]);
                    result[Index].Ori = Convert.ToInt32(r["Degree"]);
                    result[Index].Layer = Convert.ToInt32(r["Layer"]);
                    Index++;
                }
            }


            return result;
        }

        #region Simulation

        public void SimulatePattern(Panel pnl, PatternModel ptn, bool IsPressMode, Label lblAlarm, int Layer = 1)
        {
            pnl.Refresh();
            float DisplayScale = (float)pnl.Size.Height / (float)2000;
            bool[,] SQ_UDH = { {
                    (ptn.SQ_XaxisValueWithOutGap > Properties.Settings.Default.SQ_Max_X) ,
                    (ptn.SQ_YaxisValueWithOutGap > Properties.Settings.Default.SQ_Max_Y) },
                    {
                    (ptn.SQ_XaxisValue - 30 > Properties.Settings.Default.SQ_Max_X) ,
                    (ptn.SQ_YaxisValue - 30 > Properties.Settings.Default.SQ_Max_Y) } };
            if (SQ_UDH[0, 0] || SQ_UDH[0, 1])
            {
                lblAlarm.Text = "Perimeter is too small.\nthe Squarings can't reach.";
            }
            else
            {
                lblAlarm.Text = "";
            }
            DrawPallet(pnl, ptn, DisplayScale);
            DrawSquaring(pnl, ptn, DisplayScale, SQ_UDH, IsPressMode);
            DrawBundle(pnl, ptn, true, DisplayScale, Layer, IsPressMode);
        }
        private void DrawSquaring(Panel pnl, PatternModel ptn, float loc_DisplayScale, bool[,] SQ_UDH, bool IsPressMode = false)
        {
            // Initialize Variables
            int SQ_Y;
            int SQ_X;
            int SQ_Max_PeriX = ptn.SQ_Max_PeriX;
            int SQ_Max_PeriY = ptn.SQ_Max_PeriY;
            int SQ_Max_DistX = Properties.Settings.Default.SQ_Max_X;
            int SQ_Max_DistY = Properties.Settings.Default.SQ_Max_Y;
            int SQ_FrameWidth = 50;
            int SQ_ArmLength = 1200 + SQ_FrameWidth;
            int SQ_OpenOffset = 30;

            PictureBox SQ_LeftBox;
            PictureBox SQ_RightBox;
            PictureBox SQ_BottomBox;
            PictureBox SQ_TopBox;

            // Set Squaring Position
            if (IsPressMode)
            {
                SQ_Y = SQ_UDH[0, 1] ? SQ_Max_DistY : ptn.SQ_YaxisValueWithOutGap;
                SQ_X = SQ_UDH[0, 0] ? SQ_Max_DistX : ptn.SQ_XaxisValueWithOutGap;
            }
            else
            {
                SQ_Y = SQ_UDH[1, 1] ? SQ_Max_DistY : ptn.SQ_YaxisValue - SQ_OpenOffset;
                SQ_X = SQ_UDH[1, 0] ? SQ_Max_DistX : ptn.SQ_XaxisValue - SQ_OpenOffset;
            }

            // Craete Left Squaring Arm
            var _SQ_LeftBox = FindControlInPanel(pnl, "SQ_LeftBox");
            if (_SQ_LeftBox == null)
            {
                SQ_LeftBox = new PictureBox();
                SQ_LeftBox.Name = "SQ_LeftBox";
                SQ_LeftBox.BorderStyle = BorderStyle.FixedSingle;
                SQ_LeftBox.BackColor = System.Drawing.Color.LightSteelBlue;
                SQ_LeftBox.Width = ScalingDisplay(SQ_FrameWidth, loc_DisplayScale);
                SQ_LeftBox.Height = ScalingDisplay(SQ_ArmLength, loc_DisplayScale);
                SQ_LeftBox.Left = pnl.Size.Width / 2 + ScalingDisplay(-(SQ_Max_PeriY / 2) + SQ_Y - SQ_FrameWidth, loc_DisplayScale);
                SQ_LeftBox.Top = pnl.Size.Height / 2 + ScalingDisplay(-(SQ_Max_PeriX / 2) + SQ_X - SQ_FrameWidth, loc_DisplayScale);
                pnl.Controls.Add(SQ_LeftBox);
            }
            else
            {
                SQ_LeftBox = (PictureBox)_SQ_LeftBox;
                SQ_LeftBox.Left = pnl.Size.Width / 2 + ScalingDisplay(-(SQ_Max_PeriY / 2) + SQ_Y - SQ_FrameWidth, loc_DisplayScale);
                SQ_LeftBox.Top = pnl.Size.Height / 2 + ScalingDisplay(-(SQ_Max_PeriX / 2) + SQ_X - SQ_FrameWidth, loc_DisplayScale);
            }
            SQ_LeftBox.Visible = true;
            SQ_LeftBox.BringToFront();

            // Craete Right Squaring Arm
            var _SQ_RightBox = FindControlInPanel(pnl, "SQ_RightBox");
            if (_SQ_RightBox == null)
            {
                SQ_RightBox = new PictureBox();
                SQ_RightBox.Name = "SQ_RightBox";
                SQ_RightBox.BorderStyle = BorderStyle.FixedSingle;
                SQ_RightBox.BackColor = System.Drawing.Color.LightSteelBlue;
                SQ_RightBox.Width = ScalingDisplay(SQ_FrameWidth, loc_DisplayScale);
                SQ_RightBox.Height = ScalingDisplay(SQ_ArmLength, loc_DisplayScale);
                SQ_RightBox.Left = pnl.Size.Width / 2 + ScalingDisplay((SQ_Max_PeriY / 2) - SQ_Y, loc_DisplayScale);
                SQ_RightBox.Top = pnl.Size.Height / 2 + ScalingDisplay((SQ_Max_PeriX / 2) - SQ_X - SQ_ArmLength + SQ_FrameWidth, loc_DisplayScale);
                pnl.Controls.Add(SQ_RightBox);
            }
            else
            {
                SQ_RightBox = (PictureBox)_SQ_RightBox;
                SQ_RightBox.Left = pnl.Size.Width / 2 + ScalingDisplay((SQ_Max_PeriY / 2) - SQ_Y, loc_DisplayScale);
                SQ_RightBox.Top = pnl.Size.Height / 2 + ScalingDisplay((SQ_Max_PeriX / 2) - SQ_X - SQ_ArmLength + SQ_FrameWidth, loc_DisplayScale);
            }
            SQ_RightBox.Visible = true;
            SQ_RightBox.BringToFront();

            // Craete Bottom Squaring Arm
            var _SQ_BottomBox = FindControlInPanel(pnl, "SQ_BottomBox");
            if (_SQ_BottomBox == null)
            {
                SQ_BottomBox = new PictureBox();
                SQ_BottomBox.Name = "SQ_BottomBox";
                SQ_BottomBox.BorderStyle = BorderStyle.FixedSingle;
                SQ_BottomBox.BackColor = System.Drawing.Color.LightSteelBlue;
                SQ_BottomBox.Width = ScalingDisplay(SQ_ArmLength, loc_DisplayScale);
                SQ_BottomBox.Height = ScalingDisplay(SQ_FrameWidth, loc_DisplayScale);
                SQ_BottomBox.Left = pnl.Size.Width / 2 + ScalingDisplay((SQ_Max_PeriY / 2) - SQ_Y + SQ_FrameWidth - SQ_ArmLength, loc_DisplayScale);
                SQ_BottomBox.Top = pnl.Size.Height / 2 + ScalingDisplay((SQ_Max_PeriX / 2) - SQ_X, loc_DisplayScale);
                pnl.Controls.Add(SQ_BottomBox);
            }
            else
            {
                SQ_BottomBox = (PictureBox)_SQ_BottomBox;
                SQ_BottomBox.Left = pnl.Size.Width / 2 + ScalingDisplay((SQ_Max_PeriY / 2) - SQ_Y + SQ_FrameWidth - SQ_ArmLength, loc_DisplayScale);
                SQ_BottomBox.Top = pnl.Size.Height / 2 + ScalingDisplay((SQ_Max_PeriX / 2) - SQ_X, loc_DisplayScale);
            }
            SQ_BottomBox.Visible = true;
            SQ_BottomBox.BringToFront();

            // Craete Top Squaring Arm
            var _SQ_TopBox = FindControlInPanel(pnl, "SQ_TopBox");
            if (_SQ_TopBox == null)
            {
                SQ_TopBox = new PictureBox();
                SQ_TopBox.Name = "SQ_TopBox";
                SQ_TopBox.BorderStyle = BorderStyle.FixedSingle;
                SQ_TopBox.BackColor = System.Drawing.Color.LightSteelBlue;
                SQ_TopBox.Width = ScalingDisplay(SQ_ArmLength, loc_DisplayScale);
                SQ_TopBox.Height = ScalingDisplay(SQ_FrameWidth, loc_DisplayScale);
                SQ_TopBox.Left = pnl.Size.Width / 2 + ScalingDisplay(-(SQ_Max_PeriY / 2) + SQ_Y - SQ_FrameWidth, loc_DisplayScale);
                SQ_TopBox.Top = pnl.Size.Height / 2 + ScalingDisplay(-(SQ_Max_PeriX / 2) + SQ_X - SQ_FrameWidth, loc_DisplayScale);
                pnl.Controls.Add(SQ_TopBox);
            }
            else
            {
                SQ_TopBox = (PictureBox)_SQ_TopBox;
                SQ_TopBox.Left = pnl.Size.Width / 2 + ScalingDisplay(-(SQ_Max_PeriY / 2) + SQ_Y - SQ_FrameWidth, loc_DisplayScale);
                SQ_TopBox.Top = pnl.Size.Height / 2 + ScalingDisplay(-(SQ_Max_PeriX / 2) + SQ_X - SQ_FrameWidth, loc_DisplayScale);
            }
            SQ_TopBox.Visible = true;
            SQ_TopBox.BringToFront();
        }
        private void DrawPallet(Panel pnl, PatternModel ptn, float loc_DisplayScale)
        {
            PictureBox PalletBox;
            var _PalletBox = FindControlInPanel(pnl, "PalletBox");
            int PLW = ptn.PalletWidth;
            int PLL = ptn.PalletLength;
            if (_PalletBox == null)
            {
                PalletBox = new PictureBox();
                PalletBox.Name = "PalletBox";
                PalletBox.SizeMode = PictureBoxSizeMode.StretchImage;
                PalletBox.BorderStyle = BorderStyle.FixedSingle;
                PalletBox.Image = imlMain.Images[0];
                PalletBox.Width = ScalingDisplay(PLW, loc_DisplayScale);
                PalletBox.Height = ScalingDisplay(PLL, loc_DisplayScale);
                PalletBox.Left = pnl.Size.Width / 2 - ScalingDisplay(PLW / 2, loc_DisplayScale);
                PalletBox.Top = pnl.Size.Height / 2 - ScalingDisplay(PLL / 2, loc_DisplayScale);
                PalletBox.Visible = false;
                pnl.Controls.Add(PalletBox);
            }
            else
            {
                PalletBox = (PictureBox)_PalletBox;
                PalletBox.Width = ScalingDisplay(PLW, loc_DisplayScale);
                PalletBox.Height = ScalingDisplay(PLL, loc_DisplayScale);
                PalletBox.Left = pnl.Size.Width / 2 - ScalingDisplay(PLW / 2, loc_DisplayScale);
                PalletBox.Top = pnl.Size.Height / 2 - ScalingDisplay(PLL / 2, loc_DisplayScale);
            }

            PalletBox.Visible = true;
            PalletBox.BringToFront();
        }
        private void DrawBundle(Panel pnl, PatternModel ptn, bool Y_inverted, float loc_DisplayScale, int Layer = 1, bool IsPressMode = false)
        {
            if (ptn == null)
                return;

            PictureBox[] ResultBox = new PictureBox[32];

            int _BW = ptn.BundleWidth;
            int _BL = ptn.BundleLength;
            int RobOrigin_x = pnl.Size.Height / 2 + ScalingDisplay(ptn.PalletLength / 2, loc_DisplayScale);
            int RobOrigin_y = pnl.Size.Width / 2;
            int n;
            int Yinv = 1;
            if (Y_inverted) Yinv = -1;

            //Clear Data
            for (int i = pnl.Controls.Count - 1; i >= 0; i--)
            {
                Control ctrl = pnl.Controls[i];
                if (ctrl is PictureBox PB && PB.Name.Contains("Outbox"))
                    pnl.Controls.Remove(ctrl);
            }

            for (n = 0; n < 32; n++)
            {
                if (ResultBox[n] == null)
                {
                    ResultBox[n] = new PictureBox();
                    ResultBox[n].Name = "Outbox" + n;
                    ResultBox[n].SizeMode = PictureBoxSizeMode.StretchImage;
                    ResultBox[n].BorderStyle = BorderStyle.FixedSingle;
                    ResultBox[n].BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcc66");
                    ResultBox[n].Width = 10;
                    ResultBox[n].Height = 10;
                    ResultBox[n].Left = (n * 10);
                    ResultBox[n].Top = (n * 10);
                    ResultBox[n].Visible = false;
                    pnl.Controls.Add(ResultBox[n]);
                }

                ResultBox[n].Visible = false;
            }

            //Create Temp Data
            Laying[] LayerBundle = ptn.GetPlacementInfo_AtLayer(Layer, ptn.PatternBundle, ptn.PatternBundleWithOutGap, IsPressMode);
            if (LayerBundle == null || LayerBundle.Count() == 0)
                return;

            for (n = 0; n < ptn.BundlePerLayer; n++)
            {
                switch (LayerBundle[n].Ori)
                {
                    case 0:
                        ResultBox[n].Left = ScalingDisplay(LayerBundle[n].Y * Yinv, loc_DisplayScale) + RobOrigin_y;
                        ResultBox[n].Top = ScalingDisplay(-LayerBundle[n].X, loc_DisplayScale) + RobOrigin_x;
                        ResultBox[n].Width = ScalingDisplay(_BL, loc_DisplayScale);
                        ResultBox[n].Height = ScalingDisplay(_BW, loc_DisplayScale);
                        break;
                    case 1:
                        ResultBox[n].Left = ScalingDisplay((LayerBundle[n].Y * Yinv) - _BW, loc_DisplayScale) + RobOrigin_y;
                        ResultBox[n].Top = ScalingDisplay(-LayerBundle[n].X, loc_DisplayScale) + RobOrigin_x;
                        ResultBox[n].Width = ScalingDisplay(_BW, loc_DisplayScale);
                        ResultBox[n].Height = ScalingDisplay(_BL, loc_DisplayScale);
                        break;
                    case 2:
                        ResultBox[n].Left = ScalingDisplay((LayerBundle[n].Y * Yinv) - _BL, loc_DisplayScale) + RobOrigin_y;
                        ResultBox[n].Top = ScalingDisplay(-LayerBundle[n].X - _BW, loc_DisplayScale) + RobOrigin_x;
                        ResultBox[n].Width = ScalingDisplay(_BL, loc_DisplayScale);
                        ResultBox[n].Height = ScalingDisplay(_BW, loc_DisplayScale);
                        break;
                    case 3:
                        ResultBox[n].Left = ScalingDisplay(LayerBundle[n].Y * Yinv, loc_DisplayScale) + RobOrigin_y;
                        ResultBox[n].Top = ScalingDisplay(-LayerBundle[n].X - _BL, loc_DisplayScale) + RobOrigin_x;
                        ResultBox[n].Width = ScalingDisplay(_BW, loc_DisplayScale);
                        ResultBox[n].Height = ScalingDisplay(_BL, loc_DisplayScale);
                        break;
                }
                CreateBundleGraphics(n, LayerBundle[n].Ori, ResultBox);
                ResultBox[n].Visible = true;
            }
        }
        private void CreateBundleGraphics(int BoxIndex, int Orientation, PictureBox[] ResultBox)
        {
            int W = ResultBox[BoxIndex].Width;
            int H = ResultBox[BoxIndex].Height;
            Font font1 = new Font("Arial", 16);

            Bitmap buf_BundlesDetail = new Bitmap(W, H);
            System.Drawing.Pen pen1 = new System.Drawing.Pen(ColorTranslator.FromHtml("#993300"), 2F);
            using (Graphics g = Graphics.FromImage(buf_BundlesDetail))
            {
                switch (Orientation)
                {
                    case 0: // 0 degree
                        //g.DrawLine(pen1, 20, 0, 20, H);
                        g.DrawLine(pen1, W * 1 / 5, H / 2, W * 4 / 5, H / 2);
                        g.DrawEllipse(pen1, 2, 2, 8, 8);
                        break;
                    case 1: // 90 degree
                        //g.DrawLine(pen1, 0, 20, W, 20);
                        g.DrawLine(pen1, W / 2, H * 1 / 5, W / 2, H * 4 / 5);
                        g.DrawEllipse(pen1, W - 12, 2, 8, 8);
                        break;
                    case 2: // 180 degree
                        //g.DrawLine(pen1, W - 20, 0, W - 20, H);
                        g.DrawLine(pen1, W * 1 / 5, H / 2, W * 4 / 5, H / 2);
                        g.DrawEllipse(pen1, W - 12, H - 12, 8, 8);
                        break;
                    case 3: // 270 degree
                        //g.DrawLine(pen1, 0, H - 20, W, H - 20);
                        g.DrawLine(pen1, W / 2, H * 1 / 5, W / 2, H * 4 / 5);
                        g.DrawEllipse(pen1, 2, H - 12, 8, 8);
                        break;
                }
                g.DrawString((BoxIndex + 1).ToString(), font1, Brushes.Black, 10, 10);
            }
            ResultBox[BoxIndex].Image = buf_BundlesDetail;
            ResultBox[BoxIndex].BringToFront();
        }

        private Control FindControlInPanel(Panel pnl, string ctrlName)
        {
            Control result = null;

            try
            {
                foreach (Control ctrl in pnl.Controls)
                {
                    if (ctrl.Name == ctrlName)
                    {
                        result = ctrl;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - " + e.Message, LogType.Fail);
                result = null;
            }

            return result;
        }

        private int ScalingDisplay(int IntValue, float DisplayScale)
        {
            return (int)((float)IntValue * DisplayScale);
        }
        #endregion

        #region 3DSimulation
        public void Simulation3D(Bundle3DCls BD3D, PatternModel ptn, int BundleNo, bool NewPallet = false)
        {
            if (NewPallet == true)
            {
                BD3D.ClearPallet();
                pic3DOrder.Image?.Dispose();
                //if (ptb3DOrder.Image != null)
                //    ptb3DOrder.Image.Dispose();
                pic3DOrder.Image = null;
            }

            BD3D.Pattern = ptn.Pattern;

            if ((BD3D.BDS.Count == 0 || BD3D.BDS.Count < BundleNo) && BundleNo > 0)
            {
                for (int i = BD3D.BDS.Count; i < BundleNo; i++)
                {
                    BD3D.AddBundlebyBundleNo(i + 1);
                }
            }

            BD3D.DrawBundle(pic3DOrder);
        }
        #endregion

        #region Click-a-ble objects
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            OrderGrid_ImportOrderData();
        }

        // * Press Send Data *
        // - Check if there is Order with Running State or not
        //      - if found alarm and return
        // - Call SendBuff_Cmd() to begin sending data
        //      -   Check if SelecRow is not empty (if empty alarm and return)
        //      -   Check PLC connection (if offline alarm and return)
        //      -   Check if PLC is ready to recieved buffer or not (by reading PC_BlockPCAction)
        //      -   Move data of the 1st selected row to buffer row-variable 'rOrder'
        //      -   Move Order data from rOrder to Order variable 'odm'
        //      -   Move Pattern data from rOrder to Pattern variable 'ptn'
        //      -   Get Placement Data from tblPlacement to Laying[] variable 'DBPlacement'
        //      -   Check if pattern in 'ptn' and 'DBPlacement' is the same (if not alarm and return)
        //      -   Set PLC 'PC_AddidngData' to true (if cannot set, alarm (by PLCNotify) and return)
        //      -   Set PLC 'PC_DataGuarantee' to false (if cannot set, alarm (by PLCNotify) and return)
        //      -   Send 'odm' & 'ptn' data to PLC
        //          -   If fail alarm and return
        //      -   Set PLC 'PC_DataGuarantee' to true (if cannot set, alarm(by PLCNotify) and return)
        //      -   Set PLC 'PC_AddingData' to false (if cannot set, alarm(by PLCNotify) and return)
        //      -   Change all Order with state 1 to 0 and change this order state to 1
        //      -   Add Placementinfo and some of the sending data to log file
        //      -   Start tmrConfirmStart to monitor PLC 'PC_Start_Done'
        private void btnSendData_Click(object sender, EventArgs e)
        {

            if (OrderGrid_FindOrderByState(OrderState.Using, false) != null)
            {
                string buf_Msg = "Another Order is currently running in the system.\n" +
                                    "Please stop that Order either by:\n" +
                                    " - Pressing 'Lot-End' (Finish that Order)\n" +
                                    " - Pressing 'E-Stop' then 'Reset Data' (Cancel that Order)\n\n" +
                                    "Then press 'Send Data' again.\n\n\n" +
                                    "* Remark for Debug *\n" +
                                    "Detected from Order State in Database";
                string buf_Topic = "Error! - Another Order is currently running.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var buf_chkErrStat = PLCCom.GetPLCVariable(FieldPLCs.PC_Error_Status, "int");
            if (buf_chkErrStat.Item1.Equals(false))
            {
                lblSystemStatusDisp.ForeColor = Color.Red;
                lblSystemStatusDisp.BackColor = Color.Yellow;
                lblSystemStatusDisp.Text = "Cannot read PLC signal.";
                string buf_Msg = "The System can not check the Emergency Stop Status.\n" +
                                    "Please check the connection to the PLC\n" +
                                    " - Check LAN connection to the PLC\n" +
                                    " - Check if Sysmac Studio is working properly.\n" +
                                    " - Try restart the Program or the Computer.\n\n" +
                                    "Transfer Data is not allow if it can not check PLC's Emergency Stop State.";
                string buf_Topic = "Error! - Not Allow to send Data.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if ((int)buf_chkErrStat.Item2 == 5)
                {
                    lblSystemStatusDisp.ForeColor = Color.Red;
                    lblSystemStatusDisp.BackColor = Color.Yellow;
                    lblSystemStatusDisp.Text = "Emergency Stop";
                    string buf_Msg = "The System is currently in Emergency Stop State.\n" +
                                        "Reset the Emergency Stop at the Robot Area and Try again.";
                    string buf_Topic = "Warning! - Not Allow to send Data.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            SendBuff_CMD();
        }
        public void SendBuff_CMD()
        {
            if (dgvOrderList.SelectedRows != null && dgvOrderList.SelectedRows.Count == 1)
            {

                #region PLC Connection Checking
                if (!PLCCom.CheckPLCIsActive(_PLCIP))
                {
                    UICls.ShowWaiting(false);
                    string buf_Msg = "Cannot send data to the PLC.\n" +
                                        "Please check PLC connection.";
                    string buf_Topic = "Error! - Cannot connect to the PLC.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #endregion

                #region OLD ABB Connection Checking
                if (RobotModel == 1)
                {
                    if (!ABBCom.CheckRobotIsActive())
                    {
                        UICls.ShowWaiting(false);
                        string buf_Msg = "Cannot send data to the Robot\n" +
                                            "Please check the Robot connection.";
                        string buf_Topic = "Error! - Cannot connect to the Robot.";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                #endregion

                #region Check PLC Block Transfer Status
                bool OnBlock = PLCCom.Check_BlockActivity();
                //OnBlock = false; // Test Only
                if (OnBlock == true)
                {
                    UICls.ShowWaiting(false);
                    string buf_Msg = "Another Order is currently running in the system.\n" +
                                        "Please stop that Order either by:\n" +
                                        " - Pressing 'Lot-End' (Finish that Order)\n" +
                                        " - Pressing 'E-Stop' then 'Reset Data' (Cancel that Order)\n\n" +
                                        "Then press 'Send Data' again.\n\n\n" +
                                        "* Remark for Debug *\n" +
                                        "Detected from PLC's Block Signal";
                    string buf_Topic = "Error! - Another Order is currently running.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    BlockSendData_byResetStatus(true, "Transfer Error!!!");
                    return;
                }
                #endregion

                UICls.ShowWaiting(true);
                DataGridViewRow rOrder = dgvOrderList.SelectedRows[0];
                if (rOrder != null)
                {
                    #region Assign data to ODM, PTM, Placement
                    OrderModel buf_odm;
                    buf_odm = GetOrder(rOrder);
                    buf_odm.TieSheetWidth = ClientData.Selected_TSWidth;
                    buf_odm.TieSheetLength = ClientData.Selected_TSLength;

                    //Verify Pattern 2020-08-07 by Beer
                    // buf_ptm              value are from 'rOrder'
                    // buf_odm.MaterialNo   value are also from 'rOrder'
                    // The only reason that these value won't match is because Placement is Incorrect
                    PatternModel buf_ptm = GetPatternOrder(rOrder);
                    Laying[] DBPlacement = GetPlacementfromDB(buf_odm.MaterialNo);
                    #endregion

                    #region Check PatternName from DataRow and tblPlacement
                    if (buf_ptm.Pattern != DBPlacement[0].PatternName) // Check PatternName of 1st Row of the selected row (selected by matching MaterialNO)
                    {
                        UICls.ShowWaiting(false);
                        string buf_Msg = "Placement Data and Order Data in Database does not match.\n" +
                                            "Please set-up this order again by:\n" +
                                            " - Double click this Order in the Data Grid\n" +
                                            " - Set up all the parameter again\n" +
                                            " - Press Save button and try sending this data again";
                        string buf_Topic = "Error! - Placement Data does not match.";
                        MessageBox.Show(new Form { TopMost = true, Owner = this }, buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    #endregion

                    #region Set PLC AddingData and Reset DataGuarantee
                    //Set Interlock Flag
                    var PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_AddingData, true);
                    if (PLCResp.Item1 == false) { PLCNotify(FieldPLCs.PC_AddingData, PLCResp); return; }

                    //Clear Guarantee Flag
                    // ABB Robot
                    if (RobotModel == 1)
                    {
                        var ROBResp = ABBCom.WriteABBValue(ABBStaticFields.PC_DataGuarantee.ToString(), false);
                        if (ROBResp.Item1 == false) { RobotNotify(ABBStaticFields.PC_DataGuarantee.ToString(), ROBResp); return; }
                    }
                    // PLC
                    PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_DataGuarantee, false);
                    if (PLCResp.Item1 == false) { PLCNotify(FieldPLCs.PC_DataGuarantee, PLCResp); return; }
                    #endregion

                    Running_ptm = buf_ptm;
                    Running_odm = buf_odm;

                    #region Send Data to the Robot and PLC
                    bool IsComplete;
                    //== Send Data to the Robot
                    if (RobotModel == 1)
                    {
                        IsComplete = ABBCom.Send_NewOrder(buf_odm, buf_ptm, DBPlacement);
                        if (IsComplete == false)
                        {
                            UICls.ShowWaiting(false);
                            string buf_Msg = "ID4P can not write some data to the Robot\n\n" +
                                                "Please see log for details";
                            string buf_Topic = "Error! - Can not send data to the Robot.";
                            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log.WriteLog("Error! - Cannot write some data to the Robot.", LogType.Fail);
                            BlockSendData_byResetStatus(true, "Transfer Error!");
                            return;
                        }
                    }

                    //== Send Data to the PLC
                    IsComplete = PLCCom.Send_NewOrder(buf_odm, buf_ptm, DBPlacement, RobotModel);
                    if (IsComplete == false)
                    {
                        UICls.ShowWaiting(false);
                        string buf_Msg = "ID4P can not write some data to the PLC\n\n" +
                                            "Please see log for details";
                        string buf_Topic = "Error! - Can not send data to the PLC.";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.WriteLog("Error! - Cannot write some data to the PLC.", LogType.Fail);
                        BlockSendData_byResetStatus(true, "Transfer Error!");
                        return;
                    }
                    #endregion

                    #region Set Guarantee Flag and Reset PLC AddingData Flag
                    if (RobotModel == 1)
                    {
                        var ROBResp = ABBCom.WriteABBValue(ABBStaticFields.PC_DataGuarantee.ToString(), true);
                        if (ROBResp.Item1 == false) { RobotNotify(ABBStaticFields.PC_DataGuarantee.ToString(), ROBResp); return; }
                    }
                    PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_DataGuarantee, true);
                    if (PLCResp.Item1 == false) { PLCNotify(FieldPLCs.PC_DataGuarantee, PLCResp); return; }
                    //Clear Interlock Flag
                    PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_AddingData, false);
                    if (PLCResp.Item1 == false) { PLCNotify(FieldPLCs.PC_AddingData, PLCResp); return; }
                    #endregion

                    #region Change Order State to Buffer

                    string Query = "UPDATE tblOrder SET OrderState = 0 WHERE OrderState = 1";
                    bool CanClear = Sql.ExcSQL(Query);
                    if (CanClear == false)
                    {
                        Log.WriteLog("Error! - Cannot Send data. Fail to update OrderState from 1 to 0 in tblOrder", LogType.Fail);
                        BlockSendData_byResetStatus(true, "Transfer Error!");
                        return;
                    }

                    Query = "UPDATE tblOrder SET OrderState = 1 WHERE ID = " + rOrder.Cells["ID"].Value.ToString();
                    bool CanUpdate = Sql.ExcSQL(Query);
                    if (CanUpdate == false)
                    {
                        Log.WriteLog("Error! - Cannot Send Data. Fail to update OrderState to 1 in tblOrder", LogType.Fail);
                        BlockSendData_byResetStatus(true, "Transfer Error!");
                        return;
                    }
                    BlockSendData_byResetStatus(false);
                    Log.WriteLog("Complete Sequence! - Successfully send all the data to the PLC and the Robot Controller.", LogType.Success);
                    OrderGrid_ImportOrderData();
                    #endregion

                    #region Logging
                    Log.WriteLog("---[Start]Send data MaterialNo:" + buf_odm.MaterialNo + "---", LogType.Notes);
                    Log.WriteLog(string.Format("PT:{0}, BL:{1}, BW:{2}, SQ_OPX:{3}, SQ_OPY:{4}, SQ_CLX:{5}, SQ_CLY:{6}, LY_WIDTH:{7}, LY_LENGHT:{8}, SwapSide:{9}", buf_ptm.Pattern, buf_ptm.BundleLength, buf_ptm.BundleWidth, buf_ptm.SQ_XaxisValue, buf_ptm.SQ_YaxisValue, buf_ptm.SQ_XaxisValueWithOutGap, buf_ptm.SQ_YaxisValueWithOutGap, buf_ptm.LayerClosePeriWidth, buf_ptm.LayerClosePeriLength, buf_ptm.SwithBDSize), LogType.Notes);
                    string _PlacementXY = "";
                    for (int i = 0; i < DBPlacement.Length; i++)
                    {
                        _PlacementXY += string.Format("[{0}]({1},{2},{3})", DBPlacement[i].Step, DBPlacement[i].X, DBPlacement[i].Y, DBPlacement[i].Ori);
                        if (i != DBPlacement.Length - 1)
                            _PlacementXY += ", ";
                    }
                    string _ARR_TieSheet = buf_odm.GetArrTieSheet();
                    Log.WriteLog(string.Format("Placement:{0}", _PlacementXY), LogType.Notes);
                    Log.WriteLog(string.Format("TieSheet:{0}", _ARR_TieSheet), LogType.Notes);
                    Log.WriteLog("---[Finish]Send data MaterialNo:" + buf_odm.MaterialNo + "---", LogType.Notes);
                    #endregion

                    Latest_FeedData.Reset_FeedData();
                    #region Start Timer for Run Order
                    if (!tmrConfirmStart.Enabled)
                        tmrConfirmStart.Start();
                    #endregion
                }
                UICls.ShowWaiting(false);
            }
            else
            {

                string buf_Msg = "No Data has been selected.\n" +
                                    "- Click on the desired DataRow to select it.\n\n" +
                                    "* If Data Grid is disappeared:\n" +
                                    "   - There might be something happened to the Database.\n";
                string buf_Topic = "Warning! - No Data Row has been selected.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //  * Press New Order *
        //  -   Check If KeyBoard Language is English ( if not, alarm and return)
        //  -   Create form frmNewOrderDialog
        //  -   If Material NO start with 'Z' enable SO NO fill form
        //  -   If Material NO isn't start with 'Z'
        //      -   query for master data in 'tblMaster' that has matching Material NO
        //      -   if no matching row
        //          -   open Empty Product form with that Material NO
        //      -   if matching row is found
        //          -   move data from matching row in tblMaster to tblOrder
        //          -   copy that data to Data Grid View
        private void btnNewOrder_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Before click event: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            //Check Language
            var Language = System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.Name;
            if (!string.IsNullOrEmpty(Language))
            {
                if (!Language.ToLower().StartsWith("en"))
                {
                    string buf_Msg = "Please change current keyboard language to English and then try again.";
                    string buf_Topic = "Error! - Invalid System Language.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            Console.WriteLine("After Check Language: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            frmNewOrderDialog _f = new frmNewOrderDialog();
            string MatSO;
            string SONo = "";
            if (_f.ShowDialog() == DialogResult.OK)
            {
                MatSO = _f.Material_SO;
                if (Properties.Settings.Default.Print_Label == true)
                    SONo = _f.SONo;

                GetOrderDataFromDB(MatSO, SONo);
            }
            Console.WriteLine("After Code: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        }
        private void GetOrderDataFromDB(string MatSO, string SONo = "")
        {
            string Query;
            string OrderNo;
            string MaterialNo = "";
            bool IsOrderNo = MatSO.StartsWith("Z")? false : true;
            if (IsOrderNo == false) // Material false start with 'Z'
            {
                MaterialNo = MatSO;
                // Can replace '@MatNo' anytime you want with Replace Method
                Query = "SELECT count([Material_No]) FROM [tblMaster] WHERE Material_No =  '@MatNo'";
                Query = Query.Replace("@MatNo", MaterialNo);
                int CNTMaster = (int)Sql.GetScalarFromSql(Query);
                if (CNTMaster == 0)
                {
                    string buf_Msg = "ID4P couldn't find matching Material Number in History data.\n" +
                                        "Do you want to manually key the Order Data?";
                    string buf_Topic = "Error! - Can not find Data";
                    if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //Open Master
                        // Find Open form of type 'frmMain'
                        // If form is found
                        var _fParent = UICls.FindOpenForm("frmMain");
                        if (_fParent != null)
                        {
                            // cast the found form to frmMain type
                            // Create Product Form with given Material Number
                            frmMain fParent = (frmMain)_fParent;
                            fParent.ShowProductForm(MaterialNo);
                        }
                    }
                }
                else
                {
                    OrderNo = SONo;
                    int result = NewOrderByHistoryMaster(MaterialNo, OrderNo);
                    if (result == -1)
                    {
                        string buf_Msg = "There is a matching Material Number in History Data.\n" +
                                            "[ " + MaterialNo + " ]\n" +
                                            "But ID4P unsuccessfully pulled from History data.\n" +
                                            "Do you want to manually key the Order Data?";
                        string buf_Topic = "Error! - Can not find Data";
                        if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //Open Master
                            var _fParent = UICls.FindOpenForm("frmMain");
                            if (_fParent != null)
                            {
                                frmMain fParent = (frmMain)_fParent;
                                fParent.ShowProductForm(MaterialNo);
                            }
                        }
                        Log.WriteLog("Error! - Cannot create order from tblMaster where MaterialNo is " + MaterialNo, LogType.Fail);
                    }
                    else
                    {
                        if (result == 1)
                            OrderGrid_ImportOrderData();
                    }
                }
            }
            else
            {
                if (Properties.Settings.Default.OrderFromSystem)
                {
                    string ERPCon;
                    if (Properties.Settings.Default.PM3_ERP == false)
                        ERPCon = Sql.GetPM2Connection();
                    else
                        ERPCon = Sql.GetPM3Connection();

                    OrderNo = MatSO;
                    Query = "SELECT [Material_No] FROM [MO_Spec] WHERE OrderItem = '@OrderNo'";
                    Query = Query.Replace("@OrderNo", OrderNo);
                    Query = (Properties.Settings.Default.PM3_ERP == false) ? Query : Query + " AND FactoryCode = (SELECT TOP 1 Plant FROM CompanyProfile WHERE ShortName = '" + Properties.Settings.Default.PlantCode + "') ";
                    var buf_MatNo = Sql.GetScalarFromSql(Query, ERPCon);
                    //MaterialNo = (_MatNo != null ? _MatNo.ToString() : "");
                    string buf_MatFound = (buf_MatNo != null ? buf_MatNo.ToString() : "");
                    if (string.IsNullOrEmpty(buf_MatFound))
                    {
                        string buf_Msg = "There is no matching SO Number in PM2 or PM3\n" +
                                            "or maybe the PM2, PM3 server is down.\n" +
                                            "Do you want to try using MaterialNo to find it in the History Data instead?";
                        string buf_Topic = "Error! - Can not find Data";
                        if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            buf_Msg = "Press New Order button again then manually type Material Number and SO Number.";
                            buf_Topic = "Add New Order again.";
                            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            buf_Msg = "Do you want to manually key the Order Data?";
                            buf_Topic = "Error! - Can not find Data";
                            if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                //Open Master
                                var _fParent = UICls.FindOpenForm("frmMain");
                                if (_fParent != null)
                                {
                                    frmMain fParent = (frmMain)_fParent;
                                    fParent.ShowProductForm(OrderNo);
                                }
                                // Get OrderGrid_GetOrderData() will be call at ShowOrderForm when press 'Save' in fProduct
                            }
                        }
                    }
                    else
                    {
                        int result = NewOrderByHistoryMaster(buf_MatFound, OrderNo);
                        if (result == -1)
                        {
                            string buf_Msg = "There is a matching Material Number in PM2 or PM3.\n" +
                                                "[ " + buf_MatFound + " ]\n" +
                                                "But ID4P unsuccessfully pulled from History data.\n\n" +
                                                "*It either because database error or couldn't find match Material Number in History data \n" +
                                                "Do you want to manually key the Order Data?";
                            string buf_Topic = "Error! - Can not find Data";
                            if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                //Open Master
                                var _fParent = UICls.FindOpenForm("frmMain");
                                if (_fParent != null)
                                {
                                    //*checkagain* the MaterialNo will be ""
                                    frmMain fParent = (frmMain)_fParent;
                                    fParent.ShowProductForm(buf_MatFound);
                                }
                            }
                        }
                        else
                        {
                            if (result == 1)
                                OrderGrid_ImportOrderData();
                        }
                    }
                }
                else
                {

                }
            }
        }
        private int NewOrderByHistoryMaster(string MaterialNo, string OrderNo)
        {
            int result;
            try
            {
                string Query = string.Format(@"UPDATE tblOrder SET Material_No  = '{0}' WHERE Material_No = '{0}' AND (OrderState = {1} OR OrderState = {2})", MaterialNo, 1,2);
                int RowEffect = Sql.ExcSQLwithReturn(Query);
                if (RowEffect == 0)
                {
                    Query = string.Format(@"DELETE FROM tblOrder WHERE Material_No = '{0}' AND OrderState = {1}", MaterialNo, 0);
                    Sql.ExcSQL(Query);

                    // select all column of the row that has match material number (also change LotNum to the value of Orderno)
                    Query = string.Format(@"INSERT INTO tblOrder ([Material_No],[StampDate],[OrderState],[Product_Code],[ActualBox_L],[ActualBox_W],[ActualBox_H]
                                                                ,[Lot_No],[Box_Type],[Flute_Type],[PiecePerBD],[BDPerLY],[PiecePerLY]
                                                                ,[LYPerPallet],[Pallet_Type],[Pallet_W],[Pallet_L],[Pallet_H]
                                                                ,[BDPerGrip],[Pattern_Code],[Efficiency],[Robot_Speed]
                                                                ,[TopSheet],[TS_everyXLY],[BtmSheet],[FoldWork_W],[FoldWork_L],[FoldWork_T]
                                                                ,[FoldWork_Weight],[Peri_OpenL],[Peri_OpenW],[FixBDFace]
                                                                ,[Squaring],[SQ_OpenX],[SQ_OpenY],[SQ_CloseX],[SQ_CloseY],[GripperFinger]
                                                                ,[Peri_CloseL],[Peri_CloseW],[RotatePattern],[SwitchBDSize], [SpecialRotate]
                                                                ,[TS_B4LastLY], [TS_SQLayer], [StackerLiftBD], [SQ_Extra], [PlacingMode], [PatternSeq]
                                                                ,[ExtraPickDepth], [DischargeMode], [AntiBounce],[HeightRatio])
                                            SELECT [Material_No],GETDATE(),{2},[Product_Code],[ActualBox_L],[ActualBox_W],[ActualBox_H]
                                                    ,'{0}',[Box_Type],[Flute_Type],[PiecePerBD],[BDPerLY],[PiecePerLY]
                                                    ,[LYPerPallet],[Pallet_Type],[Pallet_W],[Pallet_L],[Pallet_H]
                                                    ,[BDPerGrip],[Pattern_Code],[Efficiency],[Robot_Speed]
                                                    ,[TopSheet],[TS_everyXLY],[BtmSheet],[FoldWork_W],[FoldWork_L],[FoldWork_T]
                                                    ,[FoldWork_Weight],[Peri_OpenL],[Peri_OpenW],[FixBDFace]
                                                    ,[Squaring],[SQ_OpenX],[SQ_OpenY],[SQ_CloseX],[SQ_CloseY],[GripperFinger]
                                                    ,[Peri_CloseL],[Peri_CloseW],[RotatePattern],[SwitchBDSize], [SpecialRotate]
                                                    ,[TS_B4LastLY], [TS_SQLayer], [StackerLiftBD], [SQ_Extra], [PlacingMode],[PatternSeq]
                                                    ,[ExtraPickDepth], [DischargeMode], [AntiBounce],[HeightRatio]
                                            FROM tblMaster WHERE Material_No = '{1}'", OrderNo, MaterialNo, 0);
                    // excute the INSERT statement from Query variable and return the number of effect row
                    RowEffect = Sql.ExcSQLwithReturn(Query);
                    result = (RowEffect == 1)? 1 : -1;
                }
                else
                {
                    string buf_Msg = "There is the data with the same MaterialNo that has already been sent to the Robot.\n" +
                                        "If you want to add this data again.\n" +
                                        "Please ResetData or LotEnd current data first.";
                    string buf_Topic = "Error! - Can not Add New Data.";
                    MessageBox.Show(buf_Msg,buf_Topic,MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = 0;
                }
                return result;
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - Cannot import new data from master data of Material:" + MaterialNo + "." + e.Message, LogType.Fail);
                result = -1;
                return result;
            }
        }

        // * Press Reset Data
        //
        private void btnResetData_Click(object sender, EventArgs e)
        {
            string buf_Msg = "Do you want to reset data?\n" +
                                "Click YES to proceed.\n" +
                                "Click NO to Cancel";
            string buf_Topic = "Reset Data Confirmation.";
            if (MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var buf_chkDoorLock = PLCCom.GetPLCVariable(FieldPLCs.PC_Door_Status, "int");
                if (buf_chkDoorLock.Item1.Equals(false))
                {
                    buf_Msg = "The System was trying to check Door Status from PLC.\n" +
                            "But it can not read the signal.\n" +
                            "Please check the connection between the Computer and PLC then try again.";
                    buf_Topic = "Error! - Can not start Reset Data sequence.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if ((int)buf_chkDoorLock.Item2 == 2)
                    {
                        flag_ResetData = true;  // will be use to distinguish Normal LotEnd and LotEnd from Reset Data
                                                // PLC need LotEnd Sequence from Reset Data to complete the sequence and unblock sendding data.
                        ResetData_CMD();
                        flag_ResetData = false;
                    }
                    else
                    {
                        buf_Msg = "The Door at the palletizer is not locked.\n" +
                                "Please lock the door and try again.";
                        buf_Topic = "Error! - Can not start Reset Data sequence.";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public void ResetData_CMD()
        {
            try
            {
                UICls.ShowWaiting(true);

                #region PLC Connection Checking
                if (!PLCCom.CheckPLCIsActive(_PLCIP))
                {
                    UICls.ShowWaiting(false);
                    string buf_Msg = "Cannot send Reset Cmd to PLC.\n" +
                                        "Please check PLC connection.";
                    string buf_Topic = "Error! - Cannot connect to PLC.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                #endregion

                #region Check PLC Block Transfer Status
                bool OnBlock = PLCCom.Check_BlockActivity();
                if (OnBlock == true)
                {
                    UICls.ShowWaiting(false);
                    string buf_Msg = "Another Order is currently running in the system.\n" +
                                        "Please stop that Order either by:\n" +
                                        " - Pressing 'Lot-End' (Finish that Order)\n" +
                                        " - Pressing 'E-Stop' then 'Reset Data' (Cancel that Order)\n\n\n" +
                                        "* Remark for Debug *\n" +
                                        "Detected from PLC's Block Signal";
                    string buf_Topic = "Error! - Another Order is currently running.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    BlockSendData_byResetStatus(true, "Reset Block!");
                    return;
                }
                #endregion

                this.UseWaitCursor = true; // will be set to false at BlockSendData_byResetStatus
                var CanReset = PLCCom.SetPLCVariable(FieldPLCs.PC_ResetData, true);
                if (CanReset.Item1 == true)
                {
                    if (!tmrLotEnd.Enabled) //[2020-09-09]Update SEQ Timer @Beer
                    {
                        tmrLotEnd.Start();
                        Log.WriteLog("<TimeStamp> - Start LotEnd Timer", LogType.Notes);
                    }
                    Log.WriteLog("Successfully send "+ FieldPLCs.PC_ResetData + " to PLC.", LogType.Success);

                    int checktime = 5000; // 5 second
                    int maxtime = 60; // 180 = 3 Minute
                    int time = 0;
                    bool TaskEnd = false;

                    LogCls _Log = new LogCls();
                    Task.Run(() =>
                    {
                        while (!TaskEnd)
                        {
                            time += 1000; // 1 second
                            Thread.Sleep(1000);
                            if (time % checktime == 0)
                            {
                                var IsResetComplete = PLCCom.GetPLCVariable(FieldPLCs.PC_ResetData);
                                if (IsResetComplete.Item1.Equals(true))
                                {
                                    if (IsResetComplete.Item2.Equals(false))
                                    {
                                        var IsResetError = PLCCom.GetPLCVariable(FieldPLCs.PC_CommuError);
                                        if (IsResetError.Item1.Equals(true))
                                        {
                                            if (IsResetError.Item2.Equals(true))
                                            {
                                                Log.WriteLog("Error! - Reset PLC or Robot internal Error!!!", LogType.Fail);
                                                UICls.ShowWaiting(false);
                                                BlockSendData_byResetStatus(true, "Reset Error!");
                                            }
                                            else
                                            {
                                                Log.WriteLog("Complete Sequecne! - Successfully Reset data in both the PLC and the Robot Controller.", LogType.Success);
                                                ResetData_PC();
                                            }
                                        }
                                        else
                                        {
                                            Log.WriteLog("Error! - Cannot reset data. Cannot read PLC data [" + FieldPLCs.PC_CommuError + "]", LogType.Fail);
                                            UICls.ShowWaiting(false);
                                            BlockSendData_byResetStatus(true, "Reset Error!");
                                        }
                                        TaskEnd = true;
                                    }
                                }
                                Log.WriteLog("<TimeStamp> - Try to read PLC data [" + FieldPLCs.PC_ResetData + "]" + time.ToString(), LogType.Notes);
                            }

                            if (time > (maxtime * 1000))
                            {
                                Log.WriteLog("Error! - Cannot reset data. took to long to read PLC data [" + FieldPLCs.PC_ResetData + "]", LogType.Fail);
                                UICls.ShowWaiting(false);
                                BlockSendData_byResetStatus(true, "Reset TimeOut!");
                                TaskEnd = true;
                            }
                        }
                    });
                }
                else
                {
                    UICls.ShowWaiting(false);
                    string buf_Msg = "Can not send Reset CMD to the PLC\n" +
                                        "Please try again.\n\n" +
                                        "if the problem persist check PLC variable (PC_ResetData)";
                    string buf_Topic = "Error! - Can not send Reset CMD to the PLC.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.WriteLog("Error! - Cannot reset data. Cannot write PLC data [" + FieldPLCs.PC_ResetData + "]: " + CanReset.Item2.ToString(), LogType.Fail);
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - Cannot reset data: " + e, LogType.Fail);
            }
        }
        private void ResetData_PC()
        {
            //Update Database
            string Query = "UPDATE tblOrder SET OrderState = 0 WHERE OrderState != 3";
            bool CanUpdate = Sql.ExcSQL(Query);
            string buf_msg = "Can not completely reset data.\n" +
                            "Can not update some row's OrderState in DataBase to normal";
            string buf_topic = "Error! - Reset Data is not completed!";
            if (CanUpdate == false)
            {
                MessageBox.Show(buf_msg, buf_topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.WriteLog("Error! - Cannot reset data. Cannot update OrderState in tblOrder", LogType.Fail);
                BlockSendData_byResetStatus(true, "Reset Error!");
                return;
            }
            BlockSendData_byResetStatus(false);
            Log.WriteLog("Complete Sequence! - ID4P successfully complete reset data sequence.", LogType.Success);
            UICls.ShowWaiting(false);

            Latest_FeedData.Reset_FeedData();

            buf_msg = "The System has completely reset the data.";
            buf_topic = "Reset Data is Completed!";
            MessageBox.Show(buf_msg, buf_topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
            OrderGrid_ImportOrderData();
        }
        #endregion

        private void PLCNotify(FieldPLCs _Field, Tuple<bool, object> _Resp)
        {
            UICls.ShowWaiting(false);
            string buf_Msg = "ID4P can not write value to PLC's Variable\n" +
                            "[ " + _Field.ToString() + " ]\n\n" +
                            "Please check the connection to the PLC\n" +
                            " - Check LAN connection to the PLC\n" +
                            " - Check if Sysmac Studio is working properly.\n" +
                            " - Try restart the Program or the Computer.\n\n";
            string buf_Topic = "Error! - Cannot Send Data.";
            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Log.WriteLog("Error! - Cannot write PLC value [" + _Field.ToString() + "]: " + _Resp.Item2.ToString(), LogType.Fail);
            BlockSendData_byResetStatus(true, "Transfer Error!");
        }
        private void RobotNotify(string _Field, Tuple<bool, object> _Resp)
        {
            UICls.ShowWaiting(false);
            string buf_Msg = "ID4P can not write value to PLC's Variable\n" +
                                "[ " + _Field + " ]\n\n" +
                                "Please check the connection to the Robot\n" +
                                " - Check LAN connection to the Robot\n" +
                                " - Try restart the Program or the Computer.\n\n";
            string buf_Topic = "Error! - Cannot Send Data.";
            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Log.WriteLog("Error! - Cannot write Robot data [" + _Field.ToString() + "]: " + _Resp.Item2.ToString(), LogType.Fail);
            BlockSendData_byResetStatus(true, "Transfer Error!");
        }

        private void BlockSendData_byResetStatus(bool buf_Block = true, string btnSendText = "")
        {
            // Change 'SendData' Button's colour and properties according to IsComplete variable
            btnSendText = (btnSendText == "" ? "Reset Error!" : btnSendText);
            if (btnSendData.InvokeRequired)
            {
                btnSendData.Invoke(new Action(() =>
                {
                    btnSendData.Enabled = (!buf_Block);
                    btnSendData.BackColor = (buf_Block == false ? Color.DarkOrange : ColorBtn);
                    btnSendData.Text = (buf_Block == false ? "Send Data" : btnSendText);
                }));
            }
            else
            {
                btnSendData.Enabled = (!buf_Block);
                btnSendData.BackColor = (buf_Block == false ? Color.DarkOrange : ColorBtn);
                btnSendData.Text = (buf_Block == false ? "Send Data" : btnSendText);
            }

            if (buf_Block == true)
            {
                string buf_Msg = btnSendText + "\n" + "Please Reset Data and Try Again.";
                string buf_Topic = "Error! - " + btnSendText;
                MessageBox.Show(btnSendText + " please try again.", btnSendText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.UseWaitCursor = false;
                }));
            }
            else
            {
                this.UseWaitCursor = false;
            }

        }

        #region Timer Tick
        private void TmrRobotFeed_Tick(object sender, EventArgs e)
        {
            PLCCom.CheckPLCIsActive(_PLCIP);
            if (RobotModel == 1)
            {
                //ABBCom.CheckRobotIsActive(_RobotIP);
            }
            GetRobotFeed_Event();
        }
        public void GetRobotFeed_Event(bool TestMode = false)
        {
            string loc_FeedDataGroup = "Null";
            try
            {
                bool buf_Connected;
                if (RobotModel == 1)
                    buf_Connected = PLCCom.PLCActive && ABBCom.RobotActive;
                else
                    buf_Connected = PLCCom.PLCActive;

                btnDoorUnlock.Enabled = buf_Connected;
                btnDoorLock.Enabled = buf_Connected;
                btnConfirmStart.Enabled = buf_Connected;
                btnLotEnd.Enabled = buf_Connected;
                btnHalfFinish.Enabled = buf_Connected;
                loc_FeedDataGroup = "Complete setting parameters";

                //*** comment for bypass
                if (buf_Connected)
                //*** uncomment for normal:if(true)
                {
                    loc_FeedDataGroup = "Getting PLC's Door Status";
                    GetDoorStatus();
                    loc_FeedDataGroup = "Getting PLC's Status";
                    GetSystemStatus();
                }
                else
                {
                    if (RobotModel == 1)
                    {
                        lblDoorStatusDisp.Text = "PLC, Robot connection is lost";
                        lblSystemStatusDisp.Text = "PLC, Robot connection is lost";
                    }
                    else
                    {
                        lblDoorStatusDisp.Text = "Can not connect to PLC";
                        lblSystemStatusDisp.Text = "Can not connect to PLC";
                    }
                    lblDoorStatusDisp.ForeColor = Color.Red;
                    lblSystemStatusDisp.ForeColor = Color.Red;
                    lblDoorStatusDisp.BackColor = Color.Yellow;
                    lblSystemStatusDisp.BackColor = Color.Yellow;
                    return;
                }

                if (IsOnGetRobotFeed == true)
                    return;

                if (TestMode)
                {
                    if (Latest_FeedData.Pallet_Count < 1)
                    {
                        Latest_FeedData.Pallet_Count = 1;
                        Latest_FeedData.PrvPallet_Count = 1;
                    }
                }
                else
                {

                }

                loc_FeedDataGroup = "Check if need to find Order";
                // these 3 variable value will be assigned at SendBuf_CMD and OrderGrid_FindOrderbyState( if State is Using or Buffer )
                if (Running_ptm == null || Running_ptm.Pattern == null || Running_odm == null)
                {
                    loc_FeedDataGroup = "Finding Order by State";
                    OrderGrid_FindOrderByState(OrderState.Using, true, true, false);
                }

                IsOnGetRobotFeed = true;
                UIFeedDataModel buf_FeedData = new UIFeedDataModel();
                if (TestMode)
                {
                    buf_FeedData = buf_FeedData.TestValue(Latest_FeedData, Running_odm, Running_ptm);
                }
                else if (RobotModel == 1)
                {
                    loc_FeedDataGroup = "Getting feed data from the Robot";
                    //buf_FeedData = ABBCom.GetFeedData();
                    Latest_FeedData = ABBCom.GetFeedData();
                }
                else
                {
                    loc_FeedDataGroup = "Getting feed data from the PLC";
                    //buf_FeedData = PLCCom.GetFeedData();
                    Latest_FeedData = PLCCom.GetFeedData();
                }
                IsOnGetRobotFeed = false;
                //if (buf_FeedData != null && Running_ptm != null && Running_odm != null)
                if (Latest_FeedData != null && Running_ptm != null && Running_odm != null)
                {
                    //if (buf_FeedData.PalletBD_Count.Count < 1)
                    //  buf_FeedData.PalletBD_Count.Add(0);
                    if (Latest_FeedData.PalletBD_Count.Count < 1)
                    {
                        loc_FeedDataGroup = "Add PalletBD_Count";
                        Latest_FeedData.PalletBD_Count.Add(0);
                    }

                    loc_FeedDataGroup = "Check Pallet Complete";
                    //buf_FeedData.Check_PalletComplete();
                    Latest_FeedData.Check_PalletComplete();

                    loc_FeedDataGroup = "Check PalletBD_Count";
                    //if (buf_FeedData.PalletBD_Count.Count > 0 && buf_FeedData.Pallet_Count > 0)
                    //{
                    //  buf_FeedData.PalletBD_Count[buf_FeedData.Pallet_Count - 1] = (buf_FeedData.Step_Count * Running_odm.BDPerGrip) + ((buf_FeedData.Layer_Count - 1) * Running_odm.BundlePerLayer);
                    //}
                    if (Latest_FeedData.PalletBD_Count.Count > 0 && Latest_FeedData.Pallet_Count > 0)
                    {
                        loc_FeedDataGroup = "Update PalletBD_Count";
                        //Latest_FeedData.PalletBD_Count[Latest_FeedData.Pallet_Count - 1] = (Latest_FeedData.Step_Count * Running_odm.BDPerGrip) + ((Latest_FeedData.Layer_Count - 1) * Running_odm.BundlePerLayer);
                        Latest_FeedData.PalletBD_Count[Latest_FeedData.Pallet_Count - 1] = Latest_FeedData.OrderBundle_Count - Latest_FeedData.LastBD_PrvPallet;
                    }

                    loc_FeedDataGroup = "Check SO_Count";
                    if (Latest_FeedData.SO_Count > 0)
                    {
                        loc_FeedDataGroup = "Update SO_Count";
                        //Latest_FeedData.PalletBD_Count[Latest_FeedData.Pallet_Count - 1] = (Latest_FeedData.Step_Count * Running_odm.BDPerGrip) + ((Latest_FeedData.Layer_Count - 1) * Running_odm.BundlePerLayer);
                        Latest_FeedData.SOBD_Count = Latest_FeedData.OrderBundle_Count - Latest_FeedData.LastBD_PrvSO;
                    }
                    loc_FeedDataGroup = "Update Material Number";
                    //buf_FeedData.MaterialNo = Running_odm.MaterialNo;
                    Latest_FeedData.MaterialNo = Running_odm.MaterialNo;

                    loc_FeedDataGroup = "Update TieSheet Size";
                    //Latest_FeedData = buf_FeedData;
                    Running_odm.TieSheetWidth = Latest_FeedData.TieSheet_Width;
                    Running_odm.TieSheetLength = Latest_FeedData.TieSheet_Length;

                    loc_FeedDataGroup = "Check if this is running or uploaded order";
                    if (grpbxFeedData.Text == "Running Order" || grpbxFeedData.Text == "Uploaded Order")
                    {
                        loc_FeedDataGroup = "Update static feed data";
                        UpdateFeed_Static(Running_odm, Running_ptm);
                        loc_FeedDataGroup = "Update dynamic feed data";
                        UpdateFeed_Dynamic(grpbxFeedData.Text, chbRealTime.Checked, Running_odm, Running_ptm, Latest_FeedData);
                    }

                    loc_FeedDataGroup = "Compare Material Number";
                    if (Running_odm.MaterialNo != LastMatNo)
                    {
                        loc_FeedDataGroup = "Gen new Rob_WorkData";
                        Rob_WorkData = new RBWorkingModel();
                        loc_FeedDataGroup = "Clear Order3DBD";
                        running_Order3DBD?.ClearPallet();
                        //if (running_Order3DBD != null)
                        //    running_Order3DBD.ClearPallet();
                    }
                    // - If This is new Order (Material NO != Last Material NO)
                    // - Update info for 3D Picture

                    loc_FeedDataGroup = "Update LastMatNo";
                    LastMatNo = Running_odm.MaterialNo;

                    TotalErrorRobotFeed = 0;

                }
            }
            catch (Exception e)
            {
                IsOnGetRobotFeed = false;
                TotalErrorRobotFeed += 1;
                if (TotalErrorRobotFeed < 5)
                    Log.WriteLog("Error! - Fail to read Robot Feed Data [" + loc_FeedDataGroup + "]: " + e.Message, LogType.Fail);
            }
        }

        public void GetDoorStatus()
        {
            //string buf_Msg;
            //string buf_Topic;
            var buf_chkDoorLock = PLCCom.GetPLCVariable(FieldPLCs.PC_Door_Status, "int");
            //*** comment for bypass:
            if (buf_chkDoorLock.Item1.Equals(false))
            //*** uncomment for bypass:if (true) //*** comment for normal
            {
                lblDoorStatusDisp.ForeColor = Color.Red;
                lblDoorStatusDisp.BackColor = Color.Yellow;
                lblDoorStatusDisp.Text = "Cannot read PLC signal.";
            }
            else
            {
                lblDoorStatusDisp.ForeColor = Color.WhiteSmoke;
                lblDoorStatusDisp.BackColor = Color.FromArgb(64, 64, 64);
                //*** comment for bypass:
                switch ((int)buf_chkDoorLock.Item2)
                //*** uncomment for bypass:switch (2) //*** comment for normal
                {
                    case 1:
                        lblDoorStatusDisp.Text = "PLC Received Cmd (Lock).";
                        break;

                    case 2:
                        lblDoorStatusDisp.Text = "Door is Locked.";
                        break;

                    case 3:
                        lblDoorStatusDisp.Text = "PLC Received Cmd (Open).";
                        break;

                    case 4:
                        lblDoorStatusDisp.Text = "Door is Open.";
                        break;

                    default:
                        lblDoorStatusDisp.Text = "Unknow Command. (" + (int)buf_chkDoorLock.Item2 + ")";
                        break;
                }
            }
        }

        public void GetSystemStatus()
        {
            //string buf_Msg;
            //string buf_Topic;
            var buf_chkSysStat = PLCCom.GetPLCVariable(FieldPLCs.PC_System_Status, "int");
            var buf_chkErrStat = PLCCom.GetPLCVariable(FieldPLCs.PC_Error_Status, "int");
            //*** comment for bypass:
            if (buf_chkSysStat.Item1.Equals(false) || buf_chkErrStat.Item1.Equals(false))
            //*** uncomment for bypass:if(false) //*** comment for normal
            {
                lblSystemStatusDisp.ForeColor = Color.Red;
                lblSystemStatusDisp.BackColor = Color.Yellow;
                lblSystemStatusDisp.Text = "Cannot read PLC signal.";
            }
            else
            {
                //*** comment for bypass:
                if ((int)buf_chkErrStat.Item2 == 1)
                //*** uncomment for bypass:if (false) //*** comment for normal
                {
                    lblSystemStatusDisp.ForeColor = Color.WhiteSmoke;
                    lblSystemStatusDisp.BackColor = Color.FromArgb(64, 64, 64);
                    //*** comment for bypass:
                    switch ((int)buf_chkSysStat.Item2)
                    //*** uncomment for bypass: switch (3) //*** comment for normal
                    {
                        case 1:
                            lblSystemStatusDisp.Text = "PLC Received Cmd (Start)";
                            break;

                        case 2:
                            lblSystemStatusDisp.Text = "Starting System";
                            break;

                        case 3:
                            lblSystemStatusDisp.Text = "Stop";
                            break;

                        case 4:
                            lblSystemStatusDisp.Text = "PLC Received Cmd (Half-Finish)";
                            break;

                        case 5:
                            lblSystemStatusDisp.Text = "Half-Finish Sequence";
                            break;

                        case 6:
                            lblSystemStatusDisp.Text = "Running";
                            break;

                        case 7:
                            lblSystemStatusDisp.Text = "Discharging Pallet";
                            break;

                        case 8:
                            lblSystemStatusDisp.Text = "PLC Received Cmd (Lot-End)";
                            break;

                        case 9:
                            lblSystemStatusDisp.Text = "Lot-End Sequence";
                            break;

                        default:
                            lblSystemStatusDisp.Text = "Unknow Command. (" + (int)buf_chkSysStat.Item2 + ")";
                            break;
                    }
                }
                else
                {
                    lblSystemStatusDisp.ForeColor = Color.Red;
                    lblSystemStatusDisp.BackColor = Color.Yellow;
                    //*** comment for bypass:
                    switch ((int)buf_chkErrStat.Item2)
                    //*** uncomment for bypass: switch (5) //*** comment for normal
                    {
                        case 2:
                            lblSystemStatusDisp.Text = "Stop (Start Error)";
                            break;

                        case 3:
                            lblSystemStatusDisp.Text = "Stop (Half-Finish Error)";
                            break;

                        case 4:
                            lblSystemStatusDisp.Text = "Stop (Lot-End Error)";
                            break;

                        case 5:
                            lblSystemStatusDisp.Text = "Emergency Stop";
                            break;

                        default:
                            lblSystemStatusDisp.Text = "Unknow Command. (" + (int)buf_chkSysStat.Item2 + ")";
                            break;
                    }
                }
            }
        }

        private void TmrLotEnd_Tick(object sender, EventArgs e)
        {
            Check_PLC_LotEnd();
        }
        private void Check_PLC_LotEnd()
        {
            string Query;
            string Msg;
            string loc_LogTopic;
            if (flag_ResetData)
                loc_LogTopic = "Reset Data ";
            else
                loc_LogTopic = "Lot End ";
            int OrderActive = 0;
            if (PLCCom.PLCActive == false)
                return;

            try
            {
                var PLCLotEnd = PLCCom.GetPLCVariable(FieldPLCs.PC_LotEnd_Started);
                if (PLCLotEnd.Item1 == true)
                {
                    if ((bool)PLCLotEnd.Item2 == true)
                    {

                        var LotEndFeedData = PLCCom.GetLotEndFeedData((double)Running_odm.SheetPerBundle, RobotModel);
                        if (RobotModel == 1)
                        {
                            var RobotData = ABBCom.ReadABBValue(ABBFeedFields.PC_AmountOfPlacedBundles.ToString());
                            if (RobotData.Item1 == true)
                                LotEndFeedData.AmountOfPlacedBundles = (int)ABBCom.ConvertABBtoCValue(RobotData.Item2, "int");
                        }

                        //Get Material_No and ID of current running Order
                        var PCOrder = tblOrder_GetOrderKeyByState(OrderState.Using);
                        if (PCOrder == null)
                        {
                            string buf_Msg = "PLC has requested to Lot-End this job.\n" +
                                                "But there is no current running Order.";
                            string buf_Topic = "Error! - No Running Order to perform Lot-End.";
                            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log.WriteLog("Error! - Cannot Lot End. Cannot find running order.", LogType.Fail);
                            return;
                        }
                        bool Updated = false;
                        //Update Master
                        //if (flag_ResetData ||
                        //if (double.IsNaN(LotEndFeedData.SheetHeight))
                        //{
                        //    LotEndFeedData.SheetHeight = 0;
                        //}

                        Log.WriteLog("<Notify> - Update Masterdata > SheetHeight:" + LotEndFeedData.SheetHeight + " PlacingMode:" + LotEndFeedData.PlacingMode, LogType.Notes);

                        //MessageBox.Show("Update tblOrder", "Update tblOrder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Update New Sheet Height & Soft Placing Option to tblOrder
                        Query = string.Format(@"UPDATE 
                                                            tblOrder 
                                                        SET FoldWork_T = {1},
                                                            HeightRatio = {2},
                                                            ExtraPickDepth = {3},
                                                            AntiBounce = {4},
                                                            SQ_Extra = {5},
                                                            PlacingMode = {6},
                                                            DischargeMode = {7},
                                                            Amount = {8},
                                                            OrderState = {9}
                                                        WHERE
                                                            ID = {0}",
                                                        PCOrder.OrderID,                        //  0
                                                        LotEndFeedData.SheetHeight,             //  1
                                                        LotEndFeedData.HeightRatio,             //  2
                                                        LotEndFeedData.ExtraPickDepth ? 1 : 0,  //  3
                                                        LotEndFeedData.STKAntiBounce ? 1 : 0,   //  4
                                                        LotEndFeedData.SQExtra ? 1 : 0,         //  5
                                                        LotEndFeedData.PlacingMode,             //  6
                                                        LotEndFeedData.DischargeMode,           //  7
                                                        LotEndFeedData.AmountOfPlacedBundles,   //  8
                                                        flag_ResetData ? 0 : 3);                 //9
                        // If Updated properly, Update New Sheet Height & Soft Placing Option to tblMaster
                        Updated = Sql.ExcSQL(Query);
                        if (Updated == true)
                        {
                            //MessageBox.Show("Update tblMaster", "Update tblMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Query = string.Format(@"UPDATE 
                                                                tblMaster 
                                                            SET FoldWork_T = {1},
                                                                HeightRatio = {2},
                                                                ExtraPickDepth = {3},
                                                                AntiBounce = {4},
                                                                SQ_Extra = {5},
                                                                PlacingMode = {6},
                                                                DischargeMode = {7}
                                                            WHERE
                                                                Material_No = '{0}'",
                                                            PCOrder.MaterialNo,                     //  0
                                                            LotEndFeedData.SheetHeight,             //  1
                                                            LotEndFeedData.HeightRatio,             //  2
                                                            LotEndFeedData.ExtraPickDepth ? 1 : 0,  //  3
                                                            LotEndFeedData.STKAntiBounce ? 1 : 0,   //  4
                                                            LotEndFeedData.SQExtra ? 1 : 0,         //  5
                                                            LotEndFeedData.PlacingMode,             //  6
                                                            LotEndFeedData.DischargeMode);          //  7
                            Updated = Sql.ExcSQL(Query);
                            if (Updated == false)
                            {
                                string buf_Topic = "Error! - During " + loc_LogTopic;
                                string buf_Msg = "Cannot update data to database with OrderID: " + PCOrder.OrderID;
                                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log.WriteLog("Error! - During " + loc_LogTopic + ". Cannot update data to tblMaster with OrderID: " + PCOrder.OrderID, LogType.Fail);
                            }
                        }
                        else
                        {
                            string buf_Topic = "Error! - During " + loc_LogTopic;
                            string buf_Msg = "Cannot update data to database with OrderID: " + PCOrder.OrderID;
                            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log.WriteLog("Error! - During " + loc_LogTopic + ". Cannot update data to tblOrder with OrderID: " + PCOrder.OrderID, LogType.Fail);
                        }

                        string dFormat = "yyyy-MM-dd HH:mm:ss";
                        Query = string.Format(@"UPDATE 
                                                tblOrder 
                                            SET Amount = {1},
                                                CompletedDate = '{2}'
                                            WHERE
                                                ID = {0}",
                                                PCOrder.OrderID,                                        //0
                                                LotEndFeedData.AmountOfPlacedBundles,                   //1
                                                DateTime.Now.ToString(dFormat)//,                         //2
                                                                              //_RBData.GetFirstPlaceBundleTime().ToString(dFormat),    //3
                                                                              //AvgUsedTime,                                            //4
                                                                              //_RBData.GetTotalPallet(),                               //5
                                                                              //_RBData.GetTotalWorkingTime(),                          //6
                                                                              //_RBData.GetActualSpeed(AvgUsedTime, SheetPerPallet)     //7

                                                      //FirstPlaceBundleTime = '{3}',
                                                      //AvgUsedTime = '{4}',
                                                      //TotalPallet = '{5}',
                                                      //TotalSheet = (PieceBun * {1}),
                                                      //TotalWorkingTime = '{6}',
                                                      //AvgSpeed = '{7}'
                                                      );
                        bool CanUpdate = Sql.ExcSQL(Query);

                        if (CanUpdate == false)
                        {
                            string buf_Topic = "Error! - During " + loc_LogTopic;
                            string buf_Msg = "Cannot update report data to database, OrderID:" + PCOrder.OrderID;
                            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log.WriteLog("Error! - During " + loc_LogTopic + ". Cannot update report data to tblOrder.", LogType.Fail);
                            OrderGrid_UpdateRowColor();
                            return;
                        }
                        else
                        {
                            var SetPLCLotEndAck = PLCCom.SetPLCVariable(FieldPLCs.PC_LotEnd_Ack, true);

                            if (!SetPLCLotEndAck.Item1.Equals(true))
                            {
                                Msg = loc_LogTopic + "Error! - can't write PC_LotEnd_Ack = true to PLC. of OrderID:" + PCOrder.OrderID.ToString();
                                string buf_Topic = "Error! - During " + loc_LogTopic;
                                string buf_Msg = "Cannot write to PLC data [" + FieldPLCs.PC_LotEnd_Ack + "]";
                                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log.WriteLog("Error! - During " + loc_LogTopic + ". Cannot write to PLC data [" + FieldPLCs.PC_LotEnd_Ack + "]", LogType.Fail);
                                return;
                            }

                            Log.WriteLog("Complete Sequence! - Successfully LotEnd (" + loc_LogTopic + "). OrderID:" + PCOrder.OrderID.ToString(), LogType.Success);
                            if (flag_ResetData)
                            {
                                flag_ResetData = false;
                            }
                            else
                            {
                                flag_ResetData = false;
                                OrderGrid_RemoveFinishOrder(PCOrder.OrderID.ToString());
                            }
                            OrderGrid_UpdateRowColor();
                            Running_ptm = null;
                            Latest_FeedData.Reset_FeedData();
                            if (!tmrConfirmStart.Enabled)
                                tmrConfirmStart.Start();

                            //tmrRobotFeed.Stop();
                            IsOnGetRobotFeed = false;
                            //Log.WriteLog("Timer Robot Feed was stopped.", LogType.Notes);
                        }

                        tmrLotEnd.Stop(); //04-09-2020 Update SEQ Timer @Beer                        
                        //Log.WriteLog("LotEnd Stop Trick", LogType.Notes);

                        DashBoard_Clear();
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(loc_LogTopic + e.Message, LogType.Fail);

                var SetPLCLotEndAck = PLCCom.SetPLCVariable(FieldPLCs.PC_LotEnd_Ack, true);
                if (!SetPLCLotEndAck.Item1.Equals(true))
                {
                    Msg = loc_LogTopic + "Error! can't write PC_LotEnd_Ack = true to PLC.";
                    MessageBox.Show(Msg, loc_LogTopic + "Order", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.WriteLog(Msg, LogType.Fail);
                    return;
                }
                else
                {
                    OrderGrid_RemoveFinishOrder(OrderActive.ToString());
                    OrderGrid_UpdateRowColor();
                    Running_ptm = null;
                    if (!tmrConfirmStart.Enabled)
                        tmrConfirmStart.Start();
                }
            }

        }
        public OrderKeyModel tblOrder_GetOrderKeyByState(OrderState State)
        {
            OrderKeyModel result = new OrderKeyModel();

            try
            {
                string Query = "SELECT [Material_No], [ID] FROM tblOrder WHERE OrderState = @OrderState";
                Query = Query.Replace("@OrderState", Convert.ToString((int)State));
                DataTable dt = Sql.GetDataTableFromSql(Query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    result.MaterialNo = dt.Rows[0]["Material_No"].ToString();
                    result.OrderID = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Can not get OrderKey " + e.Message, LogType.Fail);
            }

            return result;
        }
        private void DashBoard_Clear()
        {
            //Finger
            if (!lblFingerRequire.InvokeRequired)
            {
                lblFingerRequire.Text = "";
            }
            else
                lblFingerRequire.Invoke(new Action(() =>
                {
                    lblFingerRequire.Text = "";
                }));

            //PalletSide
            lblPalletType.Text = "";
            lblSizePallet.Text = "";

            //Material No
            txbDispMatNo.Text = "No Selected Order";

            //Bundle Per Grip
            txbDispBDPerGrip.Text = "";

            //TieSheet Layer

            // Robot Speed
            if (!lblSpeed.InvokeRequired)
                lblSpeed.Text = "";
            else
                lblSpeed.Invoke(new Action(() =>
                {
                    lblSpeed.Text = "";
                }));

            // Bundle Count
            if (!txbDispBDCount.InvokeRequired)
                txbDispBDCount.Text = "";
            else
                txbDispBDCount.Invoke(new Action(() =>
                {
                    txbDispBDCount.Text = "";
                }));

            // SO Index
            if (!txbDispSO.InvokeRequired)
                txbDispSO.Text = "";
            else
                txbDispSO.Invoke(new Action(() =>
                {
                    txbDispSO.Text = "";
                }));

            // SO Bundle Count
            if (!txbDispSOBD.InvokeRequired)
                txbDispSOBD.Text = "";
            else
                txbDispSOBD.Invoke(new Action(() =>
                {
                    txbDispSOBD.Text = "";
                }));

            // Layer Count
            if (!txbDispLYCount.InvokeRequired)
                txbDispLYCount.Text = "";
            else
                txbDispLYCount.Invoke(new Action(() =>
                {
                    txbDispLYCount.Text = "";
                }));

            // TieSheet Width Size
            if (!txbDispTSW.InvokeRequired)
                txbDispTSW.Text = "";
            else
                txbDispTSW.Invoke(new Action(() =>
                {
                    txbDispTSW.Text = "";
                }));

            // TieSheet Length Size
            if (!txbDispTSL.InvokeRequired)
                txbDispTSL.Text = "";
            else
                txbDispTSL.Invoke(new Action(() =>
                {
                    txbDispTSL.Text = "";
                }));

            // TieSheet Layer 1 - 10
            if (!txbTSLY1_10.InvokeRequired)
                txbTSLY1_10.Text = "";
            else
                txbTSLY1_10.Invoke(new Action(() =>
                {
                    txbTSLY1_10.Text = "";
                }));

            // TieSheet Layer 11 - 20
            if (!txbTSLY11_20.InvokeRequired)
                txbTSLY11_20.Text = "";
            else
                txbTSLY11_20.Invoke(new Action(() =>
                {
                    txbTSLY11_20.Text = "";
                }));

            // TieSheet Layer 21 - 30
            if (!txbTSLY21_30.InvokeRequired)
                txbTSLY21_30.Text = "";
            else
                txbTSLY21_30.Invoke(new Action(() =>
                {
                    txbTSLY21_30.Text = "";
                }));

            // Step Count
            if (!chkbxDispPat1.InvokeRequired)
            {
                chkbxDispPat1.ForeColor = Color.White;
                chkbxDispPat1.Checked = false;
            }
            else
                chkbxDispPat1.Invoke(new Action(() =>
                {
                    chkbxDispPat1.ForeColor = Color.White;
                    chkbxDispPat1.Checked = false;
                }));

            if (!chkbxDispPat2.InvokeRequired)
            {
                chkbxDispPat2.ForeColor = Color.White;
                chkbxDispPat2.Checked = false;
            }
            else
                chkbxDispPat2.Invoke(new Action(() =>
                {
                    chkbxDispPat2.ForeColor = Color.White;
                    chkbxDispPat2.Checked = false;
                }));

            if (!chkbxDispPat3.InvokeRequired)
            {
                chkbxDispPat3.ForeColor = Color.White;
                chkbxDispPat3.Checked = false;
            }
            else
                chkbxDispPat3.Invoke(new Action(() =>
                {
                    chkbxDispPat3.ForeColor = Color.White;
                    chkbxDispPat3.Checked = false;
                }));

            if (!chkbxDispPat4.InvokeRequired)
            {
                chkbxDispPat4.ForeColor = Color.White;
                chkbxDispPat4.Checked = false;
            }
            else
                chkbxDispPat4.Invoke(new Action(() =>
                {
                    chkbxDispPat4.ForeColor = Color.White;
                    chkbxDispPat4.Checked = false;
                }));

            // Progress bar percent text
            if (!lblPalletPercent.InvokeRequired)
            {
                lblPalletPercent.Text = string.Format("Pallet: ");
                lblPalletPercent.Tag = 0;
            }
            else
                lblPalletPercent.Invoke(new Action(() =>
                {
                    lblPalletPercent.Text = string.Format("Pallet: ");
                    lblPalletPercent.Tag = 0;
                }));

            // Progress Bar under 3D Picture
            if (!pgbPlacedBundle.InvokeRequired)
            {
                pgbPlacedBundle.Maximum = 1;
                pgbPlacedBundle.Value = 0;
            }
            else
                pgbPlacedBundle.Invoke(new Action(() =>
                {
                    pgbPlacedBundle.Maximum = 1;
                    pgbPlacedBundle.Value = 0;
                }));

            // 2D Bundle
            for (int i = pnlSimRun.Controls.Count - 1; i >= 0; i--)
            {
                Control ctrl = pnlSimRun.Controls[i];
                pnlSimRun.Controls.Remove(ctrl);
            }

            // 3D Bundle
            pic3DOrder.Image = null;
        }

        private void TmrConfirmStart_Tick(object sender, EventArgs e)
        {
            Check_PLC_ConfirmStart();
        }
        private void Check_PLC_ConfirmStart()
        {
            try
            {
                string Query;
                var PLCConfirmStart = PLCCom.GetPLCVariable(FieldPLCs.PC_Start_Done);
                if (PLCConfirmStart.Item1.Equals(true)) // Check if GetPLCVariable properly complete
                {
                    if (PLCConfirmStart.Item2.Equals(true)) // Check for the value of the FieldPLC
                    {

                        Query = "UPDATE tblOrder SET OrderState = 2 WHERE OrderState = 1";
                        bool CanUpdate = Sql.ExcSQL(Query);
                        if (CanUpdate == false)
                        {
                            string buf_Msg = "ID4P can not change the state of the Order to Running.\n" +
                                                "it might be that:\n" +
                                                " - Something happened at the Database.\n" +
                                                " - You haven't send the data to the Palletizing Robot yet.";
                            string buf_Topic = "Error! - Cannot change Order State.";
                            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log.WriteLog("Confirm Start error!!! can not save order state to Database.", LogType.Fail);
                            return;
                        }
                        else
                        {
                            PLCCom.SetPLCVariable(FieldPLCs.PC_Start_Done, false);
                            Log.WriteLog("Confirm Start complete.", LogType.Success);
                            OrderGrid_ImportOrderData();
                            tmrConfirmStart.Stop();
                            tmrLotEnd.Start();
                            ConfirmStartError = false;
                            OrderGrid_FindOrderByState(OrderState.Using, true, true, false);
                        }
                    }
                    else
                    {
                        if (ConfirmStartError == true)
                            return;

                        var IsResetError = PLCCom.GetPLCVariable(FieldPLCs.PC_CommuError);
                        if (IsResetError.Item1.Equals(true))
                        {
                            if (IsResetError.Item2.Equals(true))
                            {
                                ConfirmStartError = true;
                                tmrConfirmStart.Stop();
                                Log.WriteLog("[Confirm Start]PLC or Robot Communication Error!!!", LogType.Fail);
                                UICls.ShowWaiting(false);
                                BlockSendData_byResetStatus(true, "Start Error!");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message, LogType.Fail);
            }
        }
        #endregion

        #region Data Grid Button
        private void DgvOrderList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex].Name != "ColDelete" && senderGrid.Columns[e.ColumnIndex].Name != "Amount")
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "ColDelete")
            {
                if (senderGrid.Columns["ColDelete"] is DataGridViewButtonColumn && e.RowIndex >= 0)
                {
                    DataGridViewRow rOrder = dgvOrderList.SelectedRows[0];
                    string ID = rOrder.Cells["ID"].Value.ToString();
                    string State = rOrder.Cells["OrderState"].Value.ToString();
                    if (State != "0")
                    {
                        string buf_Msg = "The System can not delete this order.\n" +
                                            "This order is already uploaded to the Robot.";
                        string buf_Topic = "Error! - Can not delete this order.";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    DeleteOrder(ID);
                }

            }
            //Top Edit for show split mode 22-07-2020
            else
            {
                if (senderGrid.Columns["Amount"] is DataGridViewButtonColumn && e.RowIndex >= 0)
                {
                    DataGridViewRow rOrder = dgvOrderList.SelectedRows[0];
                    string ID = rOrder.Cells["ID"].Value.ToString();
                    string MatNo = rOrder.Cells["MaterialNo"].Value.ToString();
                    int OrderState = Convert.ToInt32(rOrder.Cells["OrderState"].Value);
                    int Sheetperbundle = Convert.ToInt32(rOrder.Cells["SheetPerBundle"].Value.ToString());
                    using (frmSplitSheet fSplit = new frmSplitSheet())
                    {
                        fSplit.OrderID_Split = ID;
                        fSplit.MatNo_Split = MatNo;
                        fSplit.SheetPerBundle = Sheetperbundle;
                        fSplit.OrderState = OrderState;
                        fSplit.ShowDialog();
                    }
                    OrderGrid_ImportOrderData();
                }
                else
                {
                }
            }
            //--------------------------------------------

        }

        private void DeleteOrder(string ID)
        {
            string buf_Msg = "Do you want to delete this order?";
            string buf_Topic = "Delete Order Confirmation.";
            DialogResult drConfirm = MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (drConfirm != DialogResult.Yes)
                return;

            String Query = string.Format(@"DELETE FROM tblOrder WHERE ID = '{0}'", ID);
            bool IsComplete = Sql.ExcSQL(Query);
            if (IsComplete == true)
            {
                buf_Msg = "The Order has been deleted.";
                buf_Topic = "Success! - The Order has been deleted.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
                OrderGrid_ImportOrderData();
            }
            else
            {
                buf_Msg = "Can not delete the Order.\n"+
                            "try again or check the Database.";
                buf_Topic = "Error! - Can not delete the Order.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvOrderList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow rOrder = dgvOrderList.SelectedRows[0];
                string _MaterialNo = rOrder.Cells["MaterialNo"].Value.ToString();
                var _fParent = UICls.FindOpenForm("frmMain");
                if (_fParent != null)
                {
                    frmMain fParent = (frmMain)_fParent;
                    fParent.ShowProductForm(_MaterialNo);
                }
            }
        }
        #endregion

        #region Squaring Display
        private void rdoPressSquaring_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoPressSquaring.Checked)
            {
                if (grpbxFeedData.Text == "Selected Order")
                {
                    if (dgvOrderList.SelectedRows != null && dgvOrderList.SelectedRows.Count == 1)
                    {
                        DataGridViewRow buf_SelectedRow = dgvOrderList.SelectedRows[0];
                        Get_OrderData(buf_SelectedRow);
                    }
                }
                else if (grpbxFeedData.Text == "Uploaded Order")
                    OrderGrid_FindOrderByState(OrderState.Buffer, true);
                else if (grpbxFeedData.Text == "Running Order")
                    OrderGrid_FindOrderByState(OrderState.Using, true);
            }
        }
        private void rdoOpenSquaring_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoOpenSquaring.Checked)
            {
                if (grpbxFeedData.Text == "Selected Order")
                {
                    if (dgvOrderList.SelectedRows != null && dgvOrderList.SelectedRows.Count == 1)
                    {
                        DataGridViewRow buf_SelectedRow = dgvOrderList.SelectedRows[0];
                        Get_OrderData(buf_SelectedRow);
                    }
                }
                else if (grpbxFeedData.Text == "Uploaded Order")
                    OrderGrid_FindOrderByState(OrderState.Buffer, true);
                else if (grpbxFeedData.Text == "Running Order")
                    OrderGrid_FindOrderByState(OrderState.Using, true);
            }
        }
        #endregion

        private void FrmOrder_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnEditTS_Click(object sender, EventArgs e)
        {
            if (frmTieSheet == null)
            {
                frmTieSheet = new frmTiesheet();
                frmTieSheet.ShowDialog();
            }
            else
            {
                frmTieSheet.ShowDialog();
            }

            ClientData.Selected_TSWidth = frmTieSheet.ClientDataTS.Selected_TSWidth;
            ClientData.Selected_TSLength = frmTieSheet.ClientDataTS.Selected_TSLength;
            lblTieSheetSize.Text = frmTieSheet.ClientDataTS.Selected_TSText;
        }

        private void btnChangeToCurOrder_Click(object sender, EventArgs e)
        {
            DataGridViewRow buf_SelectedRow = OrderGrid_FindOrderByState(OrderState.Buffer, true);
            if (buf_SelectedRow != null)
            {
                Get_OrderData(buf_SelectedRow);
            }
            else
            {
                buf_SelectedRow = OrderGrid_FindOrderByState(OrderState.Using, true);
                if (buf_SelectedRow != null)
                {
                    Get_OrderData(buf_SelectedRow);
                }
                else
                {
                    string buf_Msg = "No Uploaded Order or Running Order has been found.\n\n" +
                                        "To uploaad new Order:\n" +
                                        " - Click on desired DataRow to select it\n" +
                                        " - Click Send Data\n\n" +
                                        "* If you can not do that, try Reset the System:\n" +
                                        "   - Press EStop button at the Robot.\n" +
                                        "   - Click Safety Reset button at the HMI box at the Robot.\n" +
                                        "   - Locked the door at the Robot.\n" +
                                        "   - Click Reset Data then try sending the data again.\n\n\n" +
                                        "The System will disable this button until New Data is successfully sent.";
                    string buf_Topic = "Warning! - No data Uploaded or Running Order.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnChangeToCurOrder.Enabled = false;
                }
            }
        }

        private void txbCurBDCount_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblCurBDCount_Click(object sender, EventArgs e)
        {

        }

        public void Msg_HowToChangePallet()
        {
            String buf_Msg = "If you want to change the Pallet of this Order,\n" +
                                "Please follow these steps:\n\n" +
                                " 1. Double click on the Data Row you want.\n" +
                                " 2. On the right side you will see the Pallet Type panel.\n" +
                                " 3. Click Edit button inside the Pallet Type panel.\n" +
                                " 4. Select the Pallet Type you want.\n" +
                                " 5. Click Save button in Pallet Type Setting window.\n" +
                                " 6. Click Save button on the right-buttom corner.";
            String buf_Topic = "How to change Pallet Properties.";
            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void Msg_HowSetGripperFinger()
        {
            String buf_Msg = "If you want to Set the Gripper Finger of this Order,\n" +
                                "Please follow these steps:\n\n" +
                                " 1. Double click on the Data Row you want.\n" +
                                " 2. On the right side you will see the Gripper Finger checkbox.\n" +
                                " 3. Click to check/uncheck it to set/reset the use of Gripper Finger";
            String buf_Topic = "How to set Gripper Finger.";
            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lblPalletType_DoubleClick(object sender, EventArgs e)
        {
            Msg_HowToChangePallet();
        }

        private void lblSizePallet_DoubleClick(object sender, EventArgs e)
        {
            Msg_HowToChangePallet();
        }

        private void lblPallet_Title_DoubleClick(object sender, EventArgs e)
        {
            Msg_HowToChangePallet();
        }

        private void lblFinger_Title_DoubleClick(object sender, EventArgs e)
        {
            Msg_HowSetGripperFinger();
        }

        private void picGripperFinger_DoubleClick(object sender, EventArgs e)
        {
            Msg_HowSetGripperFinger();
        }

        private void lblFingerRequire_DoubleClick(object sender, EventArgs e)
        {
            Msg_HowSetGripperFinger();
        }

        private void btnDoorUnlock_Click(object sender, EventArgs e)
        {
            if (!PLCCom.CheckPLCIsActive(_PLCIP))
            {
                UICls.ShowWaiting(false);
                string buf_Msg = "PLC is disconnected.\n" +
                                    "Please check PLC connection.\n" +
                                    "Then try sending Door Unlock Command again.";
                string buf_Topic = "Error! - Cannot connect to PLC.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_Door_OpenCmd, true);
        }

        private void btnDoorLock_Click(object sender, EventArgs e)
        {
            if (!PLCCom.CheckPLCIsActive(_PLCIP))
            {
                UICls.ShowWaiting(false);
                string buf_Msg = "PLC is disconnected.\n" +
                                    "Please check PLC connection.\n" +
                                    "Then try sending Door Lock Command again.";
                string buf_Topic = "Error! - Cannot connect to PLC.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_Door_LockCmd, true);
        }

        private void btnConfirmStart_Click(object sender, EventArgs e)
        {
            if (!PLCCom.CheckPLCIsActive(_PLCIP))
            {
                string buf_Msg = "PLC is disconnected.\n" +
                                    "Please check PLC connection.\n" +
                                    "Then try sending Confirm Start Command again.";
                string buf_Topic = "Error! - Cannot connect to PLC.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (RobotModel == 1)
            {
                if (!ABBCom.CheckRobotIsActive(_RobotIP))
                {
                    string buf_Msg = "Robot is disconnected.\n" +
                                        "Please check Robot connection.\n" +
                                        "Then try sending Confirm Start Command again.";
                    string buf_Topic = "Error! - Cannot connect to the Robot.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (!PLCCom.CheckRobotIsActive())
                {
                    string buf_Msg = "Robot is disconnected.\n" +
                                        "Please check Robot connection.\n" +
                                        "Then try sending Confirm Start Command again.";
                    string buf_Topic = "Error! - Cannot connect to the Robot.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var buf_chkDoorLock = PLCCom.GetPLCVariable(FieldPLCs.PC_Door_Status, "int");
            if (buf_chkDoorLock.Item1.Equals(false))
            {
                string buf_Msg = "Can not read PC Signal.\n" +
                                    "Please check PLC Connection or Sysmac Gateway Console\n" +
                                    "Then try sending Confirm Start Command again.";
                string buf_Topic = "Error! - Cannot connect to PLC.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblDoorStatusDisp.ForeColor = Color.Red;
                lblDoorStatusDisp.BackColor = Color.Yellow;
                lblDoorStatusDisp.Text = "Cannot read PLC signal.";
                return;
            }
            else
            {
                lblDoorStatusDisp.ForeColor = Color.WhiteSmoke;
                lblDoorStatusDisp.BackColor = Color.FromArgb(64, 64, 64);
                switch ((int)buf_chkDoorLock.Item2)
                {
                    case 2:
                        lblDoorStatusDisp.Text = "Door is Locked.";
                        break;

                    default:
                        string buf_Msg = "Please lock the Door and try again.\n" +
                                            "Then try sending Confirm Start Command again.";
                        string buf_Topic = "Error! - Cannot Start the system (Door is not locked).";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }
            }

            //if (!tmrConfirmStart.Enabled)
            //{
            //    string buf_Msg = "There is no Order at the Robot System.\n" +
            //                        "Please select the Order from the Data Grid.\n" +
            //                        "and click send data and try again.";
            //    string buf_Topic = "Error! - No Order in Buffer.";
            //    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            var PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_Start_Cmd, true);
        }

        private void btnHalfFinish_Click(object sender, EventArgs e)
        {
            if (!PLCCom.CheckPLCIsActive(_PLCIP))
            {
                string buf_Msg = "PLC is disconnected.\n" +
                                    "Please check PLC connection.\n" +
                                    "Then try sending Half Finish Command again.";
                string buf_Topic = "Error! - Cannot connect to PLC.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (RobotModel == 1)
            {
                if (!ABBCom.CheckRobotIsActive(_RobotIP))
                {
                    string buf_Msg = "Robot is disconnected.\n" +
                                        "Please check Robot connection.\n" +
                                        "Then try sending Half Finish Command again.";
                    string buf_Topic = "Error! - Cannot connect to the Robot.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (!PLCCom.CheckRobotIsActive())
                {
                    string buf_Msg = "Robot is disconnected.\n" +
                                        "Please check Robot connection.\n" +
                                        "Then try sending Half Finish Command again.";
                    string buf_Topic = "Error! - Cannot connect to the Robot.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var buf_chkDoorLock = PLCCom.GetPLCVariable(FieldPLCs.PC_Door_Status, "int");
            if (buf_chkDoorLock.Item1.Equals(false))
            {
                string buf_Msg = "Can not read PC Signal.\n" +
                                    "Please check PLC Connection or Sysmac Gateway Console\n" +
                                    "Then try sending Half Finish Command again.";
                string buf_Topic = "Error! - Cannot connect to PLC.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblDoorStatusDisp.ForeColor = Color.Red;
                lblDoorStatusDisp.BackColor = Color.Yellow;
                lblDoorStatusDisp.Text = "Cannot read PLC signal.";
                return;
            }
            else
            {
                lblDoorStatusDisp.ForeColor = Color.WhiteSmoke;
                lblDoorStatusDisp.BackColor = Color.FromArgb(64, 64, 64);
                switch ((int)buf_chkDoorLock.Item2)
                {
                    case 2:
                        lblDoorStatusDisp.Text = "Door is Locked.";
                        break;

                    default:
                        string buf_Msg = "Please lock the Door and try again.\n" +
                                            "Then try sending Confirm Start Command again.";
                        string buf_Topic = "Error! - Cannot Half Finish (Door is not locked).";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }
            }
            var PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_HalfFin_Cmd, true);
        }

        private void btnLotEnd_Click(object sender, EventArgs e)
        {
            if (!PLCCom.CheckPLCIsActive(_PLCIP))
            {
                string buf_Msg = "PLC is disconnected.\n" +
                                    "Please check PLC connection.\n" +
                                    "Then try sending Lot End Command again.";
                string buf_Topic = "Error! - Cannot connect to PLC.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (RobotModel == 1)
            {
                if (!ABBCom.CheckRobotIsActive(_RobotIP))
                {
                    string buf_Msg = "Robot is disconnected.\n" +
                                        "Please check Robot connection.\n" +
                                        "Then try sending Lot End Command again.";
                    string buf_Topic = "Error! - Cannot connect to the Robot.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (!PLCCom.CheckRobotIsActive())
                {
                    string buf_Msg = "Robot is disconnected.\n" +
                                        "Please check Robot connection.\n" +
                                        "Then try sending Lot End Command again.";
                    string buf_Topic = "Error! - Cannot connect to the Robot.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }


            var buf_chkDoorLock = PLCCom.GetPLCVariable(FieldPLCs.PC_Door_Status, "int");
            if (buf_chkDoorLock.Item1.Equals(false))
            {
                string buf_Msg = "Can not read PC Signal.\n" +
                                    "Please check PLC Connection or Sysmac Gateway Console\n" +
                                    "Then try sending Lot End Command again.";
                string buf_Topic = "Error! - Cannot connect to PLC.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblDoorStatusDisp.ForeColor = Color.Red;
                lblDoorStatusDisp.BackColor = Color.Yellow;
                lblDoorStatusDisp.Text = "Cannot read PLC signal.";
                return;
            }
            else
            {
                lblDoorStatusDisp.ForeColor = Color.WhiteSmoke;
                lblDoorStatusDisp.BackColor = Color.FromArgb(64, 64, 64);
                switch ((int)buf_chkDoorLock.Item2)
                {
                    case 2:
                        lblDoorStatusDisp.Text = "Door is Locked.";
                        break;

                    default:
                        string buf_Msg = "Please lock the Door and try again.\n" +
                                            "Then try sending Lot End Command again.";
                        string buf_Topic = "Error! - Cannot Lot End (Door is not locked).";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }
            }
            var PLCResp = PLCCom.SetPLCVariable(FieldPLCs.PC_LotEnd_Cmd, true);
        }

    }

}
