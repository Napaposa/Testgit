using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_ID4P.Model
{
    public class OrderModel
    {
        public string MaterialNo { get; set; } //* Exist in EngineDesignSheet
        public int BDPerGrip { get; set; } //* Exist in EngineDesignSheet // Stacking Enable
        //public int SideGuide { get; set; }
        public int BottomSheet { get; set; } //* Exist in EngineDesignSheet
        public bool TopSheet { get; set; } = true; //* Added
        //Bundle
        public double BundleWidth { get; set; } //* Exist in EngineDesignSheet
        public double BundleLength { get; set; } //* Exist in EngineDesignSheet
        public double BundleHeight { get; set; } //* Exist in EngineDesignSheet
        public double BundleWeight { get; set; } //* Exist in EngineDesignSheet
        public int SheetPerBundle { get; set; }
        //TieSheet
        public int TSEveryXLY { get; set; } //* Exist in EngineDesignSheet
        public string ArrTieSheet { get; set; } //* Exist in EngineDesignSheet
        public int TieSheetWidth { get; set; } //* Exist in EngineDesignSheet
        public int TieSheetLength { get; set; } //* Exist in EngineDesignSheet
        //Pallet
        public string PalletType { get; set; }
        public int PalletWidth { get; set; } //* Exist in EngineDesignSheet
        public int PalletLength { get; set; } //* Exist in EngineDesignSheet
        public int PalletHeight { get; set; } //* Exist in EngineDesignSheet
        //Pattern
        public int LayerPerPallet { get; set; } //* Exist in EngineDesignSheet
        public int BundlePerLayer { get; set; } //* Exist in EngineDesignSheet
        //Finger
        public bool FingerRequired { get; set; } //* Exist in EngineDesignSheet
        //LayerPerimeter
        public int Peri_CloseW { get; set; } //* Exist in EngineDesignSheet
        public int Peri_CloseL { get; set; } //* Exist in EngineDesignSheet
        //Squaring
        public bool Squaring { get; set; } //* Exist in EngineDesignSheet
        public bool SwitchBDSize { get; set; }
        public string SplitSOText { get; set; }
        public int OrderBundle { get; set; }
        public int[] Arr_SheetPerSO { get; set; }
        public string OrderItem { get; set; }
        public string SOSplit { get; set; }
        public int[] Arr_SOID { get; set; }
        public string ProductCode { get; set; }
        //Tie Sheet Special
        public bool TS_B4LastLY { get; set; }
        public bool TS_LowSQLayer { get; set; }
        //Special NoLiftStack & ExtraSQ
        public bool STKLiftBD { get; set; } //* Exist in EngineDesignSheet
        public bool STKAntiBounce { get; set; }
        public bool ExtraSQ { get; set; } //* Exist in EngineDesignSheet
        public int HeightRatio { get; set; }
        // Placing Mode
        public int PlacingMode { get; set; }
        // Discharge Mode
        public int DischargeMode { get; set; }

        public bool ExtraPickDepth { get; set; }

        public bool AutoLotEnd { get; set; } // Added for AutoLot End Function 15/06/2023


        public string GetArrTieSheet(int buf_TSEveryXLY = 0, int buf_BDPerGrip = 0, int buf_LayerPerPallet = 0)
        {
            if (buf_TSEveryXLY > 0)
                TSEveryXLY = buf_TSEveryXLY;
            if (buf_BDPerGrip != 0)
                BDPerGrip = buf_BDPerGrip;
            if (buf_LayerPerPallet > 0)
                LayerPerPallet = buf_LayerPerPallet;

            int MaxTieSheetLayer = Properties.Settings.Default.Max_TSLayer;
            int[] TSLayer = new int[MaxTieSheetLayer];
            int[] adj_TSLayer = new int[MaxTieSheetLayer];

            try
            {
                if (TSEveryXLY > 0)
                {
                    /*if (BDPerGrip == 2)
                    {
                        if ((TSEveryXLY % 2) != 0)
                            TSEveryXLY++;
                    }*/

                    int PrvTSLayer = TSEveryXLY;
                    int adj_PrvTSLayer = PrvTSLayer % BDPerGrip == 0 ? TSEveryXLY : TSEveryXLY + 1 ;
                    int Skipped_index = 0;
                    if (adj_PrvTSLayer < LayerPerPallet)
                    {
                        for (int i = 0; i < MaxTieSheetLayer - 1; i++)
                        {
                            //Skip Squaring Height
                            int CurrentHeight = Convert.ToInt32(BundleHeight * adj_PrvTSLayer);
                            // Assuming Squaring will be above Conveyor not Pallet when it's at Low Level
                            if (CurrentHeight < (Properties.Settings.Default.SQFrame_Height - PalletHeight) && TS_LowSQLayer == false)
                            {
                                PrvTSLayer += TSEveryXLY;
                                adj_PrvTSLayer = PrvTSLayer % BDPerGrip == 0 ? PrvTSLayer : PrvTSLayer + 1;
                                Skipped_index++; // Counting the skipped index to be use to subtract later.
                                continue;
                            }
                            if (Array.IndexOf(adj_TSLayer, PrvTSLayer) != -1)
                            {
                                Skipped_index++;
                                PrvTSLayer += TSEveryXLY;
                                continue;
                            }
                            if (PrvTSLayer < LayerPerPallet)
                            {
                                TSLayer[i - Skipped_index] = PrvTSLayer; // Shift the index of actual TieSheet Layer according to skipped index.
                                adj_TSLayer[i - Skipped_index] = PrvTSLayer % BDPerGrip == 0 ? PrvTSLayer : PrvTSLayer + 1 >= LayerPerPallet ? 0 : PrvTSLayer + 1;
                            }
                            else
                            {
                                break;
                            }
                            PrvTSLayer += TSEveryXLY;
                        }

                        if (TS_B4LastLY == true)
                        {
                            int buf_LastLYSubtract = (LayerPerPallet - 1) % BDPerGrip == 0 ? 1 : 2;
                            int LY_B4Last = (LayerPerPallet - buf_LastLYSubtract);
                            if (adj_TSLayer.Contains(LY_B4Last) == false)
                            {
                                int ClosestFreeIndex = Array.IndexOf(adj_TSLayer, 0);
                                if (ClosestFreeIndex > 0)
                                    adj_TSLayer[ClosestFreeIndex] = LY_B4Last;
                            }
                        }
                    }
                }
                ArrTieSheet = string.Format("[{0}]", string.Join(",", adj_TSLayer));
            }
            catch (Exception)
            {

            }

            return ArrTieSheet;
        }

        public string GetTextSplitSO()
        {
            string result = "";

            OrdersBundleModel OBs = new OrdersBundleModel();
            OBs.GetOrderBundlefromString(SplitSOText, SheetPerBundle, SOSplit);
            result = OBs.GetOrderBundleArrayText();

            return result;
        }

        public int[] GetArrSplitSO()
        {
            int[] result;

            OrdersBundleModel OBs = new OrdersBundleModel();
            OBs.GetOrderBundlefromString(SplitSOText, SheetPerBundle, SOSplit);
            result = OBs.GetOrderBundleArray();

            return result;
        }


        public int GetOrderBundle()
        {
            string[] Arr_SheetPerSO = SplitSOText.Split(',');
            int[] intArray = Array.ConvertAll(Arr_SheetPerSO, int.Parse);
            int result = intArray.Sum() / SheetPerBundle;

            return result;
        }

        public string[] GetArrSO()
        {
            string[] result;

            OrdersBundleModel OBs = new OrdersBundleModel();
            OBs.GetOrderBundlefromString(SplitSOText, SheetPerBundle, SOSplit);
            result = OBs.GetOrderSOArray();

            return result;
        }



        public class TieSheetSizeModel
        {
            public int Width { get; set; }
            public int Length { get; set; }
            public string GetTieSheetText()
            {
                return string.Format("{0}x{1}", Length, Width);
            }
        }

    }
}
