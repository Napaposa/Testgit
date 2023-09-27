using System;
using System.CodeDom;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Linq;
using ABB.Robotics.Controllers.ConfigurationDomain;
using ATD_ID4P.Class;
using ATD_ID4P.Model;
using Newtonsoft.Json.Bson;

namespace ATD_ID4P
{
    public partial class frmProduct : Form
    {
        public frmProduct()
        {
            InitializeComponent();
        }
        // Test Leaffff
        private LogCls Log = new LogCls();
        //private ReadXmlConfigFile ReadParameterCfg = new ReadXmlConfigFile();
        private SqlCls Sql = new SqlCls();
        private Model.ClientDataModel ClientData = new Model.ClientDataModel();

        private patternCls GenPtnAuto = new patternCls();

        private PatternModel Current_ptm = new PatternModel();

        private PictureBox[] PBs = null;
        private PictureBox LastPB = null;

        private Laying[] OpenBundles;
        private int SQ_OPX = 0;
        private int SQ_OPY = 0;
        private int SQ_CLX = 0;
        private int SQ_CLY = 0;
        public string pMaterialNo = "";
        private Image ImagePallet = null;
        public bool InvalidOrder = false;
        public int HeightRatio = 100;
        public int HeightRatio_Master = 100;
        public int BundleHeight_Master = -1;

        private bool flag_UpdatingUI = false;
        private bool flag_PM2GetData = false;
        private bool flag_PM3GetData = false;
        private bool flag_unCF_boxChange = false;
        private bool flag_unCF_fluteChange = false;
        private bool flag_unCF_bdsizeChange = false;
        private bool flag_unCF_palletChange = false;
        private bool flag_unCF_fingerChange = false;
        private bool flag_GotPatternOrder = false;
        private bool flag_SwapLW_Pass = true;
        private bool flag_SpecialRot_Pass = true;
        private bool flag_CannotRotate = false;
        private bool flag_InGetPatternOrder = false;

        //== flag for Cancel Object new value in checkchange
        private bool flag_FingerUse_ReturnValue = false;
        private bool flag_ModSWPat_ReturnValue = false;
        private bool flag_FixBDFace_ReturnValue = false;
        private bool flag_RotatePat_ReturnValue = false;
        private bool flag_SwapLW_ReturnValue = false;
        private bool flag_SpeicalRot_ReturnValue = false;
        private bool flag_BDHeight_ReturnValue = false;

        private int PlacingMode = 0;
        private int DischargeMode = 0;

        //private double palletHigh { get; set; } //Pallet heig size
        private double Efficiency { get; set; } = 0.00; //Percen efficency
        private double Fold_height { get; set; } = 0; //Folding height  
        private double Fold_height_Caled { get; set; } = 0; //Calculated Folding height from system     
        private int _fWidthFromCutSheetWid { get; set; } = 0; //@Top 17-07-2020       

        private Int16 StackHeight { get; set; } = 0;    //Complete StackHeight on Pallet
        private Int16[] _fLengArry = new Int16[2]; //Fold length size x1, x2
        private Int16[] _fWidthArry = new Int16[5]; //Fold width size x1, x2, x3, x4, x5

        private frmPalletType frmPallet = new frmPalletType(); //Top@
        private palletsCls pallets = new palletsCls();

        //*rename private int LayerPerimeterLength = 0;
        //*rename private int LayerPerimeterWidth = 0;
        private int LayerClosePeriLength = 0;
        private int LayerClosePeriWidth = 0;
        private string PMTPattern = "";
        //private string[] DisablePattern = new string[] { "C0661", "I0752", "C0919",
        //                                                "C1243", "C1427", "S1688", "C2054", "C2446", "S3216" };

        // Forced Disable by Default
        private string[] DisablePattern = new string[] { "C0661", "I0752", "C0919",
                                                        "C1427", "S1688", "C2054", "C2446", "S3216" };
        private decimal CalBundleHeight = 0;

        //private PictureBox[] PBs = new PictureBox[] { picC0111, picC0221, picC0331, picC0422, picC0441, picC0632, picC0661, picC0824, picC0919, picC0933
        //                                            ,picC1243, picC1427, picC2054, picC2446
        //                                            ,picI0321, picI0532, picI0642, picI0734, picI0752
        //                                            ,picS0422, picS0844, picS1266, picS1688, picS3216
        //                                        };
        

        // * When form is load from method Show() *
        //  -   Load Client Data from .json file to ClientDataPallet
        //  -   Set Pallet Parameter according to frmPallet.FlgPalletTypeSet
        //  -   Check if 'Material No.' Text Box is not empty
        //      -   If empty Alarm
        //      -   If not empty
        //          -   Get DataTable from tblMaster that match the value in 'Material No' Text Box
        //              -   If DataTable is not Empty
        //                  -   fill object value and text with data from DataTable (and update frmPallet.FlgPalletTypeSet and call PalletSetDefault() again)
        //                  -   Run each pattern in pattern grid
        //                      -   check if this is a Forced Disable Pattern
        //                      -   Check if Swap(LxW) is tick
        //                      -   Breakdown Pattern Detail (S0422 -> 'S', '04', '22'
        //                      -   Set Bundle Width and Length according to Bundlw Size NumBox and SwapSide value
        //                      -   Set Bundle Gap according to Finger CheckBox (value are from setting file)
        //                      -   Set Pallet Width and Length according to Rotated Pallet CheckBox
        //                      -   Check if Special Rotate is tick (Rotate(W>L) Check Box)
        //                      -   Get Placement info
        //                          -   Clear 'ProcessingPattern' value and copy it to 'LayerPattern' variable
        //                          -   Calculate total step of this pattern
        //                          -   Calculate each step of pattern
        private void frmProduct_Load(object sender, EventArgs e)
        {
            var buf_fMain = UICls.FindOpenForm("frmMain");
            if (buf_fMain != null)
            {
                frmMain fMain = (frmMain)buf_fMain;
                fMain.Disable_MainMenu(true, true, true, true);
            }
            SetDisplayMode();
            cmbBoxType.Items.AddRange(ClientData.LoadBoxStyleOption());
            cmbFluteType.Items.AddRange(ClientData.LoadFluteDataOption());
            
            pallets.Width = 1000;
            pallets.Length = 1200;
            pallets.Height = 150;
            pallets.Name = "Wooden";
            
            lblPalletType.Text = pallets.Name;
            lblSizePallet.Text = string.Format("{0}x{1}x{2}", pallets.Width, pallets.Length, pallets.Height);

            InitUI();

            if (Properties.Settings.Default.SquaringModel == -1)
            {
                chkbxSquaringUse.Enabled = false;
                chkbxSquaringUse.Checked = false;
            }

            if (!Properties.Settings.Default.Allow_SoftPlace)
            {
                rdoPlaceSoft.Enabled = false;
                rdoPlaceSoft.Checked = false;
            }

            if (!Properties.Settings.Default.Allow_FastPlace)
            {
                rdoPlaceFast.Enabled = false;
                rdoPlaceFast.Checked = false;
            }

            if (!Properties.Settings.Default.Allow_SlowDCSpd)
            {
                rdoDCSlow.Enabled = false;
                rdoDCSlow.Checked = false;
            }

            if (!Properties.Settings.Default.Allow_FastDCSpd)
            {
                rdoDCFast.Enabled = false;
                rdoDCFast.Checked = false;
            }

            if (!string.IsNullOrEmpty(pMaterialNo))
            {
                txbMatNO.Text = pMaterialNo;
                Loaddata();
            }
            else
            {
                //Update Default Weight @2020-12-25 by Beer.
                numBoxWeight.Value = Convert.ToDecimal(Properties.Settings.Default.Sheet_DefaultWeight);
            }
        }
        private void SetDisplayMode()
        {
            if (Properties.Settings.Default.UI_DarkMode == false)
            {
                this.BackColor = Color.White;
                pnlBundleSize.BackColor = Color.LightGray;
                palPatternMaster.BackColor = ColorTranslator.FromHtml("#b2c1cf");
                palPatternMaster.ForeColor = Color.Black;
                pnlOption.BackColor = Color.LightGray;

                grpbxMatNoOption.ForeColor = Color.Black;
                grpbxBDSizeOption.ForeColor = Color.Black;
                grpbxLayerToDisplay.ForeColor = Color.Black;

                lblPalletType.ForeColor = Color.Black;

                grpbxConfigOption.ForeColor = Color.Black;

                lblBoxType.ForeColor = Color.Black;
                lblFluteType.ForeColor = Color.Black;
                lblPalletTypeX.ForeColor = Color.Black;

                lblLastUpdate.ForeColor = Color.Black;
                lblSO.ForeColor = Color.Black;

                lblPtName.ForeColor = Color.Black;
                lblEfficiency.ForeColor = Color.Black;
                lblScore.ForeColor = Color.Black;
                chkbxSwapLW.ForeColor = Color.Black;
                chkbxSpecialRotate.ForeColor = Color.Black;

                grpbxSQDisplayOption.ForeColor = Color.Black;
                //*lblPatternName.ForeColor = Color.Black;

                lblBDPerGrip.ForeColor = Color.Black;
                chkbxTieSheet.ForeColor = Color.Black;
                chkbxTS_B4LastLY.ForeColor = Color.Black;
                chkbxTS_LowSQLayer.ForeColor = Color.Black;
                chkbxSquaringUse.ForeColor = Color.Black;
                lblBDPerGripUnit.ForeColor = Color.Black;
                lblTSLayerUnit.ForeColor = Color.Black;

                chkbxBtmSheet.ForeColor = Color.Black;
                chkbxTopSheet.ForeColor = Color.Black;

                chkbxFixBDFace.ForeColor = Color.Black;
                chkbxRotatePattern.ForeColor = Color.Black;
                chkbxModSWPat.ForeColor = Color.Black;
                chkbx1stSWPat.ForeColor = Color.Black;
                chkbx2ndSWPat.ForeColor = Color.Black;
                chkbx3rdSWPat.ForeColor = Color.Black;
                chkbx4thSWPat.ForeColor = Color.Black;
                lblPattern.ForeColor = Color.Black;
                chkbxFingerUse.ForeColor = Color.Black;

                grpbxPlaceMode.ForeColor = Color.Black;
                grpbxPlaceMode.BackColor = ColorTranslator.FromHtml("#c2c2c2");
                rdoPlaceSoft.ForeColor = Color.Black;
                rdoPlaceNormal.ForeColor = Color.Black;
                rdoPlaceFast.ForeColor = Color.Black;

                grpbxDCSpeed.ForeColor = Color.Black;
                grpbxDCSpeed.BackColor = ColorTranslator.FromHtml("#c2c2c2");
                rdoDCSlow.ForeColor = Color.Black;
                rdoDCNormal.ForeColor = Color.Black;
                rdoDCFast.ForeColor = Color.Black;

                grpbxPatMod.ForeColor = Color.Black;
                grpbxPalletType.ForeColor = Color.Black;
                grpbxConfigOption.BackColor = ColorTranslator.FromHtml("#c2c2c2");
                //*groupBox9.BackColor = ColorTranslator.FromHtml("#c2c2c2");
                grpbxPatMod.BackColor = ColorTranslator.FromHtml("#c2c2c2");
                foreach (Control ctrl in pnlOption.Controls)
                {
                    if (ctrl.GetType().ToString() == "System.Windows.Forms.Label")
                    {
                        Label Lb = (Label)ctrl;
                        Lb.ForeColor = Color.Black;
                    }
                }

                btnSearch.BackColor = ColorTranslator.FromHtml("#377cab");
                btnOK.BackColor = ColorTranslator.FromHtml("#377cab");
                btnSave.BackColor = ColorTranslator.FromHtml("#2ab541");
                btnClose.BackColor = ColorTranslator.FromHtml("#ffb700");


            }

        }
        private void InitUI()
        {
            //PBs = new PictureBox[] { picC0111, picC0221, picC0331, picC0422, picC0441, picC0632, picC0661, picC0824, picC0919, picC0933
            //                                        ,picC1243, picC1427, picC2054, picC2446
            //                                        ,picI0321, picI0532, picI0642, picI0734, picI0752, picI0312
            //                                        ,picS0422, picS0844, picS1266, picS1688, picS3216
            //                                    };
            PBs = new PictureBox[] { picC0111, picC0221, picC0331, picC0422, picC0441, picC0632, picC0824, picC0933
                                                    ,picC1243
                                                    ,picI0321, picI0532, picI0642, picI0734, picI1043, picI1134, picI0221
                                                    ,picS0422, picS0844, picS1266,
                                                };

            foreach (PictureBox PB in PBs)
            {
                PB.Click += new EventHandler((s, ea) => OnPatternClick(s, ea));
            }

            Button[] BTs = new Button[] { btnSearch, btnOK, btnSave, btnClose };
            foreach (Button BT in BTs)
            {
                BT.MouseHover += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.Yellow));
                BT.MouseLeave += new EventHandler((s, ea) => UICls.Btn_MouseHover(s, ea, Color.LightGray));
            }

            if (ImagePallet == null)
            {
                frmOrder f = new frmOrder();
                ImagePallet = f.imlMain.Images[0];
            }
        }
        private void Loaddata()
        {
            //  If Loaddata() is called from 'frmProduct_Load()'
            //  -   txtMaterial.Text will never be null or empty
            //  But it can be null or empty if for example called when clicking 'Search' button
            if (txbMatNO.Text == "")
            {
                MessageBox.Show("Product code is empty.", "Product code not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txbMatNO.Focus();
                return;
            }

            numBDLength.Value = 0;
            numBDWidth.Value = 0;
            CalBundleHeight = 0;
            //*chkFingerUse.Checked = false;

            string Old_Pattern = "";
            string Old_PatSeq = "";

            DataTable dt = new DataTable();
            dt = CheckMatrialOnMaster(txbMatNO.Text.Trim());
            if (dt.Rows.Count > 0)
            {
                Update_UIValue();
                AutoFindPattern();
                //*PatternLay(patterns.Name, patterns.BundlePerLayer.ToString());
                //*GetPatternOrder(Current_ptm.Pattern, panelSim);

                //Select Last Pattern
                Old_Pattern = dt.Rows[0]["Pattern_Code"].ToString();
                string buf_PatSeq = dt.Rows[0]["PatternSeq"].ToString();
                Old_PatSeq = (!string.IsNullOrEmpty(buf_PatSeq)) ? buf_PatSeq : "";

            }
            else
            {
                bool IsComplete = false;
                if (Properties.Settings.Default.OrderFromSystem)
                    if (Properties.Settings.Default.PM3_ERP == false)
                        IsComplete = GetDataFromPM2(txbMatNO.Text.Trim());
                    else
                    {
                        IsComplete = GetDataFromPM3(txbMatNO.Text.Trim());

                        if (IsComplete)
                        {
                            //Check SwapSide
                            bool NeedSwapSide = CheckPM3_OrderNeedSwap(txbMatNO.Text.Trim());
                            chkbxSwapLW.Checked = NeedSwapSide; // check from Mo_Routing

                            Check_AllowSwapBDSize(Convert.ToInt16(chkbxSwapLW.Checked? numBDLength.Value : numBDWidth.Value));

                            if (NeedSwapSide && (!chkbxSwapLW.Enabled))
                            {
                                string buf_Msg = "'Swap(LxW)' option is selected from PM3 data.\n" +
                                                    "But current Bundle Width is too small for the Gripper to grip bundle after it was rotated.\n" +
                                                    "The Program will now unselect 'Swap(LxW)' option.";
                                string buf_Topic = "Warning! - Abnormal PM3 Data.";
                                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                chkbxSwapLW.Checked = chkbxSwapLW.Enabled;
                            }
                        }
                    }


                if (IsComplete == false)
                {
                    InvalidOrder = true;
                    return;
                }

                AutoFindPattern();
                //*PatternLay(patterns.Name, patterns.BundlePerLayer.ToString());
                //*GetPatternOrder(Current_ptm.Pattern, panelSim);
                //*chkSquaringByPass.Checked = false;
                //* we will set this to default in the designer instead chkSquaringUse.Checked = true;
            }

            //Bussiness Process By P'Wallop @ 2020-06-18
            GetBestPattern(); // Determine and select best pattern
            // But if the Material Number has old selected pattern that selected pattern will be select again from 'Old_Pattern'

            //GetPattern by PMT
            GetPatternFromPMT();

            //Get Last Save Pattern
            if (!string.IsNullOrEmpty(Old_Pattern) && PBs != null && PBs.Count() > 0)
            {
                foreach (PictureBox buf_PB in PBs)
                {
                    if (buf_PB.Name.EndsWith(Old_Pattern) && buf_PB.Enabled == true)
                    {
                        OnPatternClick(buf_PB, null);
                        if (!string.IsNullOrEmpty(Old_PatSeq))
                        {
                            if (Old_PatSeq.First() == '1')
                            {
                                    string buf_msg = "Please be informed that\n" +
                                                        "This Order's Pattern Sequence has been modified when it was saved.\n\n" +
                                                        "If you don't need this option.\n" +
                                                        "Please unselect 'Modify Switching Pattern' option.";
                                    string buf_topic = "Warning! - Switching Patterns has been modified.";
                                    MessageBox.Show(buf_msg, buf_topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    chkbxModSWPat.Enabled = true;
                                    chkbx1stSWPat.Enabled = true;
                                    chkbx2ndSWPat.Enabled = true;
                                    chkbx1stSWPat.Checked = (Old_PatSeq[2]) == '1' ? true : false;
                                    chkbx2ndSWPat.Checked = (Old_PatSeq[4]) == '1' ? true : false;
                                    if (Current_ptm.Pattern.First() == 'S')
                                    {
                                        chkbx3rdSWPat.Enabled = true;
                                        chkbx4thSWPat.Enabled = true;
                                        chkbx3rdSWPat.Checked = (Old_PatSeq[6]) == '1' ? true : false; ;
                                        chkbx4thSWPat.Checked = (Old_PatSeq[8]) == '1' ? true : false; ;
                                        Current_ptm.SwitchPattern = "0:1:1:1:1";
                                    }
                                    chkbxModSWPat.Checked = true;
                            }

                        }
                        flag_unCF_boxChange = false;
                        flag_unCF_fluteChange = false;
                        flag_unCF_bdsizeChange = false;
                        btnSave.Text = "SAVE";
                        btnSave.BackColor = Color.ForestGreen;
                    }
                }

            }
            else
            {
                //Auto Rotate
                if (numBDWidth.Value > numBDLength.Value)
                    chkbxSpecialRotate.Checked = true;

                //Set Bundle Height to 0 on New Material
                if (Properties.Settings.Default.BD_NewData_ClearHeight)
                    numBundleHeight.Value = 0;

                //Auto Set GripperFinger
                if (Properties.Settings.Default.Finger_NewData_AutoSet == true && chkbxFingerUse.Enabled)
                    chkbxFingerUse.Checked = true;

                rdoPlaceNormal.Checked = true;
                rdoDCNormal.Checked = true;
                chkbxAntiBounce.Checked = false;
                rdoPlaceFast.Enabled = false;
                rdoDCFast.Enabled = false;

            }

            if (String.IsNullOrEmpty(txbProductCode.Text))
                txbProductCode.Text = "No Product code";

            void Update_UIValue()
            {
                foreach (DataRow dr in dt.Rows)
                {
                    flag_UpdatingUI = true; // use to block any checkchange, valuechange
                    //== Material No. Boundary (Upper Left Corner)
                    txbMatNO.Text = dr["Material_No"].ToString();
                    numBoxLength.Value = dr["ActualBox_L"] == DBNull.Value ? 0 : Convert.ToInt16(dr["ActualBox_L"]);
                    numBoxWidth.Value = dr["ActualBox_W"] == DBNull.Value ? 0 : Convert.ToInt16(dr["ActualBox_W"]);
                    numBoxHeight.Value = dr["ActualBox_H"] == DBNull.Value ? 0 : Convert.ToInt16(dr["ActualBox_H"]);
                    cmbBoxType.Text = dr["Box_Type"].ToString();
                    cmbFluteType.Text = dr["Flute_Type"].ToString();
                    bool buf_SwapLW;
                    bool.TryParse(dr["SwitchBDSize"].ToString(), out buf_SwapLW);
                    chkbxSwapLW.Checked = buf_SwapLW;
                    Check_AllowSwapBDSize(Convert.ToInt16(numBDWidth.Value));
                    bool buf_SpecialRotate;
                    bool.TryParse(dr["SpecialRotate"].ToString(), out buf_SpecialRotate);
                    if (buf_SpecialRotate && buf_SwapLW)
                    {
                        string loc_Msg = string.Format("Both 'Swap(LxW)' and 'rotate(W>L)' options are selected from History data.\n" +
                                                    "Please verify these 2 options again.\n" +
                                                    "These 2 options are not allow to be selected at the same time.\n" +
                                                    "The Program will now unselect 'rotate(W>L)' option.");
                        string loc_Topic = string.Format("Warning! - Abnormal History Data");
                        MessageBox.Show(loc_Msg, loc_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        chkbxSpecialRotate.Checked = false;
                    }
                    else
                    {
                        chkbxSpecialRotate.Checked = buf_SpecialRotate;
                    }
                    //== Product Code Boundary (Inside Material No. Bdr.)
                    Fold_height = dr["FoldWork_T"] == DBNull.Value ? 0 : Convert.ToDouble(dr["FoldWork_T"]);
                    numBDLength.Value = dr["FoldWork_L"] == DBNull.Value ? 0 : Convert.ToInt16(dr["FoldWork_L"]);
                    numBDWidth.Value = dr["FoldWork_W"] == DBNull.Value ? 0 : Convert.ToInt16(dr["FoldWork_W"]);
                    lblBundleSizeText.Text = numBDLength.Value + " X " + numBDWidth.Value;
                    StackHeight = Cal_StackHeight(cmbBoxType.Text, cmbFluteType.Text); // Calculate Completed Palletized Stack on Pallet use at PatternLay()
                    HeightRatio = Convert.ToInt16(dr["HeightRatio"]);
                    HeightRatio_Master = HeightRatio;
                    btnHeightRatio.Text = HeightRatio.ToString() + "%";
                    if (HeightRatio != 100)
                        btnHeightRatio.Enabled = true;
                    else
                        btnHeightRatio.Enabled = false;
                    txbProductCode.Text = dr["Product_Code"].ToString();
                    lblLastUpdate.Text = "Last update: " + (DateTime)dr["StampDate"];

                    //== Bundle Properties Boundary (Top Right Corner)
                    numPiecePerBD.Value = (Int16)dr["PiecePerBD"];
                    numBDPerLY.Value = dr["BDPerLY"] == DBNull.Value ? 0 : (Int16)dr["BDPerLY"];
                    numPiecePerLY.Value = dr["PiecePerLY"] == DBNull.Value ? 0 : (Int16)dr["PiecePerLY"];
                    numLYPerPallet.Value = dr["LYPerPallet"] == DBNull.Value ? 0 : (Int16)dr["LYPerPallet"];
                    numBoxWeight.Value = Convert.ToDecimal(dr["FoldWork_Weight"]);
                    numBundleHeight.Value = (Int16)(Fold_height * (float)numPiecePerBD.Value);
                    BundleHeight_Master = (Int16)numBundleHeight.Value;
                    Check_AllowGrip2BD(Convert.ToInt16(numBundleHeight.Value)); // use numBundleHeight to compare and Enable/Disable numBDPerGrip, Set value to 1 if disable
                    numBDPerGrip.Value = numBDPerGrip.Enabled ? (Int16)dr["BDPerGrip"] : 1;

                    //== Pattern Picture Boundary
                    string buf_PatternType = dr["Pattern_Code"].ToString();
                    Current_ptm.Pattern = buf_PatternType;

                    //== Pallet Type Boundary
                    pallets.Width = (int)dr["Pallet_W"];
                    pallets.Length = (int)dr["Pallet_L"]; ;
                    pallets.Height = (int)dr["Pallet_H"]; ;
                    pallets.Name = dr["Pallet_Type"].ToString();

                    lblPalletType.Text = pallets.Name;
                    lblSizePallet.Text = string.Format("{0}x{1}x{2}", pallets.Width, pallets.Length, pallets.Height);
                    //------------------------------------

                    //== TieSheet
                    int buf_TieLayer;
                    int.TryParse(dr["TS_everyXLY"].ToString(), out buf_TieLayer);
                    if (buf_TieLayer > 0)
                    {
                        numTieLayer.Value = buf_TieLayer;
                        chkbxTieSheet.Checked = true;
                        numTieLayer.Enabled = true;

                        //Tie Sheet Special Options
                        bool buf_TSB4LastLayer;
                        bool.TryParse(dr["TS_B4LastLY"].ToString(), out buf_TSB4LastLayer);
                        chkbxTS_B4LastLY.Checked = buf_TSB4LastLayer;
                        chkbxTS_B4LastLY.Enabled = chkbxTieSheet.Checked;

                        bool buf_TSLowSQLayer;
                        bool.TryParse(dr["TS_SQLayer"].ToString(), out buf_TSLowSQLayer);
                        chkbxTS_LowSQLayer.Checked = buf_TSLowSQLayer;
                        chkbxTS_LowSQLayer.Enabled = chkbxTieSheet.Checked;
                    }
                    else
                    {
                        chkbxTieSheet.Checked = false;
                        chkbxTS_B4LastLY.Checked = false;
                        chkbxTS_LowSQLayer.Checked = false;
                        numTieLayer.Enabled = false;
                        chkbxTS_B4LastLY.Enabled = false;
                        chkbxTS_LowSQLayer.Enabled = false;
                    }

                    //== TopSheet / BottomSheet
                    bool buf_TopSheet;
                    bool.TryParse(dr["TopSheet"].ToString(), out buf_TopSheet);
                    chkbxTopSheet.Checked = buf_TopSheet;
                    int buf_BtmSheet = (Int32)dr["BtmSheet"];
                    chkbxBtmSheet.Checked = ((buf_BtmSheet == 1) || (buf_BtmSheet == 2));
                    chkbxBtmSheetByRobot.Enabled = chkbxBtmSheet.Checked && chkbxBtmSheet.Enabled;
                    chkbxBtmSheetByRobot.Checked = (buf_BtmSheet == 2);

                    //== Gripper Finger
                    bool AllowGripperFinger = Check_AllowGripperFinger(Convert.ToInt16(chkbxSwapLW.Checked? numBDWidth.Value:numBDLength.Value));
                    chkbxFingerUse.Enabled = AllowGripperFinger;
                    bool buf_FingerUse;
                    bool.TryParse(dr["GripperFinger"].ToString(), out buf_FingerUse);
                    chkbxFingerUse.Checked = buf_FingerUse && chkbxFingerUse.Enabled;

                    //== Squaring
                    if (chkbxSquaringUse.Enabled)
                    {
                        bool buf_Squaring;
                        bool.TryParse(dr["Squaring"].ToString(), out buf_Squaring);
                        chkbxSquaringUse.Checked = buf_Squaring;
                        chkbxExtraSQ.Enabled = buf_Squaring;
                        if (chkbxSquaringUse.Checked)
                        {
                            bool buf_ExtraSQ;
                            bool.TryParse(dr["SQ_Extra"].ToString(), out buf_ExtraSQ);
                            chkbxExtraSQ.Checked = buf_ExtraSQ;
                        }
                        else
                        {
                            chkbxExtraSQ.Checked = false;
                        }
                    }

                    //== Stacker Lift Bundle
                    bool buf_StackerLiftBD;
                    bool.TryParse(dr["StackerLiftBD"].ToString(), out buf_StackerLiftBD);
                    chkbxLiftStack.Checked = buf_StackerLiftBD;
                    Check_AllowSTKNoLiftBD(Convert.ToInt16(chkbxSwapLW.Checked ? numBDWidth.Value : numBDLength.Value));
                    if ((buf_StackerLiftBD = false) && (chkbxLiftStack.Enabled = false))
                    {
                        string buf_Msg = "'Stacker Lift Bundle' option is unselected from History data.\n"+
                                            "But current Bundle "+(chkbxSwapLW.Checked?"Width":"Length")+" is too small for the Gripper to grip bundle inside stacker.\n"+
                                            "The Program will now select 'Stacker Lift Bundle' option."+
                                            (chkbxSwapLW.Checked? "\n\n Remark: SwapLW is selected so Bundle Width is used to compare.":"");
                        string buf_Topic = "Warning! - Abnormal Master Data.";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    //== AntiBounce
                    bool buf_AntiBounce;
                    bool.TryParse(dr["AntiBounce"].ToString(), out buf_AntiBounce);
                    chkbxAntiBounce.Checked = buf_AntiBounce;

                    //== ExtraPick
                    bool buf_ExtraPick;
                    bool.TryParse(dr["ExtraPickDepth"].ToString(), out buf_ExtraPick);
                    chkbxExtraPick.Checked = buf_ExtraPick;

                    //== Placing Mode
                    int buf_PlacingMode;
                    int.TryParse(dr["PlacingMode"].ToString(), out buf_PlacingMode);
                    switch (buf_PlacingMode)
                    {
                        case 1:
                            {
                                rdoPlaceSoft.Checked = true;
                                break;
                            }
                        case 2:
                            {
                                rdoPlaceNormal.Checked = true;
                                break;
                            }
                        case 3:
                            {
                                rdoPlaceFast.Checked = true;
                                break;
                            }
                    }

                    //== Discharge Mode
                    int buf_DischargeMode;
                    int.TryParse(dr["DischargeMode"].ToString(), out buf_DischargeMode);
                    switch (buf_DischargeMode)
                    {
                        case 1:
                            {
                                rdoDCSlow.Checked = true;
                                break;
                            }
                        case 2:
                            {
                                rdoDCNormal.Checked = true;
                                break;
                            }
                        case 3:
                            {
                                rdoDCFast.Checked = true;
                                break;
                            }
                    }

                    //== Rotate Pattern
                    int buf_RotatePattern;
                    int.TryParse(dr["RotatePattern"].ToString(), out buf_RotatePattern);
                    switch (buf_RotatePattern)
                    {
                        case -1: chkbxRotatePattern.Checked = false; break; //Disable/Enable itself and its option will be handle at its checked_change
                        case 0: chkbxRotatePattern.Checked = true; break; // remove sub RotPat checkbox ver3.0 2023-05-8
                        case 1: chkbxRotatePattern.Checked = true; break; // remove sub RotPat checkbox ver3.0 2023-05-8
                        case 2: chkbxRotatePattern.Checked = true; break; // remove sub RotPat checkbox ver3.0 2023-05-8
                        case 3: chkbxRotatePattern.Checked = true; break; // remove sub RotPat checkbox ver3.0 2023-05-8
                    }

                    //== Fix Bundle Face
                    bool buf_FixBDFace;
                    bool.TryParse(dr["FixBDFace"].ToString(), out buf_FixBDFace);
                    chkbxFixBDFace.Checked = buf_FixBDFace;

                    //== Pattern Sequence: will be set after OnPatternClick() from OldPattern in LoadData
                    //because if we set it here it will be overrided by GetPatternOrder()

                    flag_UpdatingUI = false; // use to block any checkchange, valuechange

                    Log.WriteLog("Import Master from Mater: " + txbMatNO.Text + ": Successfully", LogType.Success);
                }
            }

            bool GetDataFromPM2(string MatNo)
            {
                bool IsComplete;
                flag_PM2GetData = true;
                String sqlcommand = "SELECT Top 1 [OrderItem],[Material_No],[PC],[Flute],[Code],[Wid],[Leg],[Hig],[Box_Type],[Bun],[BunLayer],[LayerPalet],[Weight_Box],[CutSheetWid] " +
                             "FROM [MO_Spec] where [Material_No] = '" + MatNo + "' Order by [LastUpdate] DESC";

                dt = Sql.GetDataTableFromSql(sqlcommand, Sql.GetPM2Connection());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //string boxType = dr["Box_Type"].ToString();
                        //int indOf = boxType.IndexOf(" ");
                        //string boxType = "BSC";

                        txbMatNO.Text = dr["Material_No"].ToString();
                        txbProductCode.Text = dr["PC"].ToString();
                        numBoxHeight.Value = Convert.ToInt16(dr["Hig"]);
                        numBoxLength.Value = Convert.ToInt16(dr["Leg"]);
                        numBoxWidth.Value = Convert.ToInt16(dr["Wid"]);
                        numPiecePerBD.Value = Convert.ToInt16(dr["Bun"]);
                        numBDPerLY.Value = Convert.ToInt16(dr["BunLayer"]);
                        numPiecePerLY.Value = Convert.ToInt16(dr["Bun"]) * Convert.ToInt16(dr["BunLayer"]);
                        numLYPerPallet.Value = Convert.ToInt16(dr["LayerPalet"]);

                        //if (string.IsNullOrEmpty(boxType))
                        //{
                        cmbBoxType.SelectedIndex = 0;

                        //}
                        //else
                        //{
                            //cmbBoxType.Text = boxType;
                        //}
                        cmbFluteType.Text = dr["Flute"].ToString();
                        //cmbbxPallType.SelectedIndex = 0;
                        numBoxWeight.Value = Convert.ToDecimal(dr["Weight_Box"]);
                        lblSO.Text = "SO : " + dr["OrderItem"].ToString();
                        btnOK.Enabled = true;

                        //Top @ 17-07-2020
                        _fWidthFromCutSheetWid = Convert.ToInt16(dr["CutSheetWid"]);

                    }
                    //frmProduct.txtMaterial.Text = "";
                    Log.WriteLog("Import Master from MO_Spec: " + MatNo + ": Successfully", LogType.Success);
                    IsComplete = true;
                    // log.WriteLog("Import Master from MO_Spec: " + MatNo, "Successfully", this.ToString());
                }
                else
                {
                    // If Loaddata from Click Search Event() it will need this to proc msg box
                    string buf_Msg = "Couldn't find data you request in PM2 system. \n"+
                                        "Please try again or manually key the Order Data.";
                    string buf_Topic = "Error! - Product not found. ORD3004";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    Log.WriteLog("Loaddata Import order in PM2 Error" + MatNo + ": ", LogType.Fail);
                    IsComplete = false;
                    flag_PM2GetData = false;
                    return IsComplete;
                }

                if (FoldSize(cmbBoxType.Text) == true) // Calculate numBDLength, numBDWidth, _fHeight, StackHeight return false if numBDLength x numBDWidth Exceed Limit
                {
                    numBDWidth.Value = Convert.ToDecimal(_fWidthFromCutSheetWid);//@top 17-07-2020 // from PM2
                    lblBundleSizeText.Text = numBDLength.Value.ToString("0") + " X " + numBDWidth.Value.ToString("0");
                    flag_PM2GetData = false;
                    IsComplete = true;
                }
                else
                {
                    Log.WriteLog("Loaddata : founction FoldSize in GetDataFromPM2 ", LogType.Fail);
                    IsComplete = false;
                    flag_PM2GetData = false;
                    return IsComplete;
                }

                flag_PM2GetData = false;
                return IsComplete;
            }

            bool GetDataFromPM3(string MatNo)
            {
                bool IsComplete;
                flag_PM3GetData = true;
                PMTPattern = "";

                String sqlcommand = string.Format(@"
                                                SELECT 
	                                                Top 1 [OrderItem],[Material_No],[PC],[Flute],[Code],[Wid],[Leg],[Hig],[Box_Type],[Bun],[BunLayer],[LayerPalet],[Weight_Box],[PicPallet],[CutSheetWid]
                                                FROM 
	                                                [MO_Spec] 
                                                WHERE
	                                                FactoryCode = (SELECT TOP 1 Plant FROM CompanyProfile WHERE ShortName = '{0}')
	                                                AND Material_No = '{1}'
                                                Order by [LastUpdate] DESC", Properties.Settings.Default.PlantCode, MatNo);

                dt = Sql.GetDataTableFromSql(sqlcommand, Sql.GetPM3Connection());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string boxType = dr["Box_Type"].ToString();
                        int indOf = boxType.IndexOf(" ");
                        boxType = boxType.Substring(0, indOf);

                        txbMatNO.Text = dr["Material_No"].ToString();
                        txbProductCode.Text = dr["PC"].ToString();
                        numBoxHeight.Value = Convert.ToInt16(dr["Hig"]);
                        numBoxLength.Value = Convert.ToInt16(dr["Leg"]);
                        numBoxWidth.Value = Convert.ToInt16(dr["Wid"]);
                        numPiecePerBD.Value = Convert.ToInt16(dr["Bun"]);
                        numBDPerLY.Value = Convert.ToInt16(dr["BunLayer"]);
                        numPiecePerLY.Value = Convert.ToInt16(dr["Bun"]) * Convert.ToInt16(dr["BunLayer"]);
                        numLYPerPallet.Value = Convert.ToInt16(dr["LayerPalet"]);
                        if (boxType == "Diecut")
                        {
                            string buf_Msg = "[คำเตือน]งานนี้เป็นกล่อง Diecut (จำเป็นต้องตรวจสอบขนาดจริง)\r\n คุณต้องการเดิน Order นี้ใช่หรือไม่";
                            string buf_Topic = "Warning! - This is DieCut Order.";
                            var RP = MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (RP == DialogResult.No)
                            {
                                Log.WriteLog("Invalid BoxType MO_Spec: " + MatNo, LogType.Notes);
                                return false;
                            }
                            else
                                boxType = "RSC";
                        }

                        cmbBoxType.Text = boxType;
                        cmbFluteType.Text = dr["Flute"].ToString();
                        //cmbbxPallType.SelectedIndex = 0;
                        numBoxWeight.Value = Convert.ToDecimal(dr["Weight_Box"]);
                        lblSO.Text = "SO : " + dr["OrderItem"].ToString();
                        PMTPattern = dr["PicPallet"].ToString();

                        //@Top 17-07-2020
                        _fWidthFromCutSheetWid = Convert.ToInt16(dr["CutSheetWid"]);
                        btnOK.Enabled = true;
                    }
                    //frmProduct.txtMaterial.Text = "";
                    Log.WriteLog("Import Master from MO_Spec: " + MatNo + ": Successfully", LogType.Success);
                    IsComplete = true;
                }
                else
                {
                    // If Loaddata from Click Search Event() it will need this to proc msg box
                    string buf_Msg = "Couldn't find data you request in PM3 system. Please try again or manually key data.";
                    string buf_Topic = "Error! - Product not found. ORD3004";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    Log.WriteLog("Loaddata Import order in PM2 Error" + MatNo + ": ", LogType.Fail);
                    IsComplete = false;
                    flag_PM3GetData = false;
                    return IsComplete;
                }

                if (FoldSize(cmbBoxType.Text) == true) // Calculate numBDLength, numBDWidth, _fHeight, StackHeight return false if numBDLength x numBDWidth Exceed Limit
                {
                    numBDWidth.Value = Convert.ToDecimal(_fWidthFromCutSheetWid);

                    lblBundleSizeText.Text = numBDLength.Value.ToString("0") + " X " + numBDWidth.Value.ToString("0");
                    flag_PM3GetData = false;
                    IsComplete = true;
                }
                else
                {
                    Log.WriteLog("Loaddata : founction FoldSize in GetDataFromPM3 ", LogType.Fail);
                    IsComplete = false;
                    flag_PM3GetData = false;
                    return IsComplete;
                }

                flag_PM3GetData = false;
                return IsComplete;
            }

            bool CheckPM3_OrderNeedSwap(string MatNo)
            {
                bool result = false;
                string Query = string.Format(@"SELECT 
	                                                TOP 1 OrderItem
                                                FROM 
	                                                Mo_Routing 
                                                WHERE 
	                                                Mat_Code = 'FG' 
	                                                AND Material_No = '{0}'
	                                                AND Remark_Inprocess LIKE '%มัดกล่องในแนวขวางลอน%'
	                                                AND FactoryCode = (SELECT TOP 1 Plant FROM CompanyProfile WHERE ShortName = '{1}')"
                                                , MatNo, Properties.Settings.Default.PlantCode);
                var Found = Sql.GetScalarFromSql(Query, Sql.GetPM3Connection());
                result = (Found == null || string.IsNullOrEmpty(Found.ToString())) ? false : true;
                return result;
            }
        }
        private DataTable CheckMatrialOnMaster(string MatNo)
        {
            DataTable dt = new DataTable();
            try
            {
                string Query = "SELECT * FROM [tblMaster] WHERE Material_No = '" + txbMatNO.Text + "'";
                dt = Sql.GetDataTableFromSql(Query);

            }
            catch (Exception e)
            {
                Log.WriteLog("CheckMatrialOnMaster Function Error" + e.Message, LogType.Fail);
            }

            return dt;
        }

        private void GetBestPattern()
        {
            ClientData.LoadPatternOptions();
            string loc_ResultPatName = "";
            //*rename int PalletLeng, PalletWide;
            int buf_PalletLength, buf_PalletWidth;

            buf_PalletLength = pallets.Length;
            buf_PalletWidth = pallets.Width;

            Int16 loc_BDLength = Convert.ToInt16(numBDLength.Value);
            Int16 loc_BDWidth = Convert.ToInt16(numBDWidth.Value);

            //Array.Clear(pattern, 0, pattern.Length);

            //Process for pattern of spiral
            String sPattern = GenPtnAuto.SpiralPattern(loc_BDLength, loc_BDWidth, buf_PalletLength, buf_PalletWidth, Convert.ToInt16(Properties.Settings.Default.Max_BDperLayer));
            if (sPattern != "")
            {
                string[] stringPattern = sPattern.Split(':');
                /*
                pattern1[0].Name = stringPattern[0];
                pattern1[0].Direction = stringPattern[1];
                if (stringPattern[2] == "1") { pattern1[0].Turn = true; }
                { pattern1[0].Turn = false; };
                pattern1[0].Leng = Convert.ToInt32(stringPattern[3]);
                pattern1[0].Wide = Convert.ToInt32(stringPattern[4]);
                pattern1[0].Flip = false;
                pattern1[0].Amount = Convert.ToInt32(stringPattern[0].Substring(1, 2));
                */
                loc_ResultPatName = stringPattern[0];
            }

            //Process for pattern of Interlock
            String iPattern = GenPtnAuto.InterlockPattern(loc_BDLength, loc_BDWidth, buf_PalletLength, buf_PalletWidth, Convert.ToInt16(Properties.Settings.Default.Max_BDperLayer));
            if (iPattern != "")
            {
                string[] stringPattern = iPattern.Split(':');/*
                pattern1[1].Name = stringPattern[0];
                pattern1[1].Direction = stringPattern[1];
                if (stringPattern[2] == "1") { pattern1[1].Turn = true; }
                { pattern1[1].Turn = false; };
                pattern1[1].Leng = Convert.ToInt32(stringPattern[3]);
                pattern1[1].Wide = Convert.ToInt32(stringPattern[4]);
                pattern1[1].Flip = false;
                pattern1[1].Amount = Convert.ToInt32(stringPattern[0].Substring(1, 2));*/
                loc_ResultPatName = stringPattern[0];
            }

            //Process for pattern of column
            string cPattern = GenPtnAuto.ColumnPattern(loc_BDLength, loc_BDWidth, buf_PalletLength, buf_PalletWidth, ClientData.PatternData, Convert.ToInt16(Properties.Settings.Default.Max_BDperLayer));
            if (cPattern != "")
            {
                string[] stringPattern = cPattern.Split(':');
                /*
                pattern1[2].Name = stringPattern[0];
                pattern1[2].Direction = stringPattern[1];
                if (stringPattern[2] == "1") { pattern1[2].Turn = true; }
                { pattern1[2].Turn = false; };
                pattern1[2].Leng = Convert.ToInt32(stringPattern[3]);
                pattern1[2].Wide = Convert.ToInt32(stringPattern[4]);
                pattern1[2].Flip = false;
                pattern1[2].Amount = Convert.ToInt32(stringPattern[0].Substring(1, 2));
                */
                loc_ResultPatName = stringPattern[0];
            }

            if (!string.IsNullOrEmpty(loc_ResultPatName))
            {
                foreach (PictureBox buf_PB in PBs)
                {
                    string Pattern = buf_PB.Name.Replace("pic", "");
                    if (DisablePattern.Contains(Pattern))
                        continue;

                    if (buf_PB.Name.EndsWith(loc_ResultPatName) && buf_PB.Enabled != false)
                    {
                        OnPatternClick(buf_PB, null);
                    }
                }
            }

        }
        private void GetPatternFromPMT()
        {
            if (string.IsNullOrEmpty(PMTPattern) || Properties.Settings.Default.PM3_ERP != true)
                return;

            try
            {


                string _PMTPattern = PMTPattern.Substring(0, PMTPattern.IndexOf(".")).ToUpper();
                foreach (PictureBox buf_PB in PBs)
                {
                    string Pattern = buf_PB.Name.Replace("pic", "");
                    if (DisablePattern.Contains(Pattern))
                        continue;

                    if (buf_PB.Name.EndsWith(_PMTPattern) && buf_PB.Enabled == true)
                    {
                        OnPatternClick(buf_PB, null);
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Can't find pattern from MasterCard: " + PMTPattern + "." + e.Message, LogType.Warning);
            }

        }

        #region AutoPattern
        private void AutoFindPattern()
        {
            //PictureBox[] PBs = new PictureBox[] { picC0111, picC0221, picC0331, picC0422, picC0441, picC0632, picC0661, picC0824, picC0919, picC0933
            //                                        ,picC1243, picC1427, picC2054, picC2446
            //                                        ,picI0321, picI0532, picI0642, picI0734, picI0752
            //                                        ,picS0422, picS0844, picS1266, picS1688, picS3216
            //                                    };    

            // If MatchingPattern() is called while pallets is null
            // update pallets attribute with PalletSetDefault
            // * if pallets is null it will return with Wooden Pallet attribute
            if (pallets == null)
            {
                pallets.Width = 1000;
                pallets.Length = 1200;
                pallets.Height = 150;
                pallets.Name = "Wooden";

                lblPalletType.Text = pallets.Name;
                lblSizePallet.Text = string.Format("{0}x{1}x{2}", pallets.Width, pallets.Length, pallets.Height);
            }
            decimal LastPTNScore = 0;
            PatternModel loc_ptm = new PatternModel();
            foreach (PictureBox PB in PBs)
            {
                bool IsEnable = false;
                string Pattern = PB.Name.Replace("pic", "");
                if (DisablePattern.Contains(Pattern))
                {
                    PB.ImageLocation = string.Format(@"{0}\Images\Patterns\{1}.png", Application.StartupPath, Pattern + "D");
                    PB.Enabled = IsEnable;
                    continue;
                }

                loc_ptm.Pattern = Pattern;
                loc_ptm.SwithBDSize = chkbxSwapLW.Checked;
                //loc_ptm.BundleWidth = (loc_ptm.SwithBDSize == false ? Convert.ToInt32(numBDWidth.Value) : Convert.ToInt32(numBDLength.Value));
                //loc_ptm.BundleLength = (loc_ptm.SwithBDSize == false ? Convert.ToInt32(numBDLength.Value) : Convert.ToInt32(numBDWidth.Value));
                loc_ptm.BundleWidth = (!loc_ptm.SwithBDSize ? Convert.ToInt32(numBDWidth.Value) : Convert.ToInt32(numBDLength.Value)); // Mod condition ver3.0 2023-05-05
                loc_ptm.BundleLength = (!loc_ptm.SwithBDSize ? Convert.ToInt32(numBDLength.Value) : Convert.ToInt32(numBDWidth.Value)); // Mod condition ver3.0 2023-05-05
                loc_ptm.BundleGap = (chkbxFingerUse.Checked ? Properties.Settings.Default.BDGap_wFinger : Properties.Settings.Default.BDGap_woFinger);
                //*loc_ptm.PalletIsRotated = false;//* chkRotatePallet.Checked;
                loc_ptm.PalletWidth = pallets.Width;
                loc_ptm.PalletLength = pallets.Length;
                loc_ptm.SpecialRotate = chkbxSpecialRotate.Checked; //Special Rotate 2020-10-05
                loc_ptm.UpdatePatternDetail(); // Separate Pattern Name -> S0422 -> S, 04, 22

                Laying[] loc_OpenBundles = loc_ptm.GetPlacementInfo(true); //  Get array of IndividualBundle data with Gap of all SwitchingPattern of this Pattern

                if (loc_OpenBundles != null && loc_OpenBundles.Length > 0) // Do we have IndividualBundle data after Calculation?
                {
                    if (!string.IsNullOrEmpty(loc_OpenBundles[0].PatternName)) // Does this element have Pattern name?
                    {
                        IsEnable = VerifyPattern(loc_OpenBundles, loc_ptm);

                        if (IsEnable == true)
                        {
                            decimal buf_ptmScore = GetPatternScore(loc_ptm);
                            if (buf_ptmScore > Properties.Settings.Default.Max_PatternScore)
                                IsEnable = false;
                            else
                            {
                                if (buf_ptmScore <= 100)
                                {
                                    if (buf_ptmScore > LastPTNScore)
                                    {
                                        if (Fold_height_Caled == 0)
                                            CalBundleHeight = Convert.ToDecimal(Math.Round((Fold_height * (double)numPiecePerBD.Value), 0));
                                        else
                                            CalBundleHeight = Convert.ToDecimal(Math.Round((Fold_height_Caled * (double)numPiecePerBD.Value), 0));

                                        bool AllowGripperFinger = Check_AllowGripperFinger(chkbxSwapLW.Checked ? loc_ptm.BundleWidth : loc_ptm.BundleLength);
                                        chkbxFingerUse.Enabled = AllowGripperFinger;
                                        if (chkbxFingerUse.Enabled == false)
                                            chkbxFingerUse.Checked = false;
                                        else
                                        {
                                            //if (Properties.Settings.Default.Finger_NewData_AutoSet == true)
                                                //chkbxFingerUse.Checked = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                PB.ImageLocation = string.Format(@"{0}\Images\Patterns\{1}.png", Application.StartupPath, (IsEnable == true ? Pattern : Pattern + "D"));
                PB.Enabled = IsEnable;
            }
        }

        private bool VerifyPattern(Laying[] _lays, PatternModel ptn = null)
        {
            bool Valid = true;

            // I use Perimeter instead because after i implement fixface pattern
            // Lays will don't have bundle the will turn to all side of the pattern to compare for Xmax/min Ymax/min
            if (ptn.LayerOpenPeriWidth > Properties.Settings.Default.Peri_Max_OpenW || ptn.LayerOpenPeriLength > Properties.Settings.Default.Peri_Max_OpenL)
                return false;

            // Check if Perimeter Exceed 1300x1500 (Overhang 150 on each side)
            //But also have a limit for maximum total border incase too large overhang is use
            //[2020-09-08]Check Limit @Beer
            //*wrong direction if (ptn.LayerClosePeriLength > Properties.Settings.Default.Peri_Max_Width || ptn.LayerClosePeriWidth > Properties.Settings.Default.Peri_Max_Length)
            if ((Properties.Settings.Default.SQModel != 0) && (ptn.LayerClosePeriWidth > Properties.Settings.Default.Peri_Max_CloseW || ptn.LayerClosePeriLength > Properties.Settings.Default.Peri_Max_CloseL))
                return false;
            if (ptn.LayerClosePeriWidth > ptn.PalletWidth + (2*Properties.Settings.Default.Max_OVH) || ptn.LayerClosePeriLength > ptn.PalletLength + (2*Properties.Settings.Default.Max_OVH))
                return false;

            //[2020-09-10]Check Limit for I0312 @Beer
            if (ptn.Pattern == "I0312" && ptn.LayerClosePeriLength < Properties.Settings.Default.I0312_Limit_Length)
                return false;

            //Check Bundle not in Pallet ***Check first Layer***
            if (ptn != null)
            {
                int MaxBD_OVH_Percent = Properties.Settings.Default.Max_idvBD_OVH_Percent; //Minimum Bundle Percent on Pallet
                int NewDX = 0;
                int NewDY = 0;
                int CX = 0;
                int CY = 0;
                int PX = 0;
                int PY = 0;
                // _lays is PatternBundle info of SwitchingPattern without Gap
                foreach (Laying Bundle in _lays)
                {
                    if (Bundle.Layer > 1)
                    {
                        break;
                    }

                    switch (Bundle.Ori)
                    {
                        case 0:
                            {
                                NewDY = Math.Abs(Math.Abs(Bundle.Y) > (ptn.PalletWidth / 2) ? 
                                    Bundle.Y : 
                                    (ptn.BundleLength - Bundle.Y));
                                NewDX = Bundle.X > (ptn.BundleLength / 2) ? 
                                    Bundle.X : 
                                    (Bundle.X - ptn.BundleWidth);
                                break;
                            }
                        case 1:
                            {
                                NewDY = Math.Abs(Math.Abs(Bundle.Y) > (ptn.PalletWidth / 2) ? 
                                    Bundle.Y : 
                                    (ptn.BundleWidth + Bundle.Y));
                                NewDX = Bundle.X > (ptn.BundleLength + ptn.BundleLength) ? 
                                    Bundle.X : 
                                    (Bundle.X - ptn.BundleLength);
                                break;
                            }
                        case 2:
                            {
                                NewDY = Math.Abs(Math.Abs(Bundle.Y) > (ptn.PalletWidth / 2) ? 
                                    Bundle.Y : 
                                    (ptn.BundleLength + Bundle.Y));
                                NewDX = Bundle.X < (ptn.BundleLength / 2) ? 
                                    Bundle.X : 
                                    (Bundle.X + ptn.BundleWidth);
                                break;
                            }
                        case 3:
                            {
                                NewDY = Math.Abs(Math.Abs(Bundle.Y) > (ptn.PalletWidth / 2) ? 
                                    Bundle.Y : 
                                    (Bundle.Y - ptn.BundleWidth));
                                NewDX = Bundle.X < ptn.BundleWidth ? 
                                    Bundle.X : 
                                    (Bundle.X + ptn.BundleLength);
                                break;
                            }
                    }

                    CY = (ptn.PalletWidth / 2) - NewDY;
                    if (CY < 0) // Mean this bundle is overhang
                    {
                        PY = Math.Abs(CY) * 100 / ptn.BundleWidth;
                        if (PY > MaxBD_OVH_Percent)
                            return false;
                    }

                    CX = ptn.PalletLength - NewDX;
                    if (CX < 0) // Mean this bundle is overhang
                    {
                        PX = Math.Abs(CX) * 100 / ptn.BundleLength;
                        if (PX > MaxBD_OVH_Percent)
                            return false;
                    }
                }
            }

            return Valid;
        }

        private decimal GetPatternScore(PatternModel ptn)
        {
            decimal result = 0;
            try
            {
                //int PatternPerimeter = ptn.LayerPerimeterWidth * (ptn.LayerPerimeterLength - ptn.PalletStoppingOffset);
                int LayerClosePerimeter = ptn.LayerClosePeriWidth * ptn.LayerClosePeriLength;
                int PalletPerimeter = ptn.PalletWidth * ptn.PalletLength;
                decimal PerimeterScore = (LayerClosePerimeter * 100) / PalletPerimeter;

                switch (ptn.PatternType)
                {
                    case "S": { result = (50) + (PerimeterScore / 2); break; } // S is the best because it has a lot of interlocking.
                    case "I": { result = (40) + (PerimeterScore / 2); break; } // I is good but it has less interlocking than Spiral.
                    case "C": { result = (30) + (PerimeterScore / 2); break; } // C is the worst because it doesn't has any interlocking.
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message, LogType.Fail);
            }


            return result;
        }
        #endregion

        public void PatternLay(string patternName, string BDperLY)
        {
            patternClear(); // clear PatternPicture Background Color

            if (Convert.ToInt16(BDperLY) > Properties.Settings.Default.Max_BDperLayer)
            {
                string buf_Msg = string.Format("Current Bundle per Layer value is: {0}\n" +
                                            "Which is more than maximum value: {1}\n" +
                                            "Please check the Pattern selection and try again",
                                            BDperLY, Properties.Settings.Default.Max_BDperLayer);
                string buf_topic = "Error! - Bundle Per Layer exceed maximum value.";
                MessageBox.Show(buf_Msg, buf_topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            foreach (PictureBox PB in PBs)
            {
                if (patternName == PB.Name.Replace("pic", ""))
                {
                    PB.BackColor = Color.Yellow;
                    PB.BorderStyle = BorderStyle.FixedSingle;
                }
            }

            try
            {
                //*lblPatternName.Text = patterns.Description;

            }
            catch (Exception ex)
            {
                string buf_Msg = "Abnormal error exception string as below:\n" + ex.Message;
                string buf_Topic = "Error! - Abnormal error";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Log.WriteLog("PatternLay Error :" + ex.Message, LogType.Fail);
            }
        }
        private void patternClear()
        {
            foreach (var c in Controls) // it only count Parent Control not include child control
            {
                if (c is TextBox)
                    ((TextBox)c).Text = string.Empty;
                if (c is NumericUpDown)
                    ((NumericUpDown)c).Value = 0;
                //if (c is PictureBox) ((PictureBox)c).BackColor = System.Drawing.Color.White;
                foreach (Control p in palPatternMaster.Controls)
                {
                    if (p is PictureBox)
                    {
                        p.BackColor = Color.White;
                    }
                }
            }
        }

        private bool GetPatternOrder(string PatternName, Panel pnl = null)
        {
            PatternModel loc_ptm = new PatternModel();
            if (Check_DataNotEmpty("NA", Convert.ToInt16(numBDWidth.Value), Convert.ToInt16(numBDLength.Value)))
            {

                if (PatternName != null)
                {
                    flag_InGetPatternOrder = true;
                    //--Simulation--
                    loc_ptm.Pattern = PatternName.ToString();
                    loc_ptm.SwithBDSize = chkbxSwapLW.Checked;
                    //loc_ptm.BundleWidth = (loc_ptm.SwithBDSize != true ? Convert.ToInt32(numBDWidth.Value) : Convert.ToInt32(numBDLength.Value));
                    //loc_ptm.BundleLength = (loc_ptm.SwithBDSize != true ? Convert.ToInt32(numBDLength.Value) : Convert.ToInt32(numBDWidth.Value));
                    loc_ptm.BundleWidth = (!loc_ptm.SwithBDSize? Convert.ToInt32(numBDWidth.Value) : Convert.ToInt32(numBDLength.Value)); // Mod condition ver3.0 2023-05-05
                    loc_ptm.BundleLength = (!loc_ptm.SwithBDSize? Convert.ToInt32(numBDLength.Value) : Convert.ToInt32(numBDWidth.Value)); // Mod condition ver3.0 2023-05-05
                    loc_ptm.BundleGap = (chkbxFingerUse.Checked ? Properties.Settings.Default.BDGap_wFinger : Properties.Settings.Default.BDGap_woFinger);
                    //*loc_ptm.PalletIsRotated = false;//* chkRotatePallet.Checked;
                    loc_ptm.PalletWidth = pallets.Width;
                    loc_ptm.PalletLength = pallets.Length;
                    loc_ptm.RotatePattern = chkbxRotatePattern.Checked; //remove sub RotPat checkbox ver3.0 2023-05-8
                    loc_ptm.FixBundleFace = chkbxFixBDFace.Checked; // *add 20230117
                    loc_ptm.SpecialRotate = chkbxSpecialRotate.Checked; //Special Rotate 2020-10-05
                    loc_ptm.UpdatePatternDetail();
                    OpenBundles = loc_ptm.GetPlacementInfo();

                    if (string.IsNullOrEmpty(OpenBundles[0].PatternName))
                    {
                        flag_CannotRotate = true;
                        return false;
                    }

                    if (!VerifyPattern(OpenBundles, loc_ptm))
                    {
                        flag_CannotRotate = true;
                        return false;
                    }

                    chkbxModSWPat.Enabled = true;
                    chkbx1stSWPat.Enabled = false;
                    chkbx2ndSWPat.Enabled = false;
                    chkbx3rdSWPat.Enabled = false;
                    chkbx4thSWPat.Enabled = false;
                    chkbx1stSWPat.Checked = true;
                    chkbx2ndSWPat.Checked = true;
                    if (loc_ptm.Pattern.First() == 'S')
                    {
                        chkbx3rdSWPat.Checked = true;
                        chkbx4thSWPat.Checked = true;
                        loc_ptm.SwitchPattern = "0:1:1:1:1";
                    }
                    else
                    {
                        chkbx3rdSWPat.Checked = false;
                        chkbx4thSWPat.Checked = false;
                        loc_ptm.SwitchPattern = "0:1:1:0:0";
                    }
                    chkbxModSWPat.Checked = false;
                    flag_InGetPatternOrder = false;
                    Current_ptm = loc_ptm; // Use when call SimulatePattern from Dispaly Options CheckChange

                    numBDPerLY.Value = Current_ptm.BundlePerLayer;//Convert.ToInt32(pattern[0, 1]); 
                    
                    //Piece per layer
                    numPiecePerLY.Value = Current_ptm.BundlePerLayer * Convert.ToInt16(numPiecePerBD.Value);

                    //Calculate number of layer per pallet
                    if (numLYPerPallet.Value == 0)
                    {
                        numLYPerPallet.Value = StackHeight / (Convert.ToInt16(Fold_height) * Convert.ToInt16(numPiecePerBD.Value));
                    }
                    lblPtName.Text = "Pattern : " + Current_ptm.Pattern;

                    if (pnl != null)
                        SimulatePattern(pnl, loc_ptm, rdoPressSquaring.Checked);

                    Bundle3DCls BD3D = new Bundle3DCls();
                    Simulation3D(BD3D, loc_ptm, loc_ptm.TotalPatternBundles, true);

                    SQ_OPX = loc_ptm.SQ_XaxisValue;
                    SQ_OPY = loc_ptm.SQ_YaxisValue; // use to return to db only
                    SQ_CLX = loc_ptm.SQ_XaxisValueWithOutGap;
                    SQ_CLY = loc_ptm.SQ_YaxisValueWithOutGap;
                    //*PL_OFFSET = loc_ptm.PalletStoppingOffset;
                    LayerClosePeriLength = loc_ptm.LayerClosePeriLength;
                    LayerClosePeriWidth = loc_ptm.LayerClosePeriWidth;

                    rdoDisOption_PatLay1.Checked = true; // Auto Set Layer Display Option to 1st Layer

                    decimal buf_ptmScore = GetPatternScore(loc_ptm);
                    lblScore.ForeColor = (buf_ptmScore > 100 ? Color.Yellow : Color.Green);
                    lblScore.Text = string.Format("Score:{0}%", buf_ptmScore);

                    double buf_efficiency = Cal_Efficiency(Current_ptm.BundleWidth, Current_ptm.BundleLength, pallets.Length, pallets.Width, Convert.ToInt16(Current_ptm.BundlePerLayer));
                    if (buf_efficiency < 0)
                    {
                        lblEfficiency.ForeColor = Color.Red;
                    }
                    else
                    {
                        lblEfficiency.ForeColor = Color.Black;
                    }

                    Efficiency = buf_efficiency;
                    lblEfficiency.Text = "Efficiency: " + Efficiency.ToString("n2") + " %";

                    //MessageBox.Show("Pattern ID : " + pattern[0, 2] + " Pattern Code : " + pattern[0, 0] + " Bundle : " + pattern[0, 1]);
                    if (!(flag_unCF_boxChange || flag_unCF_fluteChange || flag_unCF_bdsizeChange || flag_unCF_palletChange))
                    {
                        btnSave.Enabled = true; // Will be set to False at MatchingPattern() in OK_Click, SWAPLW_CheckChange, RotatePallet_CheckCange, SpecialRotate_CheckChange and after PalletSize has been changed
                        btnSave.Text = "SAVE";
                        btnSave.BackColor = Color.ForestGreen;
                    }
                    //TestOnly
                    lblDev.Text = string.Format("RT:{0} X:{1} Y:{2} OpenX:{3} OpenY:{4}", loc_ptm.SuggestRotate, loc_ptm.LayerClosePeriWidth, loc_ptm.LayerClosePeriLength, loc_ptm.SQ_XaxisValue, loc_ptm.SQ_YaxisValue);
                    flag_GotPatternOrder = true;
                }
            }
            flag_CannotRotate = false;
            return true;
        }

        public void CrossFromLoaddata()
        {
            txbMatNO.Text = pMaterialNo;
            Loaddata();
        }

        // Get Pre-Calculate Finished Product on Pallet Height
        public Int16 Cal_StackHeight(string BoxType, string Flute)
        {
            Int16 stackHieght = 0;
            try
            {
                for (int i = 0; i < ClientData.Flute.GetLongLength(0); i++)
                {
                    if (Flute == ClientData.Flute[i])
                    {
                        stackHieght = Convert.ToInt16(ClientData.StackHeight[i]);
                        break;
                    }
                }

            }
            catch
            {
                stackHieght = 0;
            }

            return stackHieght;
        }

        //Calculate efficency
        private static double Cal_Efficiency(double loc_FoldLeng, double loc_FoldWidth, double loc_PalletL, double loc_PalletW, Int16 loc_BDperLY)
        {
            double loc_PattBoxArea = ((loc_FoldLeng * loc_FoldWidth) * loc_BDperLY);
            double loc_PalletArea = (loc_PalletL * loc_PalletW);
            double loc_efficiency = (loc_PattBoxArea / loc_PalletArea) * 100;
            if (loc_efficiency > 100)
            {
                loc_efficiency = (loc_efficiency - 100) * -1;
            }
            return loc_efficiency;
        }

        private Boolean FoldSize(string BoxType)
        {
            //  if numBD = 0
            //      - Calculate _fLeng, _fWidth from numBox
            //      - _fHeight, StackHeight
            //      if BoxType = 'Other'
            //          - numBD = _fLeng / _fWidth
            //  if numBD != 0
            //      - _fLeng / _fWidth
            //  if _fLeng / _fWidth Exceed System Limit
            //      - return false
            //  else
            //      - return true

            Boolean foldSize = false;
            int loc_Length = 0;
            int loc_Width = 0;
            //double loc_height = 0;
            // 2nd dimension meaning
            //  0: Flute, 1: Thickness, 2: SheetPerBundle, 3: cof_a, 4: cof_b, 5: cof_c, 6: cof_s, 7: stack_H
            try
            {
                if (numBDLength.Value == 0 || numBDWidth.Value == 0)
                {
                    if (!String.IsNullOrEmpty(cmbFluteType.Text) && !String.IsNullOrEmpty(cmbBoxType.Text))
                    {
                        if (numBoxWidth.Value == 0 || numBoxLength.Value == 0 || numBoxHeight.Value == 0)
                        {
                            string buf_Msg = "The value of Bundle Width or Length is equal to zero.\n" +
                                                "ID4P will calculate Bundle Width and Length from Box Size.\n\n" +
                                                "But the values of Box Width, Length or Height, at least one of the is equal to zero.\n" +
                                                "which may result in incorrect Calculated Bundle Size.\n\n" +
                                                "If it is not intentional please verify the Box Size value and try again.";
                            string buf_Topic = "Warning! - Abnormal Box Size.";
                            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        switch (BoxType)
                        {
                            case "RSC": //RSC Standare Convert to flat box

                                for (int i = 0; i < ClientData.Flute.GetLongLength(0); i++)
                                {
                                    if (cmbFluteType.Text == ClientData.Flute[i]) //Flute, Thickness, Bundle, a, b, c, d
                                    {
                                        loc_Length = (Convert.ToInt32(numBoxLength.Value) + Convert.ToInt32(ClientData.Ratio_a[i])) +
                                            (Convert.ToInt32(numBoxWidth.Value) + Convert.ToInt32(ClientData.Ratio_a[i]));
                                        loc_Width = Convert.ToInt32(numBoxWidth.Value) + (Convert.ToInt32(ClientData.Ratio_c[i]) * 2) +
                                            (Convert.ToInt32(numBoxHeight.Value) + Convert.ToInt32(ClientData.Ratio_d[i]));
                                        if (flag_PM2GetData || flag_PM3GetData)
                                        {
                                            Fold_height_Caled = (Convert.ToDouble(ClientData.Thickness[i]) * 2);
                                            if (numBundleHeight.Value == 0 && numPiecePerBD.Value != 0)
                                                numBundleHeight.Value = (Int16)Fold_height_Caled * numPiecePerBD.Value;
                                        }
                                        StackHeight = Convert.ToInt16(ClientData.StackHeight[i]);
                                        break;
                                    }
                                }
                                break;

                            case "TELE": //Half slot carton and tele
                                for (int i = 0; i < ClientData.Flute.GetLongLength(0); i++)
                                {
                                    if (cmbFluteType.Text == ClientData.Flute[i]) //Flute, Thickness, Bundle, a, b, c, d
                                    {
                                        loc_Length = (Convert.ToInt32(numBoxLength.Value) + Convert.ToInt32(ClientData.Ratio_a[i])) +
                                            (Convert.ToInt32(numBoxWidth.Value) + Convert.ToInt32(ClientData.Ratio_a[i]));
                                        loc_Width = (Convert.ToInt32(numBoxWidth.Value) / 2) + Convert.ToInt32(ClientData.Ratio_c[i]) +
                                            (Convert.ToInt32(numBoxHeight.Value) + Convert.ToInt32(Math.Round(Convert.ToDouble(ClientData.Ratio_d[i]) / 2)));
                                        if (flag_PM2GetData || flag_PM3GetData)
                                        {
                                            Fold_height_Caled = (Convert.ToDouble(ClientData.Thickness[i]) * 2);
                                            if (numBundleHeight.Value == 0 && numPiecePerBD.Value != 0)
                                                numBundleHeight.Value = (Int16)Fold_height_Caled * numPiecePerBD.Value;
                                        }
                                        StackHeight = Convert.ToInt16(ClientData.StackHeight[i]);
                                        break;
                                    }
                                }
                                break;

                            case "FOL": //Full over lap
                                for (int i = 0; i < ClientData.Flute.GetLongLength(0); i++)
                                {
                                    if (cmbFluteType.Text == ClientData.Flute[i]) //Flute, Thickness, Bundle, a, b, c, d
                                    {
                                        loc_Length = (Convert.ToInt32(numBoxLength.Value) + Convert.ToInt32(ClientData.Ratio_a[i])) +
                                            (Convert.ToInt32(numBoxWidth.Value) + Convert.ToInt32(ClientData.Ratio_a[i]));
                                        loc_Width = (Convert.ToInt32(numBoxWidth.Value) * 2) + (Convert.ToInt32(ClientData.Ratio_c[i]) * 2) +
                                            (Convert.ToInt32(numBoxHeight.Value) + Convert.ToInt32(ClientData.Ratio_d[i]));
                                        if (flag_PM2GetData || flag_PM3GetData)
                                        {
                                            Fold_height_Caled = (Convert.ToDouble(ClientData.Thickness[i]) * 2);
                                            if (numBundleHeight.Value == 0 && numPiecePerBD.Value != 0)
                                                numBundleHeight.Value = (Int16)Fold_height_Caled * numPiecePerBD.Value;
                                        }
                                        StackHeight = Convert.ToInt16(ClientData.StackHeight[i]);
                                        break;
                                    }
                                }
                                break;

                            case "HFOL": //Half sloted full over lap
                                for (int i = 0; i < ClientData.Flute.GetLongLength(0); i++)
                                {
                                    if (cmbFluteType.Text == ClientData.Flute[i]) //Flute, Thickness, Bundle, a, b, c, d
                                    {
                                        loc_Length = (Convert.ToInt32(numBoxLength.Value) + Convert.ToInt32(ClientData.Ratio_a[i])) +
                                            (Convert.ToInt32(numBoxWidth.Value) + Convert.ToInt32(ClientData.Ratio_a[i]));
                                        loc_Width = Convert.ToInt32(numBoxWidth.Value) + Convert.ToInt32(ClientData.Ratio_c[i]) +
                                            (Convert.ToInt32(numBoxHeight.Value) + Convert.ToInt32(Math.Round(Convert.ToDouble(ClientData.Ratio_d[i]) / 2)));
                                        if (flag_PM2GetData || flag_PM3GetData)
                                        {
                                            Fold_height_Caled = (Convert.ToDouble(ClientData.Thickness[i]) * 2);
                                            if (numBundleHeight.Value == 0 && numPiecePerBD.Value != 0)
                                                numBundleHeight.Value = (Int16)Fold_height_Caled * numPiecePerBD.Value;
                                        }
                                        StackHeight = Convert.ToInt16(ClientData.StackHeight[i]);
                                        break;
                                    }
                                }
                                break;

                            case "Other": //Other box type or die cut
                                for (int i = 0; i < ClientData.Flute.GetLongLength(0); i++)
                                {
                                    if (cmbFluteType.Text == ClientData.Flute[i]) //Flute, Thickness, Bundle, a, b, c, d
                                    {
                                        loc_Length = Convert.ToInt32(numBoxLength.Value); //ใช้กว้างยาวของกล่อง แทนขนาดพับ
                                        loc_Width = Convert.ToInt32(numBoxWidth.Value);
                                        numBDLength.Value = loc_Length;
                                        numBDWidth.Value = loc_Width;
                                        if (flag_PM2GetData || flag_PM3GetData)
                                        {
                                            Fold_height_Caled = Convert.ToDouble(ClientData.Thickness[i]) * 2;
                                            if (numBundleHeight.Value == 0 && numPiecePerBD.Value != 0)
                                                numBundleHeight.Value = (Int16)Fold_height_Caled * numPiecePerBD.Value;
                                        }
                                        StackHeight = Convert.ToInt16(ClientData.StackHeight[i]);
                                        break;
                                    }
                                }
                                break;

                            default:
                                string buf_Msg = "Choosen Box type is not found in the System.\n" +
                                                    "Please check it again or Contact System Admin.";
                                string buf_Topic = "Error! - Box type not found";
                                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                Log.WriteLog("This Box type is not confuguration in system.Please contact administrator to fix it." + "", LogType.Fail);
                                break;
                        }
                    }
                    else
                    {
                        string buf_Msg = "Either or both Box Type or Flute Type is not selected.\n\n" +
                                            "Please make the selection for both of them, then try again.\n" +
                                            "or\n" +
                                            "Manually key Bundle Width and Length, then try again.";
                        string buf_Topic = "Error! - No selected Box or Flute Type.";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    loc_Length = Convert.ToInt32(numBDLength.Value);
                    loc_Width = Convert.ToInt32(numBDWidth.Value);
                }

                if (Check_BDSize(loc_Width, loc_Length))
                {
                    numBDWidth.Value = Convert.ToDecimal(loc_Width);
                    numBDLength.Value = Convert.ToDecimal(loc_Length);
                    foldSize = true;
                }
                else
                {
                    numBDWidth.Value = 0;
                    numBDLength.Value = 0;
                    btnSave.Enabled = false;
                    btnSave.Text = "Blocked until press 'OK'";
                    btnSave.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                foldSize = false;
                Log.WriteLog("FoldSize Box Type Error cel :", LogType.Fail);
            }
            return foldSize;
        }
        public bool Check_DataNotEmpty(string pc, int width = 0, int length = 0)
        {
            // Used at OnPatternClick and chkFinger_CheckChanged
            bool chk = true;

            if (string.IsNullOrEmpty(pc))
            {
                string buf_Msg = "Product Code is empty.\n" +
                                    "Please fill the information and try again.";
                string buf_Topic = "Error! - Product Code is Empty";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                chk = false;
            }

            chk = Check_BDSize(width, length);
            return chk;
        }

        private void Check_PatternTypeInterlock(string Pattern, string PatternsType)
        {
            if (PatternsType == "C")
            {
                chkbxFixBDFace.Enabled = true;
                chkbxRotatePattern.Enabled = true;
            }
            else
            {
                if (PatternsType == "I")
                {
                    chkbxFixBDFace.Enabled = false;
                    chkbxRotatePattern.Enabled = true;
                }
                else
                {
                    chkbxFixBDFace.Checked = false;
                    chkbxRotatePattern.Checked = false;
                }
            }

            //Special Case for I0312
            if (Pattern == "I0312")
            {
                chkbxFixBDFace.Enabled = false;
                chkbxRotatePattern.Enabled = false;
                chkbxFixBDFace.Checked = false;
                chkbxRotatePattern.Checked = false;
            }
        }

        #region Simulation

        public void SimulatePattern(Panel pnl, PatternModel ptn, bool IsPressMode, int Layer = 1)
        {
            pnl.Refresh();
            float DisplayScale = (float)pnl.Size.Height / (float)2000;
            bool[,] SQ_UDH = { {
                    (ptn.SQ_XaxisValueWithOutGap > Properties.Settings.Default.SQ_Max_X) ,
                    (ptn.SQ_YaxisValueWithOutGap > Properties.Settings.Default.SQ_Max_Y) },
                    {
                    (ptn.SQ_XaxisValue - 30 > Properties.Settings.Default.SQ_Max_X) ,
                    (ptn.SQ_YaxisValue - 30 > Properties.Settings.Default.SQ_Max_Y) } };
            if (Properties.Settings.Default.SquaringModel != -1)
                if (SQ_UDH[0, 0] || SQ_UDH[0, 1])
                {
                    string buf_Msg = "Minimum Squaring Perimeter is:\n" +
                                        (Properties.Settings.Default.Peri_Max_OpenW - (2 * Properties.Settings.Default.SQ_Max_Y)) + " , " +
                                        (Properties.Settings.Default.Peri_Max_OpenL - (2 * Properties.Settings.Default.SQ_Max_X)) + " mm.\n" +
                                        "But current configuration will make perimeter be:\n" +
                                        ptn.LayerClosePeriWidth + " , " + ptn.LayerClosePeriLength + " mm.\n\n" +
                                        "The Squaring will not be able to completely close the gap between bundles.";
                    string buf_Topic = "Warning! - Perimeter Exceed Minimum Overhang Value.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            DrawPallet(pnl, ptn, DisplayScale);
            DrawSquaring(pnl, ptn, DisplayScale, SQ_UDH, IsPressMode);
            DrawBundle(pnl, ptn, true, DisplayScale, Layer, IsPressMode);
        }
        private void DrawSquaring(Panel pnl, PatternModel ptn, float loc_DisplayScale,bool[,] SQ_UDH, bool IsPressMode = false)
        {
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
            PictureBox PalletBox = null;
            var _PalletBox = FindControlInPanel(pnl, "PalletBox");
            int PLW = ptn.PalletWidth;
            int PLL = ptn.PalletLength;
            if (_PalletBox == null)
            {
                PalletBox = new PictureBox();
                PalletBox.Name = "PalletBox";
                PalletBox.SizeMode = PictureBoxSizeMode.StretchImage;
                PalletBox.BorderStyle = BorderStyle.FixedSingle;
                PalletBox.Image = ImagePallet;
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
            if (Y_inverted)
                Yinv = -1;
            
            //Clear Data
            for (int i = pnl.Controls.Count - 1; i >= 0; i--)
            {
                Control ctrl = pnl.Controls[i];
                if (ctrl is PictureBox PB && PB.Name.Contains("Outbox"))
                    pnl.Controls.Remove(ctrl);
            }

            // Create 32 Picture Boxes and add it to pnl (pnlSim)
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
            Laying[] LayerBundle = ptn.GetPlacementInfo_AtLayer(Layer,ptn.PatternBundle,ptn.PatternBundleWithOutGap, IsPressMode);
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
                Log.WriteLog(e.Message, LogType.Fail);
                result = null;
            }

            return result;
        }
        private void CreateBundleGraphics(int BoxIndex, int Orientation, PictureBox[] ResultBox)
        {
            int W = ResultBox[BoxIndex].Width;
            int H = ResultBox[BoxIndex].Height;
            Font font1 = new Font("Arial", 16);

            Bitmap TempImage = new Bitmap(W, H);
            //System.Drawing.Pen pen1 = new System.Drawing.Pen(Color.Blue, 2F);
            System.Drawing.Pen pen1 = new System.Drawing.Pen(ColorTranslator.FromHtml("#993300"), 2F);
            using (Graphics g = Graphics.FromImage(TempImage))
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
            ResultBox[BoxIndex].Image = TempImage;
            ResultBox[BoxIndex].BringToFront();
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
                if (picbx3DOrder.Image != null)
                    picbx3DOrder.Image.Dispose();
                picbx3DOrder.Image = null;
            }

            BD3D.Pattern = ptn.Pattern;

            if ((BD3D.BDS.Count == 0 || BD3D.BDS.Count < BundleNo) && BundleNo > 1)
            {
                for (int i = BD3D.BDS.Count; i < BundleNo; i++)
                {
                    BD3D.AddBundlebyBundleNo(i + 1);
                }
                //LastPalletNo = PalletNo;
            }

            BD3D.DrawBundle(picbx3DOrder);
        }
        #endregion

        private void OnPatternClick(object sender, EventArgs e)
        {
            PictureBox PB = (PictureBox)sender;
            string loc_Pattern = PB.Name.Replace("pic", "");
            if (Check_DataNotEmpty(txbMatNO.Text.Trim(), Convert.ToInt16(numBDWidth.Value), Convert.ToInt16(numBDLength.Value)) == true)
            {
                PatternLay(loc_Pattern, loc_Pattern.Substring(1, 2));
                Check_PatternTypeInterlock(loc_Pattern,loc_Pattern.Substring(0, 1));
            }
            else
            {
                // Alarm will already trigger from Check_DataNotEmpty()
                return;
            }

            GetPatternOrder(loc_Pattern, pnlSim);
            LastPB = PB; // Use at chkFinger_CheckChanged to re-calculate Bundle Position

            ////Test Pattern Kawa
            //var PTemp = GetPatternOrder(Pattern, panelSim);
            //var XPArr = PTemp.GetPlacementArraywithField(PTemp.GetPlacementInfo(), "X");
            //var YPArr = PTemp.GetPlacementArraywithField(PTemp.GetPlacementInfo(), "Y");
            //var OriPArr = PTemp.GetPlacementArraywithField(PTemp.GetPlacementInfo(), "Ori");

            //Auto Disable TieSheet 2020-10-10 @Beer
            //if (Pattern == "C0111")
            //{
            //    chkTieSheet.Enabled = false;
            //    chkTieSheet.Checked = false;
            //}
            //else
            //{
            //    chkTieSheet.Enabled = true;
            //    chkTieSheet.Checked = false;
            //}
        }

        private int GetSelectedLayer()
        {
            int Layer = 1;

            if (rdoDisOption_PatLay1.Checked) return 1;
            if (rdoDisOption_PatLay2.Checked) return 2;
            if (rdoDisOption_PatLay3.Checked) return 3;
            if (rdoDisOption_PatLay4.Checked) return 4;

            return Layer;
        }

        #region Button OK, SwapLW, SpecialRotate
        private void btnOK_Click(object sender, EventArgs e)
        {
            flag_RotatePat_ReturnValue= true;
            chkbxRotatePattern.Checked = false;
            chkbxFixBDFace.Checked = false;
            chkbxModSWPat.Checked = false;
            flag_RotatePat_ReturnValue = false;
            if (MatchingPattern())
            {
                flag_unCF_boxChange = false;
                flag_unCF_fluteChange = false;
                flag_unCF_bdsizeChange = false;
                flag_unCF_palletChange = false;
                btnSave.Text = "SAVE";
                btnSave.Enabled = true;
                btnSave.BackColor = Color.ForestGreen;
            }
        }
        private void chkbxSwapLW_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_UpdatingUI)
                return;
            if (flag_SwapLW_ReturnValue)
                return;

            flag_SwapLW_Pass = MatchingPattern();
            if (!flag_SwapLW_Pass)
            {
                flag_SwapLW_ReturnValue = true;
                chkbxSwapLW.Checked = !chkbxSwapLW.Checked;
                flag_SwapLW_ReturnValue = false;
            }
            if (chkbxSwapLW.Checked)
                chkbxSpecialRotate.Checked = false;

            lblBDSizeTextTopic.Text = (chkbxSwapLW.Checked == true ? "LxW (Swap)" : "LxW");
            lblBundleSizeText.Text = (chkbxSwapLW.Checked == true ? (numBDWidth.Value.ToString("0") + " X " + numBDLength.Value.ToString("0")) : (numBDLength.Value.ToString("0") + " X " + numBDWidth.Value.ToString("0")));

        }
        private void chkbxSpecialRotate_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_UpdatingUI)
                return;
            if (flag_SpeicalRot_ReturnValue)
                return;

            flag_SpecialRot_Pass = MatchingPattern();
            if (!flag_SpecialRot_Pass)
            {
                flag_SpeicalRot_ReturnValue = true;
                chkbxSpecialRotate.Checked = !chkbxSpecialRotate.Checked;
                flag_SpeicalRot_ReturnValue = false;
            }
            if (chkbxSpecialRotate.Checked)
                chkbxSwapLW.Checked = false;

            lblBDSizeTextTopic.Text = (chkbxSwapLW.Checked == true ? "WxL (Swap)" : "LxW");
            lblBundleSizeText.Text = (chkbxSwapLW.Checked == true ? (numBDWidth.Value.ToString("0") + " X " + numBDLength.Value.ToString("0")) : (numBDLength.Value.ToString("0") + " X " + numBDWidth.Value.ToString("0")));
         
        }
        private Boolean MatchingPattern()
        {
            btnSave.Enabled = false; // will be set to TRUE at PatterLay()
            btnSave.Text = "Blocked until press 'OK'";
            btnSave.BackColor = Color.Red;

            //*lblPatternName.Text = "Pattern:";
            lblPtName.Text = "Name:";
            lblEfficiency.Text = "Efficency:";

            //Convert box dimension to folding size
            if (FoldSize(cmbBoxType.Text) == true) // Calculate numBDLength, numBDWidth, _fHeight, StackHeight return false if numBDLength x numBDWidth Exceed Limit
            {
                //To process to autoselect patten type
                lblBundleSizeText.Text = (chkbxSwapLW.Checked == true ? (numBDWidth.Value.ToString("0") + " X " + numBDLength.Value.ToString("0")) : (numBDLength.Value.ToString("0") + " X " + numBDWidth.Value.ToString("0")));
            }
            else
            {
                return false;
            }

            //Check
            //if(numPieceBundle.Value <= 0 || numLayerPallet.Value <= 0 || numBundleHeight.Value <= 0)
            if (numPiecePerBD.Value <= 0 || numLYPerPallet.Value <= 0)
            {
                string buf_Msg = string.Format("Below information must be greater than zero.\n" +
                                            "Please verify them and try again.\n" +
                                            " - Layer per Pallet\n" +
                                            " - Piece per Bundle");
                string buf_topic = "Error! - Invalid order information.";
                MessageBox.Show(buf_Msg, buf_topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            
            AutoFindPattern();
            PatternLay(Current_ptm.Pattern, Current_ptm.BundlePerLayer.ToString());
            GetPatternOrder(Current_ptm.Pattern, pnlSim);

            //Bussiness Process By P'Wallop @ 2020-06-18
            GetBestPattern();
            return true;
        }
        #endregion
        private Boolean Check_BDSize(int loc_Width, int loc_Length)
        {
            if (loc_Width >= Properties.Settings.Default.BD_Min_Width && loc_Width <= Properties.Settings.Default.BD_Max_Width
                    && loc_Length >= Properties.Settings.Default.BD_Min_Length && loc_Length <= Properties.Settings.Default.BD_Max_Length)
            {
                return true;
            }
            else
            {
                numBDWidth.Value = 0;
                numBDLength.Value = 0;
                btnSave.Enabled = false;
                btnSave.Text = "Blocked until press 'OK'";
                btnSave.BackColor = Color.Red;
                string buf_Msg = string.Format("Size of the product is not matching robot specification.\n" +
                                                "Please verify bundle size and try again.\n" +
                                                " Bundle Length: {2}-{3} mm.\n" +
                                                " Bundle Width: {0}-{1} mm.",
                                                Properties.Settings.Default.BD_Min_Width, Properties.Settings.Default.BD_Max_Width,
                                                Properties.Settings.Default.BD_Min_Length, Properties.Settings.Default.BD_Max_Length);
                string buf_topic = "Error! - Bundle Size is out of Spec.";
                MessageBox.Show(buf_Msg,buf_topic,MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #region num BundleHeight, BW, BL ValueChanged
        private void numBundleHeight_ValueChanged(object sender, EventArgs e)
        {
            var loc_OldHeight = Math.Round((Fold_height * (double)numPiecePerBD.Value), 0);
            var loc_NewHeight = numBundleHeight.Value;
            if (flag_BDHeight_ReturnValue)
                return;
            if (numPiecePerBD.Value <= 0)
            {
                string buf_Msg = "Piece Per Bundle value is equal zero.\n" +
                                    "Please change Piece Per Bundle value to greater than zero then try again.\n\n"+
                                    "The program will now return Bundle Height to old value.";
                string buf_Topic = "Error! - Piece Per Bundle is zero";
                MessageBox.Show(buf_Msg, buf_Topic,MessageBoxButtons.OK, MessageBoxIcon.Error);
                numBundleHeight.Enabled = false;
                flag_BDHeight_ReturnValue = true;
                numBundleHeight.Value = Convert.ToDecimal(loc_OldHeight);
                flag_BDHeight_ReturnValue = false;
            }
            else
            {
                if ((decimal)loc_OldHeight != loc_NewHeight)
                {
                    var buf_NewFoldHeight = Math.Round(loc_NewHeight / numPiecePerBD.Value, 2);
                    Fold_height = (double)buf_NewFoldHeight;
                }
            }
            if (numBundleHeight.Value == BundleHeight_Master && HeightRatio_Master != 100)
            {
                btnHeightRatio.Enabled = true;
                string buf_Msg = "This Bundle Height Value is the same with Master Data.\n" +
                                    "The system will use Height Ratio from Master Data as well.\n\n" +
                                    "If you don't want to use that height ratio. click the button next to it to reset the ratio to 100%.";
                string buf_Topic = "Height Ratio has been modified.";
                MessageBox.Show(buf_Msg,buf_Topic,MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                btnHeightRatio.Enabled = false;
                btnHeightRatio.Text = "100%";
                HeightRatio = 100;
            }
            Check_AllowGrip2BD(Convert.ToInt16(numBundleHeight.Value));
        }
        private void numBDWidth_ValueChanged(object sender, EventArgs e)
        {
            //btnSave.Enabled = false;
            flag_unCF_bdsizeChange = true;
            btnSave.Text = "Blocked until press 'OK'";
            btnSave.BackColor = Color.Red;

            Check_AllowGrip2BD(Convert.ToInt16(numBundleHeight.Value));

            Check_AllowSwapBDSize(Convert.ToInt16(numBDWidth.Value));

        }
        private void numBDLength_ValueChanged(object sender, EventArgs e)
        {
            //btnSave.Enabled = false;
            flag_unCF_bdsizeChange = true;
            btnSave.Text = "Blocked until press 'OK'";
            btnSave.BackColor = Color.Red;

            Check_AllowSTKNoLiftBD(Convert.ToInt16(numBDLength.Value));
        }
        public void Check_AllowSTKNoLiftBD(int loc_BDLength)
        {
            chkbxLiftStack.Enabled = (loc_BDLength < Properties.Settings.Default.NoLift_Min_BDLength ? false : true);
            if (chkbxLiftStack.Enabled == false)
                chkbxLiftStack.Checked = true;
        }
        public void Check_AllowGrip2BD(int loc_BDHeight)
        {
            if (loc_BDHeight < Properties.Settings.Default.Grip2BD_Max_BDHeight && numBDWidth.Value < Properties.Settings.Default.Grip2BD_Max_BDWidth)
            {
                numBDPerGrip.Enabled = true;
            }
            else
            {
                numBDPerGrip.Value = 1;
                numBDPerGrip.Enabled = false;
            }
        }
        public void Check_AllowSwapBDSize(int loc_BDWidth)
        {
            if (loc_BDWidth >= Properties.Settings.Default.BD_Min_Length)
            {
                chkbxSwapLW.Enabled = true;
            }
            else
            {
                chkbxSwapLW.Enabled = false;
                chkbxSwapLW.Checked = false;
            }
        }
        #endregion

        public bool Check_AllowGripperFinger(int loc_BDLength)
        {
            bool buf_FingerAllowed = false;


            if (loc_BDLength >= Properties.Settings.Default.Finger_Min_BDLength)
                buf_FingerAllowed = true;

            return buf_FingerAllowed;
        }

        #region Button Search -> Call Loaddata()
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Loaddata();
        }
        #endregion

        #region Display Options -> Call SimulatePattern() if Current_ptm is not null
        #region Button Layer Display Options
        private void rdoDisOption_PatLay1_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDisOption_PatLay1.Checked)
            {
                if (Current_ptm != null)
                    SimulatePattern(pnlSim, Current_ptm, rdoPressSquaring.Checked, 1);
            }
            else
            {
                #region Clear Picture if all display are disabled
                if (!rdoDisOption_PatLay1.Checked && !rdoDisOption_PatLay2.Checked &&
                    !rdoDisOption_PatLay3.Checked && !rdoDisOption_PatLay4.Checked)
                {
                    for (int i = pnlSim.Controls.Count - 1; i >= 0; i--)
                    {
                        Control ctrl = pnlSim.Controls[i];
                        PictureBox PB = (PictureBox)ctrl;
                        if (PB.Name.Contains("Outbox"))
                        {
                            pnlSim.Controls.Remove(ctrl);

                        }
                    }
                }
                #endregion
            }
        }
        private void rdoDisOption_PatLay2_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDisOption_PatLay2.Checked)
            {
                if (Current_ptm != null)
                    SimulatePattern(pnlSim, Current_ptm, rdoPressSquaring.Checked, 2);
            }
            else
            {
                #region Clear Picture if all display are disabled
                if (!rdoDisOption_PatLay1.Checked && !rdoDisOption_PatLay2.Checked &&
                    !rdoDisOption_PatLay3.Checked && !rdoDisOption_PatLay4.Checked)
                {
                    for (int i = pnlSim.Controls.Count - 1; i >= 0; i--)
                    {
                        Control ctrl = pnlSim.Controls[i];
                        PictureBox PB = (PictureBox)ctrl;
                        if (PB.Name.Contains("Outbox"))
                        {
                            pnlSim.Controls.Remove(ctrl);

                        }
                    }
                }
                #endregion
            }
        }
        private void rdoDisOption_PatLay3_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDisOption_PatLay3.Checked)
            {
                if (Current_ptm != null)
                    SimulatePattern(pnlSim, Current_ptm, rdoPressSquaring.Checked, 3);
            }
            else
            {
                #region Clear Picture if all display are disabled
                if (!rdoDisOption_PatLay1.Checked && !rdoDisOption_PatLay2.Checked &&
                    !rdoDisOption_PatLay3.Checked && !rdoDisOption_PatLay4.Checked)
                {
                    for (int i = pnlSim.Controls.Count - 1; i >= 0; i--)
                    {
                        Control ctrl = pnlSim.Controls[i];
                        PictureBox PB = (PictureBox)ctrl;
                        if (PB.Name.Contains("Outbox"))
                        {
                            pnlSim.Controls.Remove(ctrl);

                        }
                    }
                }
                #endregion
            }
        }
        private void rdoDisOption_PatLay4_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDisOption_PatLay4.Checked)
            {
                if (Current_ptm != null)
                    SimulatePattern(pnlSim, Current_ptm, rdoPressSquaring.Checked, 4);
            }
            else
            {
                #region Clear Picture if all display are disabled
                if (!rdoDisOption_PatLay1.Checked && !rdoDisOption_PatLay2.Checked &&
                    !rdoDisOption_PatLay3.Checked && !rdoDisOption_PatLay4.Checked)
                {
                    for (int i = pnlSim.Controls.Count - 1; i >= 0; i--)
                    {
                        Control ctrl = pnlSim.Controls[i];
                        PictureBox PB = (PictureBox)ctrl;
                        if (PB.Name.Contains("Outbox"))
                        {
                            pnlSim.Controls.Remove(ctrl);

                        }
                    }
                }
                #endregion
            }
        }
        #endregion

        #region Button Squaring Display Options
        private void rdoOpenSquaring_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoOpenSquaring.Checked)
            {
                if (Current_ptm != null)
                    SimulatePattern(pnlSim, Current_ptm, rdoPressSquaring.Checked, GetSelectedLayer());
            }
        }
        private void rdoPressSquaring_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoPressSquaring.Checked)
            {
                if (Current_ptm != null)
                    SimulatePattern(pnlSim, Current_ptm, rdoPressSquaring.Checked, GetSelectedLayer());
            }
        }
        #endregion
        #endregion

        #region Button Check TieSheet
        private void chkbxTieSheet_CheckedChanged(object sender, EventArgs e)
        {
            numTieLayer.Enabled = chkbxTieSheet.Checked;
            if (chkbxTieSheet.Checked == false) //Top @ 16-07-2020
            {
                numTieLayer.Value = 0;
                chkbxTS_LowSQLayer.Enabled = false;
                chkbxTS_B4LastLY.Enabled = false;
                chkbxTS_LowSQLayer.Checked = false;
                chkbxTS_B4LastLY.Checked = false;
            }
            else
            {
                numTieLayer.Value = 1;
                chkbxTS_LowSQLayer.Enabled = true;
                chkbxTS_B4LastLY.Enabled = true;
            }
        }
        #endregion

        #region Button Check Finger Use
        private void chkbxFingerUse_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_UpdatingUI)
                return;
            if (flag_FingerUse_ReturnValue)
                return;
            //MatchingPattern();
            //this.OnPatternClick(sender, e);
            if (LastPB == null)
            {
                flag_FingerUse_ReturnValue = true;
                chkbxFingerUse.Checked = !chkbxFingerUse.Checked;
                flag_FingerUse_ReturnValue = false;
                return;
            }

            string loc_Pattern = LastPB.Name.Replace("pic", "");
            if (Check_DataNotEmpty(txbMatNO.Text.Trim(), Convert.ToInt16(numBDWidth.Value), Convert.ToInt16(numBDLength.Value)) == true)
            {
                PatternLay(loc_Pattern, loc_Pattern.Substring(1, 2));
                Check_PatternTypeInterlock(loc_Pattern,loc_Pattern.Substring(0, 1));
            }
            else
            {
                flag_FingerUse_ReturnValue = true;
                chkbxFingerUse.Checked = !chkbxFingerUse.Checked;
                flag_FingerUse_ReturnValue = false;
                return;
            }
            GetPatternOrder(loc_Pattern, pnlSim);
            btnSave.Enabled = false;
            flag_unCF_fingerChange = true;
            string buf_Msg = "Gripper Finger option has been changed.\n" +
                                "The save button will be blocked until click OK button.\n\n" +
                                "Please click OK to re-calculate the Pattern.";
            string buf_Topic = "Gripper Finger option has been changed";
            MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion

        #region Button Check Squaring Use
        private void chkbxSquaringUse_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbxSquaringUse.Checked == true)
            {
                if (Properties.Settings.Default.SquaringModel != -1)
                {
                    chkbxExtraSQ.Enabled = true;
                }
                else
                {
                    chkbxSquaringUse.Enabled = false;
                    chkbxSquaringUse.Checked = false;
                    chkbxExtraSQ.Enabled = false;
                    chkbxExtraSQ.Checked = false;
                    string buf_Msg = "There is no Squaring function for this version.";
                    string buf_Topic = "Error! - Version not compatible.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                chkbxExtraSQ.Enabled = false;
                chkbxExtraSQ.Checked = false;
            }
        }
        #endregion  

        #region Button PlaceMode
        private void rdoPlaceSoft_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoPlaceSoft.Checked)
                if (Properties.Settings.Default.Allow_SoftPlace)
                {
                    PlacingMode = 1;
                    rdoPlaceSoft.Enabled = true;
                }
                else
                {
                    string buf_Msg = "There is no SoftPlace function for this version.";
                    string buf_Topic = "Error! - Version not compatible.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rdoPlaceNormal.Checked = true;
                    rdoPlaceSoft.Enabled = false;
                    return;
                }
        }
        private void rdoPlaceNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoPlaceNormal.Checked)
            {
                PlacingMode = 2;
            }
        }
        private void rdoPlaceFast_CheckedChanged(object sender, EventArgs e) 
        {
            if (rdoPlaceFast.Checked)
                if (Properties.Settings.Default.Allow_FastPlace)
                {
                    rdoPlaceFast.Enabled = true;
                    PlacingMode = 3;
                }
                else
                {
                    string buf_Msg = "There is no FastPlace function for this version.";
                    string buf_Topic = "Error! - Version not compatible.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rdoPlaceNormal.Checked = true;
                    rdoPlaceFast.Enabled = false;
                    return;
                }
}
        #endregion

        #region Button DischargeMode
        private void rdoDCSlow_CheckedChanged(object sender, EventArgs e)
        {
                if (rdoDCSlow.Checked)
                    if (Properties.Settings.Default.Allow_SlowDCSpd)
                {
                    rdoDCSlow.Enabled = true;
                    DischargeMode = 1;
                    }
                    else
                    { 
                        string buf_Msg = "There is no IO for varying speed at PLC in this version.";
                        string buf_Topic = "Error! - Version not compatible.";
                        MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rdoDCNormal.Checked = true;
                        rdoDCSlow.Enabled = false;
                        return;            
                    }
        }
        private void rdoDCNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDCNormal.Checked)
            {
                DischargeMode = 2;
            }
        }
        private void rdoDCFast_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDCFast.Checked)
                if (Properties.Settings.Default.Allow_FastDCSpd)
                {
                    rdoDCFast.Enabled = true;
                    DischargeMode = 3;
                }
                else
                { 
                    string buf_Msg = "There is no IO for varying speed at PLC in this version.";
                    string buf_Topic = "Error! - Version not compatible.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rdoDCNormal.Checked = true;
                    rdoDCFast.Enabled = false;
                    return;            
                }
        }
        #endregion

        private void btnEditPallet_Click(object sender, EventArgs e) //@Top
        {
            using (frmPalletType frmPallet = new frmPalletType())
            {
                int buf_palWidth = pallets.Width;
                int buf_palLength = pallets.Length;
                int buf_palHeight = pallets.Height;
                frmPallet.pallets = pallets;
                frmPallet.ShowDialog();
                
                pallets = frmPallet.pallets;
                if ((buf_palWidth != pallets.Width) || (buf_palLength != pallets.Length) || (buf_palHeight != pallets.Height))
                {
                    //btnSave.Enabled = false;
                    btnSave.Enabled = true;
                    flag_unCF_palletChange = true;

                    string buf_Msg = "Pallet size has been changed.\n" +
                                        "The save button will be blocked until click OK button.\n\n" +
                                        "Please click OK to re-calculate the Pattern.";
                    string buf_Topic = "Pallet Size has been changed";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                lblPalletType.Text = pallets.Name;
                lblSizePallet.Text = string.Format("{0}x{1}x{2}", pallets.Width, pallets.Length, pallets.Height);
                //GetPatternOrder(Current_ptm.Pattern, pnlSim);
                //if (!VerifyPattern(Current_ptm.PatternBundle, Current_ptm))
                //    MatchingPattern();

            }
        }

        #region Button Rotate Pattern
        private void chkbxRotatePattern_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_RotatePat_ReturnValue)
                return;

            if (String.IsNullOrEmpty(Current_ptm.Pattern))
            {
                string buf_Msg = "No selected pattern.\n" +
                                    "the Program cannot calculate Pattern data.";
                string buf_Topic = "Error! - No Pattern is selected";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                chkbxRotatePattern.Checked = false;
            }
            else
            {
                if (GetPatternOrder(Current_ptm.Pattern, pnlSim)) //  ** ver3.0 2023-05-09
                {
                    //
                }
                else
                {
                    chkbxRotatePattern.Checked = !chkbxRotatePattern.Checked;
                }
            }

            if (flag_UpdatingUI)
                return;

            //*will only recalculate when it option checked is changed GetPatternOrder(Current_ptm.Pattern, panelSim);
        }
        #endregion

        #region Button Fix Bundle Face
        private void chkbxFixBDFace_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_UpdatingUI)
                return;
            if (flag_FixBDFace_ReturnValue)
                return;

            if (String.IsNullOrEmpty(Current_ptm.Pattern))
            {
                string buf_Msg = "No selected pattern.\n"+
                                    "the Program cannot Pattern data.";
                string buf_Topic = "Error! - No Pattern is selected";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                flag_FixBDFace_ReturnValue = true;
                chkbxFixBDFace.Checked = false;
                flag_FixBDFace_ReturnValue = false;
                return;
            }
            if (!Check_DataNotEmpty("NA", Convert.ToInt16(numBDWidth.Value), Convert.ToInt16(numBDLength.Value)))
            {
                string buf_Msg = "No selected pattern.\n" +
                                    "the Program cannot calculate Pattern data.";
                string buf_Topic = "Error! - No Pattern is selected";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                flag_FixBDFace_ReturnValue = true;
                chkbxFixBDFace.Checked = false;
                flag_FixBDFace_ReturnValue = false;
                return;
            }

            
            GetPatternOrder(Current_ptm.Pattern, pnlSim);
        }
        #endregion

        #region Modify Switching Pattern
        private void chkbxModSWPat_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_UpdatingUI)
                return;

            if (flag_ModSWPat_ReturnValue)
                return;

            if (flag_InGetPatternOrder)
                return;
            if (String.IsNullOrEmpty(Current_ptm.Pattern))
            {
                string buf_Msg = "No selected pattern.\n" +
                                    "the Program cannot calculate Pattern data.";
                string buf_Topic = "Error! - No Pattern is selected";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Error);
                flag_ModSWPat_ReturnValue = true;
                chkbxModSWPat.Checked = false;
                flag_ModSWPat_ReturnValue = false;
            }
            
            if (chkbxModSWPat.Checked == true)
            {
                chkbx1stSWPat.Enabled = true;
                chkbx2ndSWPat.Enabled = true;
                if (Current_ptm.Pattern.First() == 'S')
                {
                    chkbx3rdSWPat.Enabled = true;
                    chkbx4thSWPat.Enabled = true;
                }
                else
                {
                    chkbx3rdSWPat.Enabled = false;
                    chkbx4thSWPat.Enabled = false;
                }
                rdoDisOption_PatLay1.Enabled = chkbx1stSWPat.Checked;
                rdoDisOption_PatLay2.Enabled = chkbx2ndSWPat.Checked;
                rdoDisOption_PatLay3.Enabled = chkbx3rdSWPat.Checked;
                rdoDisOption_PatLay4.Enabled = chkbx4thSWPat.Checked;
            }
            else
            {
                chkbx1stSWPat.Enabled = false;
                chkbx2ndSWPat.Enabled = false;
                chkbx3rdSWPat.Enabled = false;
                chkbx4thSWPat.Enabled = false;
                Restore_PatternSeq();
            }

            Select_DisPatOption();
            Update_CurrentPatterSeq();
        }
        private void chkbx1stSWPat_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbx1stSWPat.Checked)
            {
                rdoDisOption_PatLay1.Enabled = true;
            }
            else
            {
                rdoDisOption_PatLay1.Enabled = false;
            }
            if (flag_UpdatingUI)
                return;
            if (flag_InGetPatternOrder)
                return;

            Select_DisPatOption();
            Update_CurrentPatterSeq();
        }
        private void chkbx2ndSWPat_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbx2ndSWPat.Checked)
            {
                rdoDisOption_PatLay2.Enabled = true;
            }
            else
            {
                rdoDisOption_PatLay2.Enabled = false;
            }
            if (flag_UpdatingUI)
                return;
            if (flag_InGetPatternOrder)
                return;

            Select_DisPatOption();
            Update_CurrentPatterSeq();
        }
        private void chkbx3rdSWPat_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbx3rdSWPat.Checked)
            {
                rdoDisOption_PatLay3.Enabled = true;
            }
            else
            {
                rdoDisOption_PatLay3.Enabled = false;
            }

            if (flag_UpdatingUI)
                return;
            if (flag_InGetPatternOrder)
                return;

            Select_DisPatOption();
            Update_CurrentPatterSeq();
        }
        private void chkbx4thSWPat_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbx4thSWPat.Checked)
            {
                rdoDisOption_PatLay4.Enabled = true;
            }
            else
            {
                rdoDisOption_PatLay4.Enabled = false;
            }
            if (flag_UpdatingUI)
                return;
            if (flag_InGetPatternOrder)
                return;

            Select_DisPatOption();
            Update_CurrentPatterSeq();
        }
        private void Update_CurrentPatterSeq()
        {
            if (String.IsNullOrEmpty(Current_ptm.SwitchPattern))
                Current_ptm.SwitchPattern = "0:0:0:0:0";

            char[] buf_SwitchPattern = Current_ptm.SwitchPattern.ToCharArray();
            buf_SwitchPattern[0] = chkbxModSWPat.Checked ? '1' : '0';
            buf_SwitchPattern[2] = chkbx1stSWPat.Checked ? '1' : '0';
            buf_SwitchPattern[4] = chkbx2ndSWPat.Checked ? '1' : '0';
            buf_SwitchPattern[6] = chkbx3rdSWPat.Checked ? '1' : '0';
            buf_SwitchPattern[8] = chkbx4thSWPat.Checked ? '1' : '0';

            string buf_ModSwitchPattern = new string(buf_SwitchPattern);
            Current_ptm.SwitchPattern = buf_ModSwitchPattern;
        }
        private void Restore_PatternSeq()
        {
            chkbx1stSWPat.Checked = true;
            chkbx2ndSWPat.Checked = true;
            chkbx3rdSWPat.Checked = Current_ptm.Pattern[0] == 'S'? true : false;
            chkbx4thSWPat.Checked = Current_ptm.Pattern[0] == 'S' ? true : false;
        }
        private void Select_DisPatOption()
        {
            rdoDisOption_PatLay4.Checked = rdoDisOption_PatLay4.Enabled;
            rdoDisOption_PatLay3.Checked = rdoDisOption_PatLay3.Enabled;
            rdoDisOption_PatLay2.Checked = rdoDisOption_PatLay2.Enabled;
            rdoDisOption_PatLay1.Checked = rdoDisOption_PatLay1.Enabled;
        }
        #endregion

        #region Button Save
        private void btnSave_Click(object sender, EventArgs e)
        {
            
            if (chkbxModSWPat.Checked && !chkbx1stSWPat.Checked && !chkbx2ndSWPat.Checked && !chkbx3rdSWPat.Checked && !chkbx4thSWPat.Checked)
            {
                string buf_Msg = "All Switching Patterns are unselected.\n"+
                                    "Please selected atleast one of them or unselect 'Modify Switching Pattern' option.";
                string buf_Topic = "Error! - No Switching Pattern is selected.";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (Compare_BundleHeight(numBundleHeight.Value, CalBundleHeight) == false) // Comparing numBundleHeight vs CalBundleHeight(Assigned at AutoFindPattern _FlutexPPB)
                return;

            if (Check_CompletedHeight(numLYPerPallet.Value, numBundleHeight.Value) == false)
                return;

            //if (flag_unCF_boxChange || flag_unCF_fluteChange || flag_unCF_bdsizeChange || flag_unCF_palletChange)
            if (flag_unCF_boxChange || flag_unCF_fluteChange || flag_unCF_bdsizeChange)
                return;

            if (!flag_GotPatternOrder)
                return;

            string Query = string.Format(@"UPDATE tblOrder SET Material_No  = '{0}' WHERE Material_No = '{0}' AND (OrderState = {1} OR OrderState = {2})", txbMatNO.Text, 1, 2);
            int RowEffect = Sql.ExcSQLwithReturn(Query);

            if (RowEffect == 1)
            {
                string buf_Msg = "There is the data with the same MaterialNo that has already been sent to the Robot.\n" +
                                    "If you want to save this new setting.\n" +
                                    "Please ResetData or LotEnd current data first.";
                string buf_Topic = "Error! - Can not Save New Data.";

                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            SaveProduct(txbMatNO.Text);
        }
        private bool Compare_BundleHeight(decimal loc_BDHeight, decimal loc_Cal_BDHeight)
        {
            bool IsOK = false;

            //Get Min Bundle Height
            decimal MinBundleHeight = 0;
            try
            {
                //Check Min Bundle Height
                MinBundleHeight = Convert.ToDecimal(loc_Cal_BDHeight - (MinBundleHeight));
                if (loc_BDHeight < (MinBundleHeight) && Properties.Settings.Default.BD_NewData_ClearHeight)
                {
                    string buf_Msg = "Do you want to use this Height(" + loc_BDHeight + ")?\r" +
                                        "It is less than system calulated (" + MinBundleHeight + ").\n\n"+
                                        "Press YES to confirm using this Value and save Master Data.\n"+
                                        "Press NO to return to setting.";
                    string buf_Topic = "Bundle Height Confirmation";
                    var dresp = MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dresp == DialogResult.No)
                        return false;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message, LogType.Fail);
            }

            if (numBundleHeight.Value <= 0)
            {
                string buf_Msg = "Bundle Height value can not be less or equal zero.";
                string buf_Topic = "Error! - Bundle Size is out of spec.";
                MessageBox.Show(buf_Msg,buf_Topic , MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            IsOK = true;

            return IsOK;
        }
        private bool Check_CompletedHeight(decimal loc_LYPerPallet, decimal loc_BDHeight)
        {
            bool IsOK = false;

            decimal loc_OrderHeight = loc_LYPerPallet * loc_BDHeight;
            decimal loc_MaxCompleteHeight = Convert.ToDecimal(Properties.Settings.Default.Max_CompleteHeight);

            if (loc_OrderHeight > loc_MaxCompleteHeight)
            {
                MessageBox.Show("Completed product on the pallet can not exceed "+ loc_MaxCompleteHeight + " mm."+
                    "\nCurrent height: "+ loc_OrderHeight + " mm."+
                    "\nPlease reduce the height either by \n - reduce Bundle Height\n - reduce Layer per Pallet", "Error! - Completed height will exceed the limit.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                IsOK = false;
            }
            else
            {
                IsOK = true;
            }

            return IsOK;
        }
        private void SaveProduct(string MaterialNo)
        {
            string Query;
            int ExecResult;
            #region final update any Current_ptm field

            #endregion
            // Check if this MatNO is already in 'tblMaster' and ask for a save confirmation if found.
            Query = "SELECT count([Material_No]) FROM [tblMaster] WHERE Material_No =  '@MatNo'";
            Query = Query.Replace("@MatNo", MaterialNo);
            int CNTMaster = (int)Sql.GetScalarFromSql(Query);

            if (CNTMaster > 0)
            {
                string buf_Msg = "There is already information for :" + txbMatNO.Text + "in the Database.\n\n" +
                                    "Do you want to update that information with new data?\n" +
                                    "Press YES to Update it with your new data\n"+
                                    "Press No to Cancel and go back to setting";
                string buf_Topic = "Update old data Confirmation";
                DialogResult dr = MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dr != DialogResult.Yes)
                {
                    return;
                }
            }

            SqlTransaction STrans = null;
            SqlConnection SaveMasterConn = null;
            try
            {
                using (SaveMasterConn = new SqlConnection(Sql.connString))
                {
                    DataTable dtMaster = new DataTable();
                    SqlDataAdapter daMaster = new SqlDataAdapter();
                    DataTable dtPatterns = new DataTable();
                    SqlDataAdapter daPatterns = new SqlDataAdapter();
                    DataTable dtPlacement = new DataTable();
                    SqlDataAdapter daPlacement = new SqlDataAdapter();
                    SqlCommand cmdTemp = new SqlCommand();

                    SaveMasterConn.Open();
                    STrans = SaveMasterConn.BeginTransaction(IsolationLevel.Serializable, "SaveMasterData");

                    //Get Last SO
                    string SO_OLD = "";
                    string SO = lblSO.Text;
                    Query = string.Format("SELECT TOP 1 Lot_No FROM tblOrder WHERE Material_No = '{0}' ORDER BY StampDate DESC", MaterialNo);
                    cmdTemp.Connection = SaveMasterConn;
                    cmdTemp.Transaction = STrans;
                    cmdTemp.CommandText = Query;
                    var ExeSO = cmdTemp.ExecuteScalar();
                    if (ExeSO != null)
                    {
                        SO_OLD = ExeSO.ToString();
                    }

                    //== tblMaster
                    //find MaterialNo in 'tblMaster'
                    //if found update that row
                    //if not add new data to added row
                    Query = string.Format("SELECT * FROM tblMaster WHERE Material_No = '{0}'", MaterialNo);
                    dtMaster = Sql.GetDataTableFromSqlwithCUD(ref daMaster, Query, SaveMasterConn, STrans);
                    if (dtMaster.Rows.Count == 0)
                    {
                        //Insert Master
                        DataRow rNewMaster = dtMaster.Rows.Add();
                        rNewMaster["StampDate"] = DateTime.Now;
                        rNewMaster["Product_Code"] = txbProductCode.Text;
                        rNewMaster["ActualBox_L"] = numBoxLength.Value;
                        rNewMaster["ActualBox_W"] = numBoxWidth.Value;
                        rNewMaster["ActualBox_H"] = numBoxHeight.Value;
                        rNewMaster["Box_Type"] = cmbBoxType.Text;
                        rNewMaster["Flute_Type"] = cmbFluteType.Text;
                        rNewMaster["PiecePerBD"] = numPiecePerBD.Value;
                        rNewMaster["BDPerLY"] = numBDPerLY.Value;
                        rNewMaster["PiecePerLY"] = numPiecePerLY.Value;
                        rNewMaster["LYPerPallet"] = numLYPerPallet.Value;

                        //top Edit 14-04-2020
                        rNewMaster["Pallet_Type"] = pallets.Name;
                        rNewMaster["Pallet_W"] = pallets.Width;
                        rNewMaster["Pallet_L"] = pallets.Length;
                        rNewMaster["Pallet_H"] = pallets.Height;

                        rNewMaster["BDPerGrip"] = numBDPerGrip.Value;
                        rNewMaster["Pattern_Code"] = Current_ptm.Pattern;
                        rNewMaster["Efficiency"] = Efficiency; // this will be update at Cal_Efficiency which will be call inside GetPatternOrder
                        rNewMaster["TopSheet"] = chkbxTopSheet.Checked;
                        if (chkbxBtmSheet.Checked) 
                        {
                            if(chkbxBtmSheetByRobot.Checked)
                                rNewMaster["BtmSheet"] = 2;
                            else
                                rNewMaster["BtmSheet"] = 1;
                        }
                        else
                            rNewMaster["BtmSheet"] = 0;
                        rNewMaster["TS_everyXLY"] = numTieLayer.Value;
                        rNewMaster["FoldWork_W"] = numBDWidth.Value;
                        rNewMaster["FoldWork_L"] = numBDLength.Value;
                        rNewMaster["FoldWork_T"] = (double)numBundleHeight.Value / (double)numPiecePerBD.Value;
                        rNewMaster["FoldWork_Weight"] = Math.Round(numBoxWeight.Value, 3);
                        rNewMaster["Material_No"] = txbMatNO.Text;
                        rNewMaster["Lot_No"] = (string.IsNullOrEmpty(SO) || SO == "SO :" ? SO_OLD : lblSO.Text.Replace("SO :", "")); //SO_OLD = "",
                        rNewMaster["Peri_OpenL"] = Current_ptm.LayerOpenPeriLength; // this will be update at Get_PlacementInfo which will be call inside GetPatternOrder
                        rNewMaster["Peri_OpenW"] = Current_ptm.LayerOpenPeriWidth; // this will be update at Get_PlacementInfo which will be call inside GetPatternOrder
                        rNewMaster["FixBDFace"] = chkbxFixBDFace.Checked;
                        rNewMaster["RotatePattern"] = (chkbxRotatePattern.Checked ? 1 : -1); // remove sub RotPat checkbox ver3.0 2023-05-8
                        rNewMaster["PatternSeq"] = Current_ptm.SwitchPattern;
                        rNewMaster["Squaring"] = chkbxSquaringUse.Checked;
                        rNewMaster["SQ_OpenX"] = SQ_OPX; // this will be update at GetPatternOrder
                        rNewMaster["SQ_OpenY"] = SQ_OPY; // this will be update at GetPatternOrder
                        rNewMaster["SQ_CloseX"] = SQ_CLX; // this will be update at GetPatternOrder
                        rNewMaster["SQ_CloseY"] = SQ_CLY; // this will be update at GetPatternOrder
                        rNewMaster["GripperFinger"] = chkbxFingerUse.Checked;
                        rNewMaster["SwitchBDSize"] = chkbxSwapLW.Checked;
                        rNewMaster["Peri_CloseL"] = LayerClosePeriLength; // this will be update at Get_PlacementInfo which will be call inside GetPatternOrder
                        rNewMaster["Peri_CloseW"] = LayerClosePeriWidth; // this will be update at Get_PlacementInfo which will be call inside GetPatternOrder
                        rNewMaster["SpecialRotate"] = (chkbxSpecialRotate.Checked ? true : false);

                        //TieSheet
                        rNewMaster["TS_B4LastLY"] = chkbxTS_B4LastLY.Checked;
                        rNewMaster["TS_SQLayer"] = chkbxTS_LowSQLayer.Checked;

                        //Special Stack & SQ @29-10-2020
                        rNewMaster["StackerLiftBD"] = chkbxLiftStack.Checked;
                        rNewMaster["SQ_Extra"] = chkbxExtraSQ.Checked;
                        rNewMaster["ExtraPickDepth"] = chkbxExtraPick.Checked;
                        rNewMaster["PlacingMode"] = PlacingMode;
                        rNewMaster["DischargeMode"] = DischargeMode;
                        rNewMaster["AntiBounce"] = chkbxAntiBounce.Checked;
                        rNewMaster["HeightRatio"] = HeightRatio;
                    }
                    else
                    {
                        //Update Master
                        DataRow rOldMaster = dtMaster.Rows[0];
                        rOldMaster["StampDate"] = DateTime.Now;
                        rOldMaster["Product_Code"] = txbProductCode.Text;
                        rOldMaster["ActualBox_L"] = numBoxLength.Value;
                        rOldMaster["ActualBox_W"] = numBoxWidth.Value;
                        rOldMaster["ActualBox_H"] = numBoxHeight.Value;
                        rOldMaster["Box_Type"] = cmbBoxType.Text;
                        rOldMaster["Flute_Type"] = cmbFluteType.Text;
                        rOldMaster["PiecePerBD"] = numPiecePerBD.Value;
                        rOldMaster["BDPerLY"] = numBDPerLY.Value;
                        rOldMaster["PiecePerLY"] = numPiecePerLY.Value;
                        rOldMaster["LYPerPallet"] = numLYPerPallet.Value;

                        //top Edit 14-04-2020
                        rOldMaster["Pallet_Type"] = pallets.Name;
                        rOldMaster["Pallet_W"] = pallets.Width;
                        rOldMaster["Pallet_L"] = pallets.Length;
                        rOldMaster["Pallet_H"] = pallets.Height;

                        rOldMaster["BDPerGrip"] = numBDPerGrip.Value;
                        rOldMaster["Pattern_Code"] = Current_ptm.Pattern;
                        rOldMaster["Efficiency"] = Efficiency; // this will be update at Cal_Efficiency which will be call inside GetPatternOrder
                        rOldMaster["TopSheet"] = chkbxTopSheet.Checked;
                        if (chkbxBtmSheet.Checked)
                        {
                            if (chkbxBtmSheetByRobot.Checked)
                                rOldMaster["BtmSheet"] = 2;
                            else
                                rOldMaster["BtmSheet"] = 1;
                        }
                        else
                            rOldMaster["BtmSheet"] = 0;

                        rOldMaster["TS_everyXLY"] = numTieLayer.Value;
                        rOldMaster["FoldWork_W"] = numBDWidth.Value;
                        rOldMaster["FoldWork_L"] = numBDLength.Value;
                        rOldMaster["FoldWork_T"] = (double)numBundleHeight.Value / (double)numPiecePerBD.Value;
                        rOldMaster["FoldWork_Weight"] = Math.Round(numBoxWeight.Value, 3);
                        rOldMaster["Material_No"] = txbMatNO.Text;
                        rOldMaster["Lot_No"] = (string.IsNullOrEmpty(SO) || SO == "SO :" ? SO_OLD : lblSO.Text.Replace("SO :", ""));
                        rOldMaster["Peri_OpenL"] = Current_ptm.LayerOpenPeriLength; // this will be update at Get_PlacementInfo which will be call inside GetPatternOrder
                        rOldMaster["Peri_OpenW"] = Current_ptm.LayerOpenPeriWidth; // this will be update at Get_PlacementInfo which will be call inside GetPatternOrder
                        rOldMaster["FixBDFace"] = chkbxFixBDFace.Checked;
                        rOldMaster["RotatePattern"] = (chkbxRotatePattern.Checked ? 1 : -1); // remove sub RotPat checkbox ver3.0 2023-05-8
                        rOldMaster["PatternSeq"] = Current_ptm.SwitchPattern;
                        rOldMaster["Squaring"] = chkbxSquaringUse.Checked;
                        rOldMaster["SQ_OpenX"] = SQ_OPX; // this will be update at GetPatternOrder
                        rOldMaster["SQ_OpenY"] = SQ_OPY; // this will be update at GetPatternOrder
                        rOldMaster["SQ_CloseX"] = SQ_CLX; // this will be update at GetPatternOrder
                        rOldMaster["SQ_CloseY"] = SQ_CLY; // this will be update at GetPatternOrder

                        rOldMaster["GripperFinger"] = chkbxFingerUse.Checked;
                        rOldMaster["SwitchBDSize"] = chkbxSwapLW.Checked;
                        rOldMaster["Peri_CloseL"] = LayerClosePeriLength; // this will be update at Get_PlacementInfo which will be call inside GetPatternOrder
                        rOldMaster["Peri_CloseW"] = LayerClosePeriWidth; // this will be update at Get_PlacementInfo which will be call inside GetPatternOrder
                        rOldMaster["SpecialRotate"] = (chkbxSpecialRotate.Checked ? true : false);

                        //TieSheet
                        rOldMaster["TS_B4LastLY"] = chkbxTS_B4LastLY.Checked;
                        rOldMaster["TS_SQLayer"] = chkbxTS_LowSQLayer.Checked;

                        //Special Stack & SQ @29-10-2020
                        rOldMaster["StackerLiftBD"] = chkbxLiftStack.Checked;
                        rOldMaster["SQ_Extra"] = chkbxExtraSQ.Checked;
                        rOldMaster["ExtraPickDepth"] = chkbxExtraPick.Checked;
                        rOldMaster["PlacingMode"] = PlacingMode;
                        rOldMaster["DischargeMode"] = DischargeMode;
                        rOldMaster["AntiBounce"] = chkbxAntiBounce.Checked;
                        rOldMaster["HeightRatio"] = HeightRatio;
                    }
                    daMaster.Update(dtMaster);

                    //== tblPlacement
                    //Clear exiting PlacementInfo
                    // - Delete existing Rows that has the matching MaterialNO
                    Query = string.Format("SELECT * FROM tblPlacement WHERE Material_No = '{0}'", MaterialNo);
                    dtPlacement = Sql.GetDataTableFromSqlwithCUD(ref daPlacement, Query, SaveMasterConn, STrans);
                    if (dtPlacement.Rows.Count > 0)
                    {
                        for (int i = dtPlacement.Rows.Count - 1; i >= 0; i--)
                        {
                            dtPlacement.Rows[i].Delete();
                        }
                    }

                    //Update new PlacementInfo to tblPlacement
                    for (int i = 0; i < OpenBundles.Length; i++)
                    {
                        DataRow rNewPlacement = dtPlacement.Rows.Add();
                        rNewPlacement["Material_No"] = txbMatNO.Text;
                        rNewPlacement["Step"] = OpenBundles[i].Step;
                        rNewPlacement["X"] = OpenBundles[i].X;
                        rNewPlacement["Y"] = OpenBundles[i].Y;
                        rNewPlacement["Degree"] = OpenBundles[i].Ori;
                        rNewPlacement["Layer"] = OpenBundles[i].Layer;
                        rNewPlacement["PatternName"] = OpenBundles[i].PatternName;
                    }
                    daPlacement.Update(dtPlacement);

                    //== tblOrder
                    // - Delete existing Rows that has the matching MaterialNO
                    Query = string.Format("DELETE FROM tblOrder WHERE Material_No = '{0}' AND OrderState = 0", MaterialNo);
                    cmdTemp.Connection = SaveMasterConn;
                    cmdTemp.Transaction = STrans;
                    cmdTemp.CommandText = Query;
                    ExecResult = cmdTemp.ExecuteNonQuery();
                    if (ExecResult == -1)
                    {
                        STrans.Rollback("SaveMasterData");
                        if (SaveMasterConn.State != ConnectionState.Closed)
                            SaveMasterConn.Close();

                        Log.WriteLog("Can not DELETE tblOrder of MaterialNo:" + MaterialNo, LogType.Fail);
                        //**return;
                    }

                    //Update new OrderInfo from tblMaster to tblOrder that has a matching MaterialNO
                    Query = string.Format("INSERT INTO tblOrder(" +
                                                "[StampDate],[OrderState],[Material_No],[Product_Code],[ActualBox_L],[ActualBox_W],[ActualBox_H],[Lot_No],[Box_Type],[Flute_Type],[PiecePerBD],[BDPerLY]" +
                                                ",[PiecePerLY],[LYPerPallet],[Pallet_Type],[Pallet_W],[Pallet_L],[Pallet_H],[BDPerGrip],[Pattern_Code],[Efficiency],[Robot_Speed]" +
                                                ",[TopSheet],[TS_everyXLY],[BtmSheet],[FoldWork_W],[FoldWork_L],[FoldWork_T],[FoldWork_Weight],[Peri_OpenL],[Peri_OpenW],[FixBDFace]" +
                                                ",[Squaring],[SQ_OpenX],[SQ_OpenY],[SQ_CloseX],[SQ_CloseY],[GripperFinger],[Peri_CloseL],[Peri_CloseW],[RotatePattern],[SwitchBDSize]" +
                                                ",[SpecialRotate], [TS_B4LastLY], [TS_SQLayer], [StackerLiftBD], [SQ_Extra], [PlacingMode],[PatternSeq]" +
                                                ",[ExtraPickDepth], [DischargeMode], [AntiBounce],[HeightRatio])" +
                                            "SELECT [StampDate],{1},[Material_No],[Product_Code],[ActualBox_L],[ActualBox_W],[ActualBox_H],[Lot_No],[Box_Type],[Flute_Type],[PiecePerBD],[BDPerLY]" +
                                                ",[PiecePerLY],[LYPerPallet],[Pallet_Type],[Pallet_W],[Pallet_L],[Pallet_H],[BDPerGrip],[Pattern_Code],[Efficiency],[Robot_Speed]" +
                                                ",[TopSheet],[TS_everyXLY],[BtmSheet],[FoldWork_W],[FoldWork_L],[FoldWork_T],[FoldWork_Weight],[Peri_OpenL],[Peri_OpenW],[FixBDFace]" +
                                                ",[Squaring],[SQ_OpenX],[SQ_OpenY],[SQ_CloseX],[SQ_CloseY],[GripperFinger],[Peri_CloseL],[Peri_CloseW],[RotatePattern],[SwitchBDSize]" +
                                                ",[SpecialRotate], [TS_B4LastLY], [TS_SQLayer], [StackerLiftBD], [SQ_Extra], [PlacingMode],[PatternSeq]" +
                                                ",[ExtraPickDepth], [DischargeMode], [AntiBounce],[HeightRatio]" +
                                                "From tblMaster Where Material_No = '{0}'", MaterialNo,0);

                    cmdTemp.Connection = SaveMasterConn;
                    cmdTemp.Transaction = STrans;
                    cmdTemp.CommandText = Query;

                    ExecResult = cmdTemp.ExecuteNonQuery();
                    if (ExecResult == -1)
                    {
                        STrans.Rollback("SaveMasterData");
                        if (SaveMasterConn.State != ConnectionState.Closed)
                            SaveMasterConn.Close();

                        Log.WriteLog("Can not INSERT tblOrder of MaterialNo:" + MaterialNo, LogType.Fail);
                        return;
                    }

                    STrans.Commit();
                    SaveMasterConn.Close();

                    Log.WriteLog("---[Start]Save MaterialNo:" + MaterialNo + "---", LogType.Notes);
                    Log.WriteLog(string.Format("PT:{0}, BL:{1}, BW:{2}, SQ_OPX:{3}, SQ_OPY:{4}, SQ_CLX:{5}, SQ_CLY:{6}, LY_WIDTH:{7}, LY_LENGHT:{8}, SwapSide:{9}, SpecialRotate:{10}, TSNearlyLast:{11}, TSByPassSQHigh:{12}", Current_ptm.Pattern, numBDLength.Value, numBDWidth.Value, SQ_OPX, SQ_OPY, SQ_CLX, SQ_CLY, LayerClosePeriWidth, LayerClosePeriLength, (chkbxSwapLW.Checked ? 1 : -1), (chkbxSpecialRotate.Checked ? 1 : -1), (chkbxTS_B4LastLY.Checked ? 1 : -1), (chkbxTS_LowSQLayer.Checked ? 1 : -1)), LogType.Notes);
                    string _PlacementXY = "";
                    for (int i = 0; i < OpenBundles.Length; i++)
                    {
                        _PlacementXY += string.Format("[{0}]({1},{2},{3})", OpenBundles[i].Step, OpenBundles[i].X, OpenBundles[i].Y, OpenBundles[i].Ori);
                        if (i != OpenBundles.Length - 1)
                            _PlacementXY += ", ";
                    }
                    OrderModel buf_odm = new OrderModel();
                    buf_odm.BundleHeight = Convert.ToInt32(numBundleHeight.Value);
                    buf_odm.TS_LowSQLayer = chkbxTS_LowSQLayer.Checked;
                    buf_odm.TS_B4LastLY = chkbxTS_B4LastLY.Checked;
                    string _ARR_TieSheet = buf_odm.GetArrTieSheet((int)numTieLayer.Value, (int)numBDPerGrip.Value, (int)numLYPerPallet.Value);
                    Log.WriteLog(string.Format("Placement:{0}", _PlacementXY), LogType.Notes);
                    Log.WriteLog(string.Format("TieSheet:{0}", _ARR_TieSheet), LogType.Notes);
                    Log.WriteLog("---[Finish]Save MaterialNo:" + MaterialNo + "---", LogType.Notes);
                    Log.WriteLog("Save master data of Material:" + MaterialNo + " complete.", LogType.Success);

                    string buf_Msg = "Save master data of Material:" + MaterialNo + " complete.";
                    string buf_Topic = "Succesfully save Master Data";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //== Update Order Grid
                    var _fOrder = UICls.FindOpenForm("frmOrder");
                    if (_fOrder != null)
                    {
                        frmOrder f = (frmOrder)_fOrder;
                        f.OrderGrid_ImportOrderData();
                    }

                    var buf_fMain = UICls.FindOpenForm("frmMain");
                    if (buf_fMain != null)
                    {
                        frmMain fMain = (frmMain)buf_fMain;
                        fMain.Disable_MainMenu(true, false, false, false);
                    }
                    this.Close();
                    this.Dispose();
                    /*//== Show Order Grid
                    var _fMain = UICls.FindOpenForm("frmMain");
                    if (_fMain != null)
                    {
                        frmMain f = (frmMain)_fMain;
                        f.ShowOrderForm();
                    }*/
                }
            }
            catch (Exception e)
            {
                if (STrans != null && STrans.Connection != null)
                {
                    STrans.Rollback("SaveMasterData");
                }

                if (SaveMasterConn.State != ConnectionState.Closed)
                    SaveMasterConn.Close();

                Log.WriteLog("Can't save master data of Material:" + MaterialNo + "." + e.Message, LogType.Fail);
            }
        }
        #endregion

        #region Button Close
        private void btnClose_Click(object sender, EventArgs e)
        {
            btnSave.Text = "SAVE";
            btnSave.BackColor = Color.ForestGreen;
            var buf_fMain = UICls.FindOpenForm("frmMain");
            if (buf_fMain != null)
            {
                frmMain fMain = (frmMain)buf_fMain;
                fMain.Disable_MainMenu(true, false, false, false);
            }
            this.Close();
            this.Dispose();
        }
        #endregion

        private void cmbBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (flag_UpdatingUI)
            //    return;

            //flag_unCF_boxChange = true;
            //MessageBox.Show("Selected item changed to: " + cmbBoxType.SelectedItem.ToString());
        }

        private void cmbFluteType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (flag_UpdatingUI)
            //    return;

            //flag_unCF_fluteChange = true;
            //MessageBox.Show("Selected item changed to: " + cmbFluteType.SelectedItem.ToString());
        }

        private void numPiecePerBD_ValueChanged(object sender, EventArgs e)
        {
            if (numPiecePerBD.Value > 0)
                numBundleHeight.Enabled = true;
        }

        private void numBDPerGrip_ValueChanged(object sender, EventArgs e)
        {
            if (numBDPerGrip.Value == 2)
            {
                if ((numTieLayer.Value % 2) != 0)
                {
                    string buf_Msg = "From your setting\n"+
                                        "Robot will grip a stacked of "+numBDPerGrip.Value+" bundles at a time\n"+
                                        "Which make it impossible to place TieSheet every "+numTieLayer.Value+" Layer(s)\n\n"+ 
                                        "The System will have to adjust the odd TieSheet Layer to closest even Layer.\n" +
                                        "for example:\t\n" +
                                        "if place every 3 Layer\n" +
                                        "\t" + "3rd\t" + ">\t" + "4th  (changed)\n" +
                                        "\t" + "6th\t" + ">\t" + "6th  (not change)\n" +
                                        "\t" + "9th\t" + ">\t" + "10th  (changed)";
                    string buf_Topic = "TieSheet Layer has been adjusted!";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (chkbxTS_B4LastLY.Checked && numLYPerPallet.Value % 2 == 0 )
                {
                    string OrdinalSuffix1 = numLYPerPallet.Value - 1 > 3 ? "th" : numLYPerPallet.Value - 1 == 3 ? "rd" : numLYPerPallet.Value - 1 == 2 ? "nd" : "st";
                    string OrdinalSuffix2 = numLYPerPallet.Value - 2 > 3 ? "th" : numLYPerPallet.Value - 2 == 3 ? "rd" : numLYPerPallet.Value - 2 == 2 ? "nd" : "st";

                    string buf_Msg = "From your setting\n" +
                                        "Robot will grip a stacked of " + numBDPerGrip.Value + " bundles at a time\n" +
                                        "Which make it impossible to place TieSheet on Layer before LastLayer: " + numLYPerPallet.Value + OrdinalSuffix1 + "\n\n";
                    string buf_Topic = "TieSheet Before LastLayer has been adjusted!";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
        }
        
        private void chkbxTS_B4LastLY_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_UpdatingUI)
                return;

            if (numBDPerGrip.Value == 2)
            {
                if (chkbxTS_B4LastLY.Checked && numLYPerPallet.Value % 2 == 0)
                {
                    string OrdinalSuffix1 = numLYPerPallet.Value - 1 > 3 ? "th" : numLYPerPallet.Value - 1 == 3 ? "rd" : numLYPerPallet.Value - 1 == 2 ? "nd" : "st";
                    string OrdinalSuffix2 = numLYPerPallet.Value - 2 > 3 ? "th" : numLYPerPallet.Value - 2 == 3 ? "rd" : numLYPerPallet.Value - 2 == 2 ? "nd" : "st";

                    string buf_Msg = "From your setting\n" +
                                        "Robot will grip a stacked of " + numBDPerGrip.Value + " bundles at a time\n" +
                                        "Which make it impossible to place TieSheet on Layer before LastLayer: " + numLYPerPallet.Value + OrdinalSuffix1 + "\n\n" +
                                        "The System will have to adjust the TieSheet Layer to new data.\n" +
                                        "The Robot will place Last TieSheet on Layer " + (numLYPerPallet.Value - numBDPerGrip.Value) + OrdinalSuffix2 + " instead.";
                    string buf_Topic = "TieSheet Before LastLayer has been adjusted!";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }

        }

        private void btnHeightRatio_Click(object sender, EventArgs e)
        {
            string buf_Msg;
            string buf_Topic = "Height Ratio has been updated.";
            if (HeightRatio == HeightRatio_Master)
            {
                HeightRatio = 100;
                buf_Msg = "Bundle Height Ratio has been reset to 100%\n"+
                            "Click again to reset to Master Value ("+HeightRatio_Master+"%)";
            }
            else
            {
                HeightRatio = HeightRatio_Master;
                buf_Msg = "Bundle Height Ratio has been reset to Master Value (" + HeightRatio + "%)\n"+
                            "Click again to reset to 100%";
            }
            MessageBox.Show(buf_Msg,buf_Topic,MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnHeightRatio.Text = HeightRatio.ToString()+"%";
        }

        private void chkbxBtmSheet_CheckedChanged(object sender, EventArgs e)
        {
            chkbxBtmSheetByRobot.Enabled = chkbxBtmSheet.Checked;
            if (chkbxBtmSheet.Checked == false)
                chkbxBtmSheetByRobot.Checked = false;
        }

        private void chkbxLiftStack_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbxLiftStack.Checked == true)
            {
                chkbxExtraPick.Enabled = true;
            }
            else
            {
                chkbxExtraPick.Enabled = false;
                chkbxExtraPick.Checked = false;
            }
        }
    }
}
