using OMRON.Compolet.CIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATD_ID4P.Model;
using System.Diagnostics;
using System.Windows.Forms;

namespace ATD_ID4P.Class
{
    public class PLCCommu
    {
        public bool PLCActive { get; set; }
        private readonly NXCompolet PLCLink = new NXCompolet();
        private readonly LogCls Log = new LogCls();
        private bool PauseCheck = false;

        public bool CheckPLCIsActive(string IP)
        {
            try
            {
                //*** ByPass For Dry Test:
                if (PauseCheck == true)
                    return false;
                // test commit
                if (NetworkCls.CheckPing(IP) == true)
                {
                    string routePath = "2%" + IP;
                    PLCLink.PeerAddress = IP;
                    PLCLink.RoutePath = routePath;
                    PLCLink.Active = true;
                    PLCActive = true;

                    var PLCService = CheckPLCService(IP);
                    if (!PLCService)
                    {
                        PLCActive = false;
                        PauseCheck = true;
                        System.Windows.Forms.Form _frm;
                        using (_frm = new System.Windows.Forms.Form())
                        {
                            _frm.TopMost = true;
                            string buf_Msg = "PLC Service return false.\n"+
                                                "Please check if you already start Sysmac Gateway Console.\n"+
                                                "and it's currently working normally.";
                            string buf_Topic = "Error! - Cannot check PLC Service (False).";
                            System.Windows.Forms.MessageBox.Show(_frm, buf_Msg, buf_Topic, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }

                    }
                }
                else
                {
                    PLCActive = false;
                    PauseCheck = false;
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - Cannot check PLC Service: " + e.Message, LogType.Fail);
                PLCActive = false;
                PauseCheck = false;
            }

            return PLCActive;
        }

        public bool CheckPLCService(string IP = "")
        {
            bool result = false;

            try
            {
                if (string.IsNullOrEmpty(IP))
                    IP = Properties.Settings.Default.IP_PLC;

                var SysmacApp = Process.GetProcessesByName("CIPCore");
                if (SysmacApp != null && SysmacApp.Count() > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - Cannot check PLC Service: " + e.Message, LogType.Fail);
            }

            return result;
        }

        public Tuple<bool, object> GetPLCVariable(FieldPLCs FP, string Type = "", KawaPLCField KP = 0)
        {
            bool IsComplete;
            object result = null;

            if (Type == "" || Type == "Boolean")
                Type = "bool";

            try
            {
                if (KP == 0 && FP != FieldPLCs.Empty)
                    result = PLCLink.ReadVariable(FP.ToString());
                else
                    result = PLCLink.ReadVariable(KP.ToString());

                if (Type != "")
                {
                    var _Var = AutoConvertDataType(result, Type, FP.ToString());
                    if (_Var.Item1 == false)
                    {
                        IsComplete = false;
                        result = "Convert data error.";
                    }
                    else
                    {
                        IsComplete = true;
                        result = _Var.Item2;
                    }
                }
                else
                {
                    IsComplete = true;
                }
            }
            catch (Exception e)
            {
                IsComplete = false;
                Log.WriteLog("Error! - Cannot read PLC data [" + FP.ToString() + "]: " + e.Message, LogType.Fail);
            }

            return new Tuple<bool, object>(IsComplete, result);
        }

        public Tuple<bool, object> SetPLCVariable(FieldPLCs FP, object Value, string Type = "")
        {
            bool IsComplete;
            object result = null;

            //--ByPas PLC ***Test Only***---
            //return new Tuple<bool, object>(true, Value);

            try
            {
                PLCLink.WriteVariable(FP.ToString(), Value);
                Type = (string.IsNullOrEmpty(Type) ? GetDataTypefromValue(Value) : Type);
                if (!string.IsNullOrEmpty(Type))
                {
                    var Writed = GetPLCVariable(FP, Type);
                    if (Writed.Item1 == true)
                    {
                        if (Type.EndsWith("[]"))
                        {
                            if (Enumerable.SequenceEqual((int[])Value, (int[])Writed.Item2))
                            {
                                IsComplete = true;
                            }
                            else
                            {
                                IsComplete = false;
                                result = "[" + FP.ToString() + "]Data not match PC:" + Value + " PLC:" + Writed.Item2;
                            }
                        }
                        else
                        {
                            if (Writed.Item2.Equals(Value))
                            {
                                IsComplete = true;
                            }
                            else
                            {
                                IsComplete = false;
                                result = "[" + FP.ToString() + "]Data not match PC:" + Value + " PLC:" + Writed.Item2;
                            }
                        }
                    }
                    else
                    {
                        IsComplete = false;
                        result = Writed.Item2;
                    }
                }
                else
                {
                    IsComplete = true;
                }
            }
            catch (Exception e)
            {
                IsComplete = false;
                Log.WriteLog("Error! - Cannot write PLC data [" + FP.ToString() + "]: " + e.Message, LogType.Fail);
            }

            return new Tuple<bool, object>(IsComplete, result);
        }

        private string GetDataTypefromValue(object Value)
        {
            string result = "";

            try
            {
                string Type = Value.GetType().Name;
                result = (Type == "int32" ? "int" : Type);
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - Can't get data Type from PLC: " + e.Message, LogType.Fail);
            }

            return result;
        }

        public bool Send_NewOrder(OrderModel odm, PatternModel ptn, Laying[] DBPlacement, int buf_RobotModel)
        {
            bool IsComplete = true;

            // Material Number
            var Writed = SetPLCVariable(FieldPLCs.PC_MaterialNumber, odm.MaterialNo, "string");
            if (Writed.Item1 == false) { return false; }

            //Product Code
            Writed = SetPLCVariable(FieldPLCs.PC_ProductCode, odm.ProductCode);
            if (Writed.Item1 == false) { return false; }

            //SplitSO
            Writed = SetPLCVariable(FieldPLCs.PC_SOBundle, odm.GetArrSplitSO());
            if (Writed.Item1 == false) { return false; }

            //Pallet
            Writed = SetPLCVariable(FieldPLCs.PC_PalletWidth, odm.PalletWidth);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_PalletLength, odm.PalletLength);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_PalletHeight, odm.PalletHeight);
            if (Writed.Item1 == false) { return false; }

            //Bundle
            Writed = SetPLCVariable(FieldPLCs.PC_BundleWidth, (odm.SwitchBDSize == true ? odm.BundleLength : odm.BundleWidth));
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_BundleLength, (odm.SwitchBDSize == true ? odm.BundleWidth : odm.BundleLength));
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_BundleHeight, odm.BundleHeight);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_BundleWeight, odm.BundleWeight);
            if (Writed.Item1 == false) { return false; }
            //[2021-23-12]Top add PC_SheetPerBundle parameter.
            Writed = SetPLCVariable(FieldPLCs.PC_SheetPerBundle, odm.SheetPerBundle);
            if (Writed.Item1 == false) { return false; }
            //[2021-01-06]HeightRatio added by Beer.
            Writed = SetPLCVariable(FieldPLCs.PC_HeightRatio, odm.HeightRatio);
            if (Writed.Item1 == false) { return false; }

            //TieSheet
            Writed = SetPLCVariable(FieldPLCs.PC_TieSheetWidth, odm.TieSheetWidth);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_TieSheetLength, odm.TieSheetLength);
            if (Writed.Item1 == false) { return false; }
            //TieSheet Layer.
            string TieSheetText = odm.GetArrTieSheet();
            int[] ArrTieSheet = TieSheetText.Replace("[", "").Replace("]", "").Split(',').Select(Int32.Parse).ToArray();
            Writed = SetPLCVariable(FieldPLCs.PC_TieSheetLayer, ArrTieSheet);
            if (Writed.Item1 == false) { return false; }

            //Bottom Sheet
            Writed = SetPLCVariable(FieldPLCs.PC_BottomSheet, odm.BottomSheet);
            if (Writed.Item1 == false) { return false; }
            //Top Sheet
            Writed = SetPLCVariable(FieldPLCs.PC_TopSheet, odm.TopSheet);
            if (Writed.Item1 == false) { return false; }

            //Grip 2 Bundles
            Writed = SetPLCVariable(FieldPLCs.PC_Grip2BD, odm.BDPerGrip == 2);
            if (Writed.Item1 == false) { return false; }

            //Extra Pick Depth
            Writed = SetPLCVariable(FieldPLCs.PC_ExtraPickDepth, odm.ExtraPickDepth);
            if (Writed.Item1 == false) { return false; }

            //Stacker Lift Bundle
            Writed = SetPLCVariable(FieldPLCs.PC_STKLiftBD, odm.STKLiftBD);
            if (Writed.Item1 == false) { return false; }
            //AnitBounce
            Writed = SetPLCVariable(FieldPLCs.PC_STKAntiBounce, odm.STKAntiBounce);
            if (Writed.Item1 == false) { return false; }

            // Gripper Finger
            Writed = SetPLCVariable(FieldPLCs.PC_GRPFinger, odm.FingerRequired);
            if (Writed.Item1 == false) { return false; }


            //Pattern
            Writed = SetPLCVariable(FieldPLCs.PC_BundlePerLayer, odm.BundlePerLayer);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_LayerPerPallet, odm.LayerPerPallet);
            if (Writed.Item1 == false) { return false; }

            if (buf_RobotModel == 1)
            {
            }
            else
            {

                int buf_SWPat1 = ptn.SwitchPattern[2] - '0';
                int buf_SWPat2 = ptn.SwitchPattern[4] - '0';
                int buf_SWPat3 = ptn.SwitchPattern[6] - '0';
                int buf_SWPat4 = ptn.SwitchPattern[8] - '0';
                int buf_PlacementArraySize = buf_SWPat1 + buf_SWPat2 + buf_SWPat3 + buf_SWPat4;
                Writed = SetPLCVariable(FieldPLCs.PC_PlacementArraySize, buf_PlacementArraySize);
                if (Writed.Item1 == false) { return false; }

                //[2021-01-06]LayerPattern added by Beer.
                Laying[] Lays;// = new Laying[1];
                if (DBPlacement != null)
                    Lays = DBPlacement;
                else
                    Lays = ptn.GetPlacementInfo();

                var Layer_X = ptn.GetPlacementInfo_Field(Lays, "X", ptn.SwitchPattern);
                Writed = SetPLCVariable(FieldPLCs.PC_Placement_X, Layer_X);
                if (Writed.Item1 == false) { return false; }

                var Layer_Y = ptn.GetPlacementInfo_Field(Lays, "Y", ptn.SwitchPattern);
                Writed = SetPLCVariable(FieldPLCs.PC_Placement_Y, Layer_Y);
                if (Writed.Item1 == false) { return false; }

                var Layer_Ori = ptn.GetPlacementInfo_Field(Lays, "Ori", ptn.SwitchPattern);
                Writed = SetPLCVariable(FieldPLCs.PC_Placement_Ori, Layer_Ori);
                if (Writed.Item1 == false) { return false; }
            }
            //Squaring
            Writed = SetPLCVariable(FieldPLCs.PC_SQActive, odm.Squaring);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_SQOpenX, ptn.SQ_XaxisValue);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_SQOpenY, ptn.SQ_YaxisValue);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_SQPressX, ptn.SQ_XaxisValueWithOutGap);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_SQPressY, ptn.SQ_YaxisValueWithOutGap);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_LayerLength, odm.Peri_CloseL);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_LayerWidth, odm.Peri_CloseW);
            if (Writed.Item1 == false) { return false; }

            //ExtraSQ
            Writed = SetPLCVariable(FieldPLCs.PC_SQExtra, odm.ExtraSQ);
            if (Writed.Item1 == false) { return false; }

            if (buf_RobotModel == 1)
            { }
            else
            {
                //Placing Mode
                Writed = SetPLCVariable(FieldPLCs.PC_PlaceSoft, odm.PlacingMode == 1);
                if (Writed.Item1 == false) { return false; }
                Writed = SetPLCVariable(FieldPLCs.PC_PlaceFast, odm.PlacingMode == 3);
                if (Writed.Item1 == false) { return false; }
            }
            
            //Discharge Mode
            Writed = SetPLCVariable(FieldPLCs.PC_DCSpdSlow, odm.DischargeMode == 1);
            if (Writed.Item1 == false) { return false; }
            Writed = SetPLCVariable(FieldPLCs.PC_DCSpdFast, odm.DischargeMode == 3);
            if (Writed.Item1 == false) { return false; }
            
            return IsComplete;
        }

        public bool Check_BlockActivity()
        {
            bool IsOnBlock = true;

            try
            {

                for (int i = 0; i < 3; i++)
                {
                    var OnBlock = GetPLCVariable(FieldPLCs.PC_BlockPCAction);
                    if (OnBlock.Item1.Equals(false))
                    {
                        Log.WriteLog("Error! - Cannot read PLC data [" + FieldPLCs.PC_BlockPCAction.ToString() + "].", LogType.Fail);
                        IsOnBlock = true;
                        //return IsOnBlock;
                    }
                    else
                    {
                        if (OnBlock.Item2.Equals(true))
                        {
                            Log.WriteLog("Error! - PLC is in Block State.", LogType.Notes);
                            IsOnBlock = true;
                            return IsOnBlock;
                        }
                        else
                        {
                            IsOnBlock = false;
                            //return IsOnBlock;
                        }
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                return IsOnBlock;
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - Cannot read PLC data [" + FieldPLCs.PC_BlockPCAction.ToString() + "]: " + e.Message, LogType.Fail);
            }

            return IsOnBlock;
        }

        private Tuple<bool, object> AutoConvertDataType(object obj, string Type, string Field)
        {
            bool IsComplete = true;
            object result = null;

            try
            {
                switch (Type.ToLower())
                {
                    case "string": result = obj.ToString(); break;
                    case "bool": result = Convert.ToBoolean(obj.ToString()); break;
                    case "int":
                    case "int32":
                        result = Convert.ToInt32(obj.ToString()); break;
                    case "double": result = Convert.ToDouble(obj.ToString()); break;
                    case "int32[]":
                        result = (int[])obj; break;

                }
            }
            catch (Exception e)
            {
                IsComplete = false;
                Log.WriteLog("Error! - while trying to convert PLC value ["+ Field + "]: " + e.Message, LogType.Fail);
            }

            return new Tuple<bool, object>(IsComplete, result);
        }

        #region Kawasaki_Robot
        public Tuple<bool, object> GetPLCVariable(KawaPLCField KP, string Type = "")
        {
            return GetPLCVariable(FieldPLCs.Empty, Type, KP);
        }

        public bool CheckRobotIsActive()
        {
            bool IsActive = false;

            try
            {
                var PLCData = GetPLCVariable(KawaPLCField.PC_RobInAuto);
                if (PLCData.Item1 == true)
                    IsActive = (bool)PLCData.Item2;
            }
            catch (Exception)
            {
                IsActive = false;
            }

            return IsActive;
        }

        public UIFeedDataModel GetFeedData()
        {
            UIFeedDataModel Rob_FeedData = new UIFeedDataModel();
            var FeedData = GetPLCVariable(KawaPLCField.Using_AmountOfPlacedBD, "int");
            if (FeedData.Item1 == true)
                Rob_FeedData.OrderBundle_Count = (int)FeedData.Item2;

            FeedData = GetPLCVariable(KawaPLCField.Using_PalletCount, "int");
            if (FeedData.Item1 == true)
                Rob_FeedData.Pallet_Count = (int)FeedData.Item2;

            FeedData = GetPLCVariable(KawaPLCField.Using_LastBDonPrvPallet, "int");
            if (FeedData.Item1 == true)
                Rob_FeedData.LastBD_PrvPallet = (int)FeedData.Item2;

            FeedData = GetPLCVariable(KawaPLCField.Using_CurrentStack, "int");
            if (FeedData.Item1 == true)
                Rob_FeedData.Step_Count = (int)FeedData.Item2;

            FeedData = GetPLCVariable(KawaPLCField.Using_CurrentLayer, "int");
            if (FeedData.Item1 == true)
                Rob_FeedData.Layer_Count = (int)FeedData.Item2;

            FeedData = GetPLCVariable(KawaPLCField.Using_LastBDonPrvSO, "int");
            if (FeedData.Item1 == true)
                Rob_FeedData.LastBD_PrvSO = (int)FeedData.Item2;

            FeedData = GetPLCVariable(KawaPLCField.Using_CurrentSO, "int");
            if (FeedData.Item1 == true)
                Rob_FeedData.SO_Count = (int)FeedData.Item2;
            
            return Rob_FeedData;
        }

        public LotEndFeedDataModel GetLotEndFeedData(double buf_SheetPerBD, int buf_RobotModel)
        {
            LotEndFeedDataModel buf_LEF = new LotEndFeedDataModel();

            //  1.  Sheet Height
            //  2.  Height Ratio
            //  3.  TieSheet Width x
            //  4.  TieSheet Length x
            //  5.  Bottom Sheet
            //  6.  Top Sheet
            //  7.  Extra Pick
            //  8.  Anti Bounce
            //  9.  Extra Squaring
            //  10. Placing Mode
            //  11. Discharge Mode
            //  12. Amount of Placed Bundle
            
            //  1.  Sheet Height
            var FeedData = GetPLCVariable(FieldPLCs.Using_BundleHeight, "int");
            if (FeedData.Item1 == true)
            {
                buf_LEF.SheetHeight = (int)FeedData.Item2;
                buf_LEF.SheetHeight = Math.Round((double)buf_LEF.SheetHeight / buf_SheetPerBD, 2);
            }

            //  2.  Height Ratio
            FeedData = GetPLCVariable(FieldPLCs.Using_HeightRatio, "int");
            if (FeedData.Item1 == true)
                buf_LEF.HeightRatio = (int)FeedData.Item2;

            //  5.  Bottom Sheet
            FeedData = GetPLCVariable(FieldPLCs.Using_BottomSheet, "int");
            if (FeedData.Item1 == true)
            {
                buf_LEF.BottomSheet = (int)FeedData.Item2;
            }

            //  6.  Top Sheet
            FeedData = GetPLCVariable(FieldPLCs.Using_TopSheet);
            if (FeedData.Item1 == true)
                buf_LEF.TopSheet = (bool)FeedData.Item2;

            //  7.  Extra Pick
            FeedData = GetPLCVariable(FieldPLCs.Using_ExtraPickDepth);
            if (FeedData.Item1 == true)
                buf_LEF.ExtraPickDepth = (bool)FeedData.Item2;

            //  8.  Anti Bounce
            FeedData = GetPLCVariable(FieldPLCs.Using_STKAntiBounce);
            if (FeedData.Item1 == true)
                buf_LEF.STKAntiBounce = (bool)FeedData.Item2;

            //  9.  Extra Squaring
            FeedData = GetPLCVariable(FieldPLCs.Using_SQExtra);
            if (FeedData.Item1 == true)
                buf_LEF.SQExtra = (bool)FeedData.Item2;

            //  10.  Placing Mode
            FeedData = GetPLCVariable(FieldPLCs.Using_PlaceSoft);
            if (FeedData.Item1 == true)
                buf_LEF.PlacingMode = (bool)FeedData.Item2? 1 : 2;
            FeedData = GetPLCVariable(FieldPLCs.Using_PlaceFast);
            if (FeedData.Item1 == true)
                buf_LEF.PlacingMode = (bool)FeedData.Item2? 3 : buf_LEF.PlacingMode;

            //  11. Discharge Mode
            FeedData = GetPLCVariable(FieldPLCs.Using_DCSpdSlow);
            if (FeedData.Item1 == true)
                buf_LEF.DischargeMode = (bool)FeedData.Item2 ? 1 : 2;
            FeedData = GetPLCVariable(FieldPLCs.Using_DCSpdFast);
            if (FeedData.Item1 == true)
                buf_LEF.DischargeMode = (bool)FeedData.Item2 ? 3 : buf_LEF.DischargeMode;

            //  12. Amount of Placed Bundle
            if (buf_RobotModel == 1)
            {
            }
            else
            {
                FeedData = GetPLCVariable(KawaPLCField.Using_AmountOfPlacedBD, "int");
                if (FeedData.Item1 == true)
                    buf_LEF.AmountOfPlacedBundles = (int)FeedData.Item2;
            }
            return buf_LEF;
        }

        public DateTime ConvertRobotCTime(string _Time)
        {
            var Today = DateTime.Now;
            DateTime StartTime = new DateTime(Today.Year, Today.Month, Today.Day);
            try
            {
                if (TimeSpan.TryParse(_Time, out TimeSpan _PTime))
                {
                    StartTime = new DateTime(Today.Year, Today.Month, Today.Day);
                    StartTime = StartTime.Add(_PTime);
                }
                else
                {
                    Log.WriteLog("Error! - Cannot convert robot's Pallet Time [Input:" + _Time + "].", LogType.Fail);
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - While trying to convert robot's Pallet Time [Input:" + _Time + "]: " + e.Message, LogType.Fail);
            }

            return StartTime;
        }
        #endregion

    }

    //*add PC_BundleWeight
    public enum FieldPLCs
    {
        PC_BlockPCAction,
        PC_AddingData,
        PC_Door_Status,
        PC_Door_LockCmd,
        PC_Door_OpenCmd,
        PC_System_Status,
        PC_Error_Status,
        PC_Start_Cmd,
        PC_Start_Done,
        PC_HalfFin_Cmd,
        PC_LotEnd_Cmd,
        PC_LotEnd_Started,
        PC_LotEnd_Ack,
        PC_ResetData,
        PC_CommuError,
        PC_MaterialNumber,
        PC_ProductCode,
        PC_SOBundle,
        PC_PalletWidth,
        PC_PalletLength,
        PC_PalletHeight,
        PC_BundleWidth,
        PC_BundleLength,
        PC_BundleHeight,
        PC_BundleWeight,
        PC_SheetPerBundle,
        PC_HeightRatio,
        PC_TieSheetWidth,
        PC_TieSheetLength,
        PC_TieSheetLayer,
        PC_BottomSheet,
        PC_TopSheet,
        PC_Grip2BD,
        PC_ExtraPickDepth,
        PC_STKLiftBD,
        PC_STKAntiBounce,
        PC_GRPFinger,
        PC_BundlePerLayer,
        PC_LayerPerPallet,
        PC_PlacementArraySize,
        PC_Placement_X,
        PC_Placement_Y,
        PC_Placement_Ori,
        PC_SQActive,
        PC_SQOpenX,
        PC_SQOpenY,
        PC_SQPressX,
        PC_SQPressY,
        PC_SQExtra,
        PC_PlaceSoft,
        PC_PlaceFast,
        PC_DCSpdSlow,
        PC_DCSpdFast,
        PC_DataGuarantee,
        PC_LayerLength,
        PC_LayerWidth,

        Empty,

        Using_BundleHeight,
        Using_HeightRatio,
        Using_TieSheetWidth,
        Using_TieSheetLength,
        Using_BottomSheet,
        Using_TopSheet,
        Using_ExtraPickDepth,
        Using_STKAntiBounce,
        Using_SQExtra,
        Using_PlaceSoft,
        Using_PlaceFast,
        Using_DCSpdSlow,
        Using_DCSpdFast,
        Using_RoboSpeed
    }

    public enum KawaPLCField
    {
        PC_RobInAuto,
        Using_AmountOfPlacedBD,
        Using_PalletCount,
        Using_LastBDonPrvPallet,
        Using_CurrentStack,
        Using_CurrentLayer,
        Using_LastBDonPrvSO,
        Using_CurrentSO
    }
}
