using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Configuration;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using ATD_ID4P.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATD_ID4P.Class 
{ 
    public class ABBCommu
    {
        public Controller ABBCtrl = null;
        public bool RobotActive = false;
        private LogCls _Log = new LogCls();

        public void FindRobotByIP(string IP)
        {
            try
            {
                NetworkScanner ABBScan = new NetworkScanner();
                NetworkScanner.AddRemoteController(IP);
                ABBScan.Scan();

                if (ABBScan.Controllers.Count == 0)
                {
                    _Log.WriteLog("Error! - No Robot Controller found in the system.", LogType.Warning);
                    return;
                }
                else
                {
                    foreach (ControllerInfo _Ctrl in ABBScan.Controllers)
                    {
                        if (_Ctrl.IPAddress.ToString() == IP)
                        {

                            ABBCtrl = Controller.Connect(_Ctrl,ConnectionType.Standalone);
                            ABBCtrl.Logon(UserInfo.DefaultUser);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e != null)
                {
                    // Handle the exception
                    MessageBox.Show("Error! - While scanning for Robot Controller: " + e.Message);
                }
                else
                {
                    // Handle the case where the exception object is null
                    Console.WriteLine("Error! - While scanning for Robot Controller, but no exception object was created.");
                }
                _Log.WriteLog("Error! - While scanning for Robot Controller: " + e.Message, LogType.Fail);
                return;
            }
        }

        public bool CheckRobotIsActive(string IP = "")
        {
            if (!string.IsNullOrEmpty(IP))
            {
                //*** ByPass For Dry Test:
                FindRobotByIP(IP);
            }

            if (ABBCtrl != null)
            {
                try
                {
                    string Field = ABBFeedFields.Flag_RobInAuto.ToString();
                    var _Read = ReadABBValue(Field);
                    if (_Read.Item1 == true)
                    {
                        RobotActive = Convert.ToBoolean(_Read.Item2);
                        _Log.WriteLog((RobotActive == true ? "Robot is in Automode" : "Robot is in Manualmode"), LogType.Notes);
                    }
                    else
                    {
                        RobotActive = false;
                        _Log.WriteLog("Error! - Can not read robot data [Flag_RobInAuto]", LogType.Fail);
                    }
                }
                catch (Exception e)
                {
                    _Log.WriteLog("Error! - While checking for robot controller status: "+e.Message, LogType.Fail);
                }
            }

            return RobotActive;
        }

        public Tuple<bool, object> ReadABBValue(string Param, string Task = "T_ROB1", string Module = "DataModule")
        //public Tuple<bool,object> ReadABBValue(string Param, string Task = "T_ROB1", string Module = "MainModule")
        {
            bool IsComplete;
            object result = null;

            try
            {
                if (ABBCtrl != null)
                {
                    var Item = ABBCtrl.Rapid.GetTask(Task).GetModule(Module).GetRapidData(Param);
                    result = Item.Value;
                    IsComplete = true;
                }
                else
                {
                    _Log.WriteLog("Error! - Cannot read robot data [" + Param + "]. No active robot controller.", LogType.Fail);
                    result = "";
                    IsComplete = false;
                }
            }
            catch (Exception e)
            {
                IsComplete = false;
                _Log.WriteLog("Error! - Cannot read robot data [" + Param + "]: " + e.Message, LogType.Fail);
            }

            return new Tuple<bool, object>(IsComplete, result);
        }

        public Tuple<bool, object> ReadABBValuewithModule(Module MD, string Param)
        {
            bool IsComplete;
            object result = null;

            try
            {
                if (ABBCtrl != null)
                {
                    var Item = MD.GetRapidData(Param);
                    result = Item.Value;
                    IsComplete = true;
                }
                else
                {
                    _Log.WriteLog("Error! - Cannot read robot data [" + Param + "]. No active robot controller.", LogType.Fail);
                    result = "";
                    IsComplete = false;
                }
            }
            catch (Exception e)
            {
                IsComplete = false;
                _Log.WriteLog("Error! - Cannot read robot data [" + Param + "]: " + e.Message, LogType.Fail);
            }

            return new Tuple<bool, object>(IsComplete, result);
        }

        //public Tuple<bool, object> WriteABBValue(string Param, object Value, string Task = "T_ROB1", string Module = "MainModule", bool DoCompare = true)
        public Tuple<bool, object> WriteABBValue(string Param, object Value, string Task = "T_ROB1", string Module = "DataModule", bool DoCompare = true)
        {
            bool IsComplete = false;
            string respMsg = "";

            try
            {
                if (ABBCtrl != null)
                {
                    RapidData Item = ABBCtrl.Rapid.GetTask(Task).GetModule(Module).GetRapidData(Param);
                    //using (Mastership ms = Mastership.Request(ABBCtrl.Rapid))
                    using (Mastership ms = Mastership.Request(ABBCtrl))
                    {
                        if (Param.EndsWith("_BundleWeight"))
                        {
                            Item.Value = new Num(float.Parse(Value.ToString()));
                        }
                        else if (Param.EndsWith("_TieSheetLayer") || Param.EndsWith("_SOBundle"))
                        {
                            Item.Value.FillFromString(Value.ToString());
                        }
                        else
                        {
                            switch (Value.GetType().Name.ToLower())
                            {
                                case "bool": { Item.Value = new Bool(Convert.ToBoolean(Value)); break; }
                                case "boolean": { Item.Value = new Bool(Convert.ToBoolean(Value)); break; }
                                case "string": { Item.Value = new ABB.Robotics.Controllers.RapidDomain.String(Value.ToString()); break; }
                                case "double": { Item.Value = new Num(Convert.ToInt32(Value)); break; }
                                case "int":
                                case "int32":
                                    { Item.Value = new Num(Convert.ToInt32(Value)); break; }
                            }
                        }
                        ms.Release();

                        if (DoCompare == true)
                        {
                            var Added = ReadABBValue(Param);
                            if (Added.Item1 == false)
                            {
                                respMsg = Added.Item2.ToString();
                                return new Tuple<bool, object>(false, respMsg);
                            }
                            else
                            {
                                var Readed = ConvertABBtoCValue(Added.Item2, Value.GetType().Name);
                                if (!Value.Equals(Readed))
                                {
                                    respMsg = "Data not match [" + Param + "]";
                                    return new Tuple<bool, object>(false, respMsg);
                                }
                            }
                        }

                        IsComplete = true;
                    }
                }
                else
                {
                    _Log.WriteLog("Error! - Cannot write robot data [" + Param + "]. No active robot controller.", LogType.Fail);
                }
            }
            catch (Exception e)
            {
                IsComplete = false;
                respMsg = e.Message;
                _Log.WriteLog("Error! - Cannot write robot data [" + Param + "]: " + e.Message, LogType.Fail);
            }

            return new Tuple<bool, object>(IsComplete, respMsg);
        }

        public Tuple<bool, object> WriteABBObject(string RapidName, object[] Vals, string Task = "T_ROB1", bool DoCompare = true, string _RapidKey = "Dimension")
        {
            bool IsComplete = false;
            string respMsg = "";

            try
            {
                if (ABBCtrl != null)
                {
                    UserDefined udf;
                    //string RapidKey = RapidName.Replace("PC_", "").Replace("Using_", "");
                    string RapidKey = _RapidKey;
                    RapidData[] Items = GetRapidDatas(RapidKey, Task);
                    //using (Mastership ms = Mastership.Request(ABBCtrl.Rapid))
                    using (Mastership ms = Mastership.Request(ABBCtrl))
                    {
                        try
                        {
                            foreach (RapidData _Item in Items)
                            {
                                if (_Item.Name == RapidName)
                                {
                                    udf = (UserDefined)_Item.Value;
                                    for (int i = 0; i < Vals.Count(); i++)
                                    {
                                        string Type = Vals[i].GetType().Name.ToLower();
                                        switch (Type)
                                        {
                                            case "int":
                                            case "int32":
                                                udf.Components[i] = new Num(Convert.ToInt32(Vals[i])); break;
                                            case "bool":
                                                udf.Components[i] = new Bool(Convert.ToBoolean(Vals[i])); break;
                                            case "double":
                                                udf.Components[i] = new Num(Convert.ToDouble(Vals[i])); break;
                                            case "string":
                                                udf.Components[i] = new ABB.Robotics.Controllers.RapidDomain.String(Vals[i].ToString()); break;
                                        }
                                    }
                                    _Item.Value = udf;
                                    ms.Release();
                                    IsComplete = true;
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            IsComplete = false;
                            _Log.WriteLog("Error! - Cannot write robot data [" + RapidName + "]: " + ex.Message, LogType.Fail);
                        }
                    }

                    if (DoCompare == true)
                    {
                        var Added = ReadABBObject(RapidName, Task, _RapidKey);
                        if (Added.Item1 == false)
                        {
                            respMsg = Added.Item2.ToString();
                            return new Tuple<bool, object>(false, respMsg);
                        }
                        else
                        {
                            string strUdf = udf.ToString();
                            string strAdded = Added.Item2.ToString();
                            //if (!udf.Equals(Added.Item2))
                            if (!strUdf.Equals(strAdded))
                            {
                                respMsg = "Data not match[" + RapidName + "]";
                                return new Tuple<bool, object>(false, respMsg);
                            }
                        }
                    }
                }
                else
                {
                    _Log.WriteLog("Error! - Cannot write robot data [" + RapidName + "], No active robot controller", LogType.Fail);
                }
            }
            catch (Exception e)
            {
                IsComplete = false;
                respMsg = e.Message;
                _Log.WriteLog("Error! - Cannot write robot data[" + RapidName + "]: " + e.Message, LogType.Fail);
            }

            return new Tuple<bool, object>(IsComplete, respMsg);
        }

        //public Tuple<bool, object> ReadABBObject(string RapidName, string Task = "T_ROB1")
        public Tuple<bool, object> ReadABBObject(string RapidName, string Task = "T_ROB1", string _RapidKey = "Dimension")
        {
            bool IsComplete = false;
            string respMsg = "";
            UserDefined result;

            try
            {
                if (ABBCtrl != null)
                {
                    //string RapidKey = RapidName.Replace("PC_", "").Replace("Using_", "");
                    string RapidKey = _RapidKey;
                    RapidData[] Items = GetRapidDatas(RapidKey, Task);
                    //using (Mastership ms = Mastership.Request(ABBCtrl.Rapid))
                    using (Mastership ms = Mastership.Request(ABBCtrl))
                    {
                        try
                        {
                            foreach (RapidData _Item in Items)
                            {
                                if (_Item.Name == RapidName)
                                {
                                    result = (UserDefined)_Item.Value;
                                    ms.Release();
                                    IsComplete = true;
                                    return new Tuple<bool, object>(IsComplete, result);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            IsComplete = false;
                            _Log.WriteLog("Error! - Cannot read robot data [" + RapidName + "]: " + ex.Message, LogType.Fail);
                        }
                    }
                }
                else
                {
                    _Log.WriteLog("Error! - Cannot read robot data [" + RapidName + "]. No Active robot controller.", LogType.Fail);
                }
            }
            catch (Exception e)
            {
                IsComplete = false;
                respMsg = e.Message;
                _Log.WriteLog("Error! - Cannot read robot data [" + RapidName + "]: " + e.Message, LogType.Fail);
            }

            return new Tuple<bool, object>(IsComplete, respMsg);
        }

        public Tuple<bool, object> WriteABBPlacement(Laying[] lays, string RapidKey,string loc_SwitchPattern, bool IsReset = false, bool DoCompare = true)
        {
            bool IsComplete = false;
            string respMsg = "";
            int buf_SWPat1 = loc_SwitchPattern[2] - '0';
            int buf_SWPat2 = loc_SwitchPattern[4] - '0';
            int buf_SWPat3 = loc_SwitchPattern[6] - '0';
            int buf_SWPat4 = loc_SwitchPattern[8] - '0';
            int MaxPatLayer = buf_SWPat1 + buf_SWPat2 + buf_SWPat3 + buf_SWPat4;

            try
            {
                RapidData[] RPIs = GetRapidDatas("Placementinfo");
                RapidData TargetRPI = null;
                if (RPIs != null && RPIs.Count() > 0)
                {
                    //Create Data for Reset
                    if (IsReset == true)
                    {
                        //int MaxPatLayer = Properties.Settings.Default.Max_PatLayer;
                        int MaxBundlePerLayer = Properties.Settings.Default.Max_BDperLayer;
                        int LayerIndex = 1;
                        lays = new Laying[MaxPatLayer * MaxBundlePerLayer];
                        for (int i = 0; i < lays.Count(); i++)
                        {
                            if ((i > 0) && ((i % MaxBundlePerLayer) == 0))
                                LayerIndex++;

                            lays[i].Layer = LayerIndex;
                        }
                    }

                    //Write Data
                    // Look in to all object in RPI (all variable of Placementinfo Type)
                    // if the object name equal RapidKey then proceed
                    foreach (RapidData RPI in RPIs)
                    {
                        if (RPI.Name == RapidKey)
                        {
                            //using (Mastership ms = Mastership.Request(ABBCtrl.Rapid))
                            using (Mastership ms = Mastership.Request(ABBCtrl))
                            {
                                int BIndex = 0;
                                int LastLayer = 0;
                                int PatLayer = -1;
                                for (int i = 0; i < lays.Count(); i++)
                                {
                                    Laying lay = lays[i];

                                    if (LastLayer != lay.Layer)
                                        BIndex = 0;                          
                                    if (lay.Layer == buf_SWPat1 || lay.Layer == 2*buf_SWPat2 ||
                                        lay.Layer == 3*buf_SWPat3 || lay.Layer == 4*buf_SWPat4)
                                    {
                                        if (BIndex == 0)
                                            PatLayer++;
                                        ////Disable 2020-10-22
                                        //if (BIndex > 9)
                                        //    continue;

                                        UserDefined URP = (UserDefined)RPI.ReadItem(lay.Layer - 1, BIndex);
                                        URP.Components[0] = new Num(Convert.ToInt32(lay.X));
                                        URP.Components[1] = new Num(Convert.ToInt32(lay.Y));
                                        URP.Components[2] = new Num(Convert.ToInt32(lay.Ori));
                                        //URP.Components[3] = new Num(Convert.ToInt32(lay.Row));
                                        //RPI.WriteItem(URP, lay.Layer - 1, BIndex);
                                        RPI.WriteItem(URP, PatLayer, BIndex);
                                    }
                                    BIndex++;
                                    LastLayer = lay.Layer;
                                }
                                ms.Release();
                            }
                            TargetRPI = RPI;

                            IsComplete = true;
                        }
                    }

                    //Compare Data
                    if (DoCompare == true && TargetRPI != null)
                    {
                        //using (Mastership ms = Mastership.Request(ABBCtrl.Rapid))
                        using (Mastership ms = Mastership.Request(ABBCtrl))
                        {
                            int BIndex = 0;
                            int LastLayer = 0;
                            int EmptyPatLayer = MaxPatLayer;
                            int PatLayer = -1;
                            string strArr = "";
                            string strURP = "";
                            for (int i = 0; i < lays.Count(); i++)
                            {
                                Laying lay = lays[i];
                                if (LastLayer != lay.Layer)
                                    BIndex = 0;

                                if (BIndex > 9)
                                    //break;
                                    continue;
                                UserDefined URP;
                                //strArr = string.Format("[{0},{1},{2},{3}]", lay.X, lay.Y, lay.Degree, lay.Row);
                                if (lay.Layer == buf_SWPat1 || lay.Layer == 2 * buf_SWPat2 ||
                                    lay.Layer == 3 * buf_SWPat3 || lay.Layer == 4 * buf_SWPat4)
                                {
                                    strArr = string.Format("[{0},{1},{2}]", lay.X, lay.Y, lay.Ori);
                                    if (BIndex == 0)
                                        PatLayer++;
                                    URP = (UserDefined)TargetRPI.ReadItem(PatLayer, BIndex);
                                }
                                else
                                {
                                    strArr = string.Format("[{0},{1},{2}]", 0, 0, 0);
                                    if (BIndex == 0)
                                        EmptyPatLayer++;
                                    URP = (UserDefined)TargetRPI.ReadItem(EmptyPatLayer - 1, BIndex);
                                }
                                strURP = URP.ToString();

                                if (!strArr.Equals(strURP))
                                {
                                    respMsg = "Data not match[" + RapidKey + "]";
                                    return new Tuple<bool, object>(false, respMsg);
                                }

                                BIndex++;
                                LastLayer = lay.Layer;
                            }
                            ms.Release();
                        }

                        IsComplete = true;
                        respMsg = "";
                    }
                }
                else
                {
                    IsComplete = false;
                    respMsg = "Not found [" + RapidKey + "]";
                }

                return new Tuple<bool, object>(IsComplete, respMsg);
            }
            catch (Exception e)
            {
                return new Tuple<bool, object>(false, e.Message);
            }
        }

        public RapidData[] GetRapidDatas(string RapidName, string Task = "T_ROB1")
        //public RapidData[] GetRapidDatas(string RapidName, string Task = "T_ROB1", string Module = "DataModule")
        {
            RapidSymbolSearchProperties searchProps;
            RapidSymbol[] rapidSymbols;
            RapidData[] rapidDatas = null;
            ABB.Robotics.Controllers.RapidDomain.Task _Task = ABBCtrl.Rapid.GetTask(Task);

            // check arguments
            if (ABBCtrl != null && _Task != null && !string.IsNullOrEmpty(RapidName))
            {
                // define search properties
                searchProps = RapidSymbolSearchProperties.CreateDefaultForData();
                searchProps.Recursive = true;
                searchProps.Types = SymbolTypes.Data;

                // search for results
                rapidSymbols = _Task.SearchRapidSymbol(searchProps);
                // convert results to rapid-data
                rapidDatas = rapidSymbols.Select(rs => new RapidData(ABBCtrl, rs)).Where(rs => rs.RapidType == RapidName).ToArray();
            }

            return rapidDatas;
        }

        public object ConvertABBtoCValue(object Value, string CType)
        {
            object result = null;

            try
            {
                switch (CType.ToLower())
                {
                    case "bool": result = Convert.ToBoolean(Value); break;
                    case "boolean": result = Convert.ToBoolean(Value); break;
                    case "double": result = Convert.ToDouble(Value); break;
                    case "int":
                    case "int32":
                        result = Convert.ToInt32(Value); break;
                    case "string": result = Value.ToString().Replace("\"", ""); break;
                }
            }
            catch (Exception e)
            {
                _Log.WriteLog("Error! - while trying to convert ABB value: "+e.Message, LogType.Fail);
            }

            return result;
        }

        //public bool Send_ResetData()
        //{
        //    bool IsComplete;

        //    //Clear Static Field
        //    foreach (string str in Enum.GetNames(typeof(ABBStaticFields)))
        //    {
        //        if(str.EndsWith("_ProductCode"))
        //        {
        //            IsComplete = WriteABBValue(str, "").Item1;
        //        }                    
        //        else
        //        {
        //            IsComplete = WriteABBValue(str, 0).Item1;
        //        }

        //        if (IsComplete == false)
        //            return IsComplete;
        //    }

        //    //Clear TieSheet
        //    OrderModel odm = new OrderModel()
        //    {
        //        TieSheetLayer = 0
        //    };
        //    string ArrTieSheet = odm.GetArrTieSheet();
        //    IsComplete = WriteABBValue("PC_TieSheetLayer", ArrTieSheet).Item1;
        //    if(IsComplete == false) { return IsComplete; }
        //    IsComplete = WriteABBValue("Using_TieSheetLayer", ArrTieSheet).Item1;
        //    if (IsComplete == false) { return IsComplete; }

        //    object[] empty3 = new object[] { 0, 0, 0 };
        //    //Clear PalletSize
        //    IsComplete = WriteABBObject("PC_PalletSize", empty3).Item1;
        //    if (IsComplete == false) { return IsComplete; }
        //    IsComplete = WriteABBObject("Using_PalletSize", empty3).Item1;
        //    if (IsComplete == false) { return IsComplete; }

        //    //Clear BundleSize
        //    IsComplete = WriteABBObject("PC_BundleSize", empty3).Item1;
        //    if (IsComplete == false) { return IsComplete; }
        //    IsComplete = WriteABBObject("Using_BundleSize", empty3).Item1;
        //    if (IsComplete == false) { return IsComplete; }

        //    //Clear TieSheetSize
        //    IsComplete = WriteABBObject("PC_TieSheetSize", empty3).Item1;
        //    if (IsComplete == false) { return IsComplete; }
        //    IsComplete = WriteABBObject("Using_TieSheetSize", empty3).Item1;
        //    if (IsComplete == false) { return IsComplete; }

        //    //Clear PlacementInfo
        //    Laying[] emptyLay = new Laying[1];
        //    //PC Placement
        //    IsComplete = WriteABBPlacement(emptyLay, false, true).Item1;
        //    if (IsComplete == false) { return IsComplete; }
        //    //Using Placement
        //    IsComplete = WriteABBPlacement(emptyLay, true, true).Item1;
        //    if (IsComplete == false) { return IsComplete; }

        //    if(IsComplete == true)
        //        _Log.WriteLog("Robot send data complete", LogType.Success);

        //    return IsComplete;
        //}

        public bool Send_NewOrder(OrderModel odm, PatternModel ptm, Laying[] DBPlacement)
        {
            bool IsComplete;

            object[] obj3;
            object[] obj2;

            //SplitSO
            string ArrSplitSO = odm.GetTextSplitSO();
            IsComplete = WriteABBValue(ABBStaticFields.PC_SOBundle.ToString(), string.Format(@"[{0}]", ArrSplitSO)).Item1;
            if (IsComplete == false) { return IsComplete; }

            //Auto Lot End
            IsComplete = WriteABBValue(ABBStaticFields.PC_AutoLotEnd.ToString(), odm.AutoLotEnd).Item1;
            if (IsComplete == false) { return IsComplete; }

            //PalletSize
            obj3 = new object[] { odm.PalletWidth, odm.PalletLength, odm.PalletHeight };
            IsComplete = WriteABBObject(ABBStaticFields.PC_PalletSize.ToString(), obj3).Item1;
            if (IsComplete == false) { return IsComplete; }

            //BundleSize
            obj3 = new object[] { odm.BundleWidth, odm.BundleLength, odm.BundleHeight };
            if (odm.SwitchBDSize == true)
                obj3 = new object[] { odm.BundleLength, odm.BundleWidth, odm.BundleHeight };
            IsComplete = WriteABBObject(ABBStaticFields.PC_BundleSize.ToString(), obj3).Item1;
            if (IsComplete == false) { return IsComplete; }
            //Bundle Weight
            IsComplete = WriteABBValue(ABBStaticFields.PC_BundleWeight.ToString(), odm.BundleWeight).Item1;
            if (IsComplete == false) { return IsComplete; }
            
            //TieSheet Layer
            string ArrTieSheet = odm.GetArrTieSheet();
            IsComplete = WriteABBValue(ABBStaticFields.PC_TieSheetLayer.ToString(), ArrTieSheet).Item1;
            if (IsComplete == false) { return IsComplete; }

            //Grip 2 Bundles
            IsComplete = WriteABBValue(ABBStaticFields.PC_Grip2BD.ToString(), odm.BDPerGrip == 2).Item1;
            if (IsComplete == false) { return IsComplete; }

            //Stacker Lift Bundle
            IsComplete = WriteABBValue(ABBStaticFields.PC_STKLiftBD.ToString(), odm.STKLiftBD).Item1;
            if (IsComplete == false) { return IsComplete; }

            //Finger Require
            IsComplete = WriteABBValue(ABBStaticFields.PC_FingerRequired.ToString(), odm.FingerRequired).Item1;
            if (IsComplete == false) { return IsComplete; }

            //Pattern
            IsComplete = WriteABBValue(ABBStaticFields.PC_AmountOfStack.ToString(), odm.BundlePerLayer).Item1;
            if (IsComplete == false) { return IsComplete; }
            IsComplete = WriteABBValue(ABBStaticFields.PC_AmountOfLayer.ToString(), odm.LayerPerPallet).Item1;
            if (IsComplete == false) { return IsComplete; }

            int buf_SWPat1 = ptm.SwitchPattern[2] - '0';
            int buf_SWPat2 = ptm.SwitchPattern[4] - '0';
            int buf_SWPat3 = ptm.SwitchPattern[6] - '0';
            int buf_SWPat4 = ptm.SwitchPattern[8] - '0';
            int buf_PlacementArraySize = buf_SWPat1 + buf_SWPat2 + buf_SWPat3 + buf_SWPat4;
            IsComplete = WriteABBValue(ABBStaticFields.PC_PlacementArraySize.ToString(), buf_PlacementArraySize).Item1;
            if (IsComplete == false) { return IsComplete; }

            //PlacementInfo
            //  Clear PlacementInfo
            Laying[] Lays = new Laying[1];
            IsComplete = WriteABBPlacement(Lays, ABBStaticFields.PC_Placement.ToString(), "0:1:1:1:1", true).Item1;
            if (IsComplete == false) { return IsComplete; }
            //  Add PlacementInfo
            if (DBPlacement != null)
                Lays = DBPlacement;
            else
                Lays = ptm.GetPlacementInfo();
            IsComplete = WriteABBPlacement(Lays, ABBStaticFields.PC_Placement.ToString(), ptm.SwitchPattern).Item1;
            if (IsComplete == false) { return IsComplete; }

            //Squaring
            IsComplete = WriteABBValue(ABBStaticFields.PC_SQActive.ToString(), odm.Squaring).Item1;
            if (IsComplete == false) { return IsComplete; }
            IsComplete = WriteABBValue(ABBStaticFields.PC_SQ_Press_Yaxis.ToString(), ptm.SQ_YaxisValueWithOutGap).Item1;
            if (IsComplete == false) { return IsComplete; }
            IsComplete = WriteABBValue(ABBStaticFields.PC_SQ_Press_Xaxis.ToString(), ptm.SQ_XaxisValueWithOutGap).Item1;
            if (IsComplete == false) { return IsComplete; }
            IsComplete = WriteABBValue(ABBStaticFields.PC_SQ_Open_Yaxis.ToString(), ptm.SQ_YaxisValue).Item1;
            if (IsComplete == false) { return IsComplete; }
            IsComplete = WriteABBValue(ABBStaticFields.PC_SQ_Open_Xaxis.ToString(), ptm.SQ_XaxisValue).Item1;
            if (IsComplete == false) { return IsComplete; }

            if (IsComplete == true)
                _Log.WriteLog("Complete Sequence! -  Successfully send all the data to the Robot Controller.", LogType.Success);

            return IsComplete;
        }

        public UIFeedDataModel GetFeedData()
        {
            UIFeedDataModel Rob_FeedData = new UIFeedDataModel();
            if (ABBCtrl != null)
            {
                var MD = ABBCtrl.Rapid.GetTask("T_ROB1").GetModule("DataModule");
                if (MD != null)
                {
                    var RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_AmountOfPlacedBundles.ToString());
                    if (RobotData.Item1 == true)
                        Rob_FeedData.OrderBundle_Count = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_AmountOfPallet.ToString());
                    if (RobotData.Item1 == true)
                        Rob_FeedData.Pallet_Count = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_CurrentSO.ToString());
                    if (RobotData.Item1 == true)
                        Rob_FeedData.SO_Count = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_CurrentStack.ToString());
                    if (RobotData.Item1 == true)
                        Rob_FeedData.Step_Count = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_CurrentLayer.ToString());
                    if (RobotData.Item1 == true)
                        Rob_FeedData.Layer_Count = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_LastBundleOnPallet.ToString());
                    if (RobotData.Item1 == true)
                        Rob_FeedData.LastBD_PrvPallet = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_LastBundleOnSO.ToString());
                    if (RobotData.Item1 == true)
                        Rob_FeedData.LastBD_PrvSO = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    // I think ID4P can detect itself from current layer
                    //RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_CurrentPattern.ToString());
                    //if (RobotData.Item1 == true)
                    //    Rob_FeedData.Pattern_Count = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    // May not be used because we will let PLC calcualte the Speed 
                    //RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_PalletTime.ToString());
                    //if (RobotData.Item1 == true)
                    //    Rob_FeedData.PalletTime = RobotData.Item2.ToString();

                    // May not be used because we will let PLC calcualte the Speed 
                    //RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PC_PalletTimeFinish.ToString());
                    //if (RobotData.Item1 == true)
                    //    Rob_FeedData.PalletTimeFinish = RobotData.Item2.ToString();


                    // We gonna use AmountOfPlaceBundle and PalletCount Comparison to get this
                    //RobotData = ReadABBValuewithModule(MD, ABBFeedFields.PlacedBundlesOnPallet.ToString());
                    //if (RobotData.Item1 == true)
                    //    Rob_FeedData.PlacedBundlesOnPallet = (int)ConvertABBtoCValue(RobotData.Item2, "int");

                    if (RobotData.Item1 == false)
                        RobotActive = false;
                }
                else
                {
                    RobotActive = false;
                }
            }

            return Rob_FeedData;
        }

    }

    public enum ABBStatusFields
    {
        //Flag_RobInAuto,
        //PC_DataGuarantee
    }

    public enum ABBStaticFields
    {
        PC_BundleWeight,
        PC_BoxPerBundle,
        PC_Grip2BD,
        PC_AmountOfLayer,
        PC_AmountOfStack,
        PC_PlacementArraySize,
        PC_FingerRequired,
        PC_SQ_Press_Yaxis,
        PC_SQ_Press_Xaxis,
        PC_SQActive,
        PC_STKLiftBD,
        PC_SQ_Open_Yaxis,
        PC_SQ_Open_Xaxis,
        PC_TieSheetLayer,
        PC_PalletSize,
        PC_BundleSize,
        PC_SOBundle,
        PC_AutoLotEnd,
        PC_Placement,
        PC_DataGuarantee

    }

    public enum ABBFeedFields
    {
        Flag_RobInAuto,
        PC_AmountOfPlacedBundles,
        PC_AmountOfPallet,
        PC_CurrentSO,
        PC_CurrentStack,
        PC_CurrentLayer,
        PC_CurrentPattern,
        PC_LastBundleOnPallet,
        PC_LastBundleOnSO
    }
}
