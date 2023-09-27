using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ATD_ID4P.Class;
using Microsoft.Office.Interop.Excel;

namespace ATD_ID4P.Model
{
    public class PatternModel
    {
        public string Pattern { get; set; }
        public string PatternType { get; set; }
        public string OptionCode { get; set; }
        public int BundlePerLayer { get; set; }
        public int BundleWidth { get; set; }
        //*rename public int BundleLong { get; set; }
        public int BundleLength { get; set; }
        //public int RotatePattern { get; set; }
        public bool RotatePattern { get; set; } // we remove sub rotate ver3.0 2023-05-09
        public bool FixBundleFace { get; set; }
        public int BundleGap { get; set; }
        public int PalletWidth { get; set; }
        //*rename public int PalletLong { get; set; }
        public int PalletLength { get; set; }
        public int PatternLayer { get; set; }
        //*rename public int TotalLayer { get; set; }
        public int TotalPatternBundles { get; set; }
        //*rename* public Laying[] PatternLayers { get; set; }
        public Laying[] PatternBundle { get; set; }
        //*rename* public Laying[] PatternLayersWithOutGap { get; set; }
        public Laying[] PatternBundleWithOutGap { get; set; }
        //*public int SideGuide { get; set; }
        //*public bool PalletIsRotated { get; set; }
        public string SwitchPattern{ get; set; }
        

        public int SQ_Max_PeriX { get; set; }
        public int SQ_Max_PeriY { get; set; }
        public int SQ_OriginBias { get; set; }

        public int SQ_YaxisValue { get; set; }
        //public int SQ_XaxisLeftValue { get; set; }
        public int SQ_XaxisValue { get; set; }

        //public int SQ_XaxisValue { get; set; } // PLS

        //public int SQ_XaxisValueWithOutGap { get; set; } //PLS
        public int SQ_XaxisValueWithOutGap { get; set; }
        //public int SQ_XaxisLeftValueWithOutGap { get; set; }
        public int SQ_YaxisValueWithOutGap { get; set; }
        //*remove public int PalletStoppingOffset { get; set; }

        //*rename* public int LayerPerimeterWidth { get; set; }
        public int LayerClosePeriWidth { get; set; }
        //*rename* public int LayerPerimeterLength { get; set; }
        public int LayerClosePeriLength { get; set; }
        public int LayerOpenPeriWidth { get; set; }
        public int LayerOpenPeriLength { get; set; }

        //private PatternEngineClass Engine = new PatternEngineClass();
        private PatternEngineSquaringClass Engine = new PatternEngineSquaringClass();
        //private const int SGLimit = 150;
        public bool SwithBDSize { get; set; }

        public bool SuggestRotate { get; set; }

        public bool SpecialRotate { get; set; }

        public void UpdatePatternDetail()
        {
            //  S0422
            //  S   :   Pattern Type
            //  04  :   Pattern Layer & Bundle Per Layer
            //  22  :   Option Code
            if (string.IsNullOrEmpty(Pattern))
                return;

            PatternType = Pattern.Substring(0, 1);
            PatternLayer = Convert.ToInt32(Pattern.Substring(1, 2));
            OptionCode = Pattern.Substring(3, 2);
            BundlePerLayer = int.Parse(Pattern.Substring(1, 2));

            //SQ_MaxX = Engine.SQ_FixedArmBiasFromSQcenter + Engine.SQ_Max_PeriXFromSQcenter; //PLS
            SQ_Max_PeriY = Engine.SQ_Max_PeriY;
            SQ_Max_PeriX = Engine.SQ_Max_PeriX;
            SQ_OriginBias = Engine.SQ_XaxisLeftZeroBiasFromRobotOrigin;
            //SQ_OriginBias = 0; //PLS
        }

        public Laying[] GetPlacementInfo(bool HiddenMode = false)
        {
            PatternBundle = null;

            Engine.BundlePerLayer = BundlePerLayer;
            Engine.BundleWidth = BundleWidth;
            Engine.BundleLength = BundleLength;
            Engine.BundleGap = BundleGap;
            Engine.PalletWidth = PalletWidth;
            Engine.PalletLength = PalletLength;
            Engine.SwapSide = SwithBDSize;     //Swap side @ 04-07-2020
            Engine.SpecialRotate = SpecialRotate; //Special Rotate 2020-10-06
            Engine.RotatePattern = RotatePattern; // ver3.0 2023-05-09

            Engine.ClearLayerPattern();

            if (PatternType == "S")
            {
                //*rename TotalLayer = PatternLayer * 4;
                TotalPatternBundles = BundlePerLayer * 4;
            }
            else
            {
                //*rename TotalLayer = PatternLayer * 2;
                TotalPatternBundles = BundlePerLayer * 2;
            }
            //  C0111 -> 01 * 2 = 2
            //  C0221 -> 02 * 2 = 4
            //  I0321 -> 03 * 2 = 6
            //  S0422 -> 04 * 4 = 16
            //  S0844 -> 08 * 4 = 16
            //  C0933 -> 09 * 2 = 18

            //if (Pattern == "I0312") //New Pattern 08-09-2020 @Beer
            //    TotalLayer = 9;

            PatternBundle = new Laying[TotalPatternBundles];
            PatternBundleWithOutGap = new Laying[TotalPatternBundles];

            bool IsGenError;
            switch (PatternType)
            {
                case "C":
                    {
                        IsGenError = Engine.PatternDataGenerator("C", OptionCode);
                        //*change* if (RotatePattern >= 0)
                        if (FixBundleFace)
                            IsGenError = Engine.PatternDataGenerator("C_FixFace", OptionCode);

                        if (IsGenError)
                        {
                            if (HiddenMode == false)
                                ResponseMessage("Generate pattern auto fail!!!");
                            return PatternBundle;
                        }
                        else
                        {
                            CreatePatternLayer();
                            CreateLayer(1);
                            CreateLayer(2);
                        }
                        break;
                    }
                case "I":
                    {
                        IsGenError = Engine.PatternDataGenerator("I", OptionCode);
                        if (IsGenError)
                        {
                            if (HiddenMode == false)
                                ResponseMessage("Generate pattern auto fail!!!");
                            return PatternBundle;
                        }
                        else
                        {
                            CreatePatternLayer();
                            CreateLayer(1);
                            CreateLayer(2);
                            //if (Pattern == "I0312") //New Pattern I0312 08-09-2020 @Beer
                            //{
                            //    CreatePatternLayer();
                            //    CreateLayer(1);
                            //    CreateLayer(2);
                            //    CreateLayer(3);
                            //}
                            //else
                            //{
                            //    CreatePatternLayer();
                            //    CreateLayer(1);
                            //    CreateLayer(2);
                            //}                            
                        }
                        break;
                    }
                case "S":
                    {
                        IsGenError = Engine.PatternDataGenerator("S", OptionCode);
                        if (IsGenError)
                        {
                            if (HiddenMode == false)
                                ResponseMessage("Generate pattern auto fail!!!");
                            return PatternBundle;
                        }
                        else
                        {
                            CreatePatternLayer();
                            CreateLayer(1);
                            CreateLayer(2);
                            CreateLayer(3);
                            CreateLayer(4);
                        }
                        break;
                    }
            }

            return PatternBundle;
        }

        private void CreatePatternLayer()
        {
            #region Rotate 'SourcePattern' 90 degree and check overhang value 
            //  -   Clear 'LayerPattern' array both w/ & w/o Gap
            //  -   Copy 'SourcePattern' to 'ProcessingPattern'
            //  -   Copy 'SourcePattern' Dimension to 'ProcessingPattern' Dimension
            //  -   Rotate 'ProcessingPattern' and Swap it Dimension
            //  -   Copy Rotated 'ProcessingPattern' to 'RotatedPattern'
            //  -   Calculate 'RotatedPattern' Dimension
            //  -   Compare Overhang value of 'SourcePattern' and 'RotatedPattern'
            //      -   If 'SourcePattern' is underhange choose it
            //      -   ElseIf  'PatternType' is 'S' choose 'SourcePattern'
            //      -   ElseIf  'RotatedPattern' is underhange choose it
            //      -   Else    both are overhang choose the least overhang
            //  -   Assign choose pattern and dimension to 'LayerPattern'
            #endregion
            Engine.Layer1PatternDirectionEvaluation();

            #region If Bundle Rotation is selected -> Calculate Pattern Data again (Include Overhang)
            //  -   Clear 'LayerPattern' array
            //  -   Copy 'SourcePattern' to 'ProcessingPattern'
            //  -   Rotate 'ProcessingPattern' according to selected BundleRotate
            //  -   Copy Rotated 'ProcessingPattern' to 'LayerPattern' array
            //  -   Copy Rotated Dimension to 'LayerPattern'
            //  -   Calculate New Overhang from Rotated Pattern
            #endregion
            // we don't use sub rotate anymore ver3.0 2023-05-09
            //if (RotatePattern >= 0) // int? mean it can be null so this
            //{
            //    Engine.Layer1PatternOverideGenerator(RotatePattern);
            //    Engine.Layer1PatternOverideGeneratorWithOutGap(RotatePattern);
            //}
            #region Calculate switching pattern(s) from 'LayerPattern'
            //  -   Copy index 0 of 'LayerPattern' array to 'ProcessingPattern'
            //  -   'C' & 'I' pattern will only have 2 switching pattern
            //      -   'LayerPattern'[0]
            //      -   'LayerPattern'[1] = Modified 'LayerPattern'[1]
            //  -   'S' pattern will have 4 switch pattern
            //      -   'LayerPattern'[0]
            //      -   'LayerPattern'[1] = Mirrored 'LayerPattern'[0]
            //      -   'LayerPattern'[2] = 180-degree Rotated 'LayerPattern'[1]
            //      -   'LayerPattern'[3] = 180-degree Rotated 'LayerPattern'[0]
            #endregion
            Engine.NextLayerDataGenerator();
            Engine.NextLayerDataGeneratorWithOutGap();

            Engine.ReOrderPatternSequence();
            //Engine.ReOrderPatternSequence(Properties.Settings.Default.MirrorLayout); // Modify function for mirrored layout ver3.0 2023-05-08

            #region Shift Y-Axis to 'Pallet's Bottom-Edge' instead of 'Pattern-Edge'
            //  -   Copy 'LayerPattern' array to 'ProcessingPattern'
            //  -   Shift Y-Axis to 'Pallet's Bottom-Edge'
            //  -   Copy shifted 'ProcessingPattern' to 'LayerPattern' array
            #endregion
            Engine.PatternOffsetAndOverHangCalculation();

            #region Invert Y-Axis Direction
            //  -   Copy 'LayerPattern' array to 'ProcessingPattern'
            //  -   Invert Y value of 'ProcessingPattern' element
            //  -   Copy 'ProcessingPattern' to 'LayerPattern'
            #endregion
            Engine.YaxisInverting();

            //  Calculate Squaring Moving Distance on both X,Y-Axis
            Engine.SquaringMoveCalculation();

            //  Pass Squaring Moving Distance from class Engine to this Class variable
            SQ_YaxisValue = Engine.SQ_YaxisValue;
            SQ_XaxisValue = Engine.SQ_XaxisRightValue;
            ////Disable for TCRB
            //UpdateSquaringOpen();   //Check Open 1300x1300

            //  Pass Squaring Moving Distance from class Engine to this Class variable
            SQ_XaxisValueWithOutGap = Engine.SQ_XaxisRightValueWithOutGap;
            SQ_YaxisValueWithOutGap = Engine.SQ_YaxisValueWithOutGap;

            //Perimeter
            //  Pass Rotate Flag from class Engine to this Class variable
            SuggestRotate = Engine.SuggestRotate; // This flag will be set/reset at Layer1PatternDirectionEvaluatio()
            //  Pass Perimeter value from class Engine to this Class variable
            //LayerClosePeriLength = Engine.Layer1PatternDimensionWithOutGap.MaxY;
            //LayerClosePeriWidth = Engine.Layer1PatternDimensionWithOutGap.MaxX;
            LayerClosePeriLength = Engine.Layer1PatternDimensionWithOutGap.MaxX;
            LayerClosePeriWidth = Engine.Layer1PatternDimensionWithOutGap.MaxY;

            //Use in pattern interlock in VerifyPattern()
            //SQ OneSide: Use for Determining External Squaring Options
            LayerOpenPeriLength = Engine.Layer1PatternDimension.MaxX;
            LayerOpenPeriWidth = Engine.Layer1PatternDimension.MaxY;

            //Special Case for I0312 08-09-2020 @Beer *checkagain*
            if (Pattern == "I0312")
            {
                SQ_YaxisValue -= Convert.ToInt32(BundleWidth * Properties.Settings.Default.I0312_Gap);
            }


            ////Update Sequence for Bundle Press
            //Engine.ReOrderPatternSequence();
        }

        private void CreateLayer(int Floor)
        {
            int d1 = Floor - 1; //Dimention
            int d2 = 0; //Dimension
            int iStart = (BundlePerLayer * Floor) - BundlePerLayer; // calculate start bundle index (from Total Bundle of all Switching Pattern)
            //if (Floor == 4 || Floor == 3)   //Spiral Swap 3 and 4
            //    d1 = (Floor == 3 ? 3 : 2);  // If Floor = 3 -> d1 = 3 // If Floor = 4 -> d1 = 2

            //if (Pattern == "I0312" && Floor == 3)
            //    d1 = 0;

            // PatternLayers store individual bundle data not array of bundle in the pattern
            //*rename for (int i = iStart; i < (PatternLayer * Floor); i++)
            for (int i = iStart; i < (BundlePerLayer * Floor); i++)
            {
                PatternBundle[i].PatternName = Pattern;
                PatternBundle[i].Step = i + 1; // index start at 0 but actual step is start at 1
                //PatternLayers[i].Row = Engine.LayerPattern[d1, d2].Row;
                PatternBundle[i].X = Engine.LayerPattern[d1, d2].x;
                PatternBundle[i].Y = Engine.LayerPattern[d1, d2].y;
                PatternBundle[i].Ori = Engine.LayerPattern[d1, d2].ori;
                PatternBundle[i].Layer = Floor;

                PatternBundleWithOutGap[i].PatternName = Pattern;
                PatternBundleWithOutGap[i].Step = i + 1;
                //PatternLayersWithOutGap[i].Row = Engine.LayerPatternWithOutGap[d1, d2].Row;
                PatternBundleWithOutGap[i].X = Engine.LayerPatternWithOutGap[d1, d2].x;
                PatternBundleWithOutGap[i].Y = Engine.LayerPatternWithOutGap[d1, d2].y;
                PatternBundleWithOutGap[i].Ori = Engine.LayerPatternWithOutGap[d1, d2].ori;
                PatternBundleWithOutGap[i].Layer = Floor;

                //Special Case for I0312 08-09-2020 @Beer
                if (Pattern == "I0312")
                {
                    switch (Floor)
                    {
                        case 1:
                            {
                                PatternBundle[i].Y += Convert.ToInt32(BundleWidth * Properties.Settings.Default.I0312_Gap);
                                break;
                            }
                        case 2:
                            {
                                PatternBundle[i].Y -= Convert.ToInt32(BundleWidth * Properties.Settings.Default.I0312_Gap);
                                break;
                            }
                    }
                }

                d2++;
            }


        }

        public void ResponseMessage(string Message, string Title = "Pattern Model")
        {
            MessageBox.Show(Message, Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // use at Draw Bundle
        public Laying[] GetPlacementInfo_AtLayer(int Layer, Laying[] Lays_WGap, Laying[] Lays_WOGap = null, bool IsPressMode = false)
        {
            Laying[] result = null;
            if (Lays_WGap == null)
                return result;

            result = Lays_WGap.Where(x => x.Layer == Layer).ToArray();
            if (IsPressMode == true)
                result = Lays_WOGap.Where(x => x.Layer == Layer).ToArray();

            return result;
        }

        private void UpdateSquaringOpen()
        {
            int Temp = 0;
            int MinSize = 1300;
            Temp = SQ_Max_PeriX - Math.Abs(SQ_XaxisValue);
            if (Temp < MinSize)
            {
                SQ_XaxisValue = SQ_Max_PeriX - MinSize;
            }

            Temp = SQ_Max_PeriY - Math.Abs(SQ_YaxisValue);
            if (Temp < MinSize)
            {
                SQ_YaxisValue = SQ_Max_PeriY - MinSize;
            }
        }


        public int[] GetPlacementInfo_Field(Laying[] Lays_WGap, string Field,string PatternSeq = null)
        {
            int MaxArray = 60;
            var resp = new int[MaxArray];
            var LayTemp = new int[MaxArray];
            var SWPattern = new int[MaxArray];
            int LastIndex = 0;
            int LastCopyIDX = 0;
            int MaxBundlerPerLayer = Properties.Settings.Default.Max_BDperLayer;
            if (String.IsNullOrEmpty(PatternSeq))
            {
                if (Lays_WGap[0].PatternName[0] == 'S')
                    PatternSeq = "1:1:1:1:1";
                else
                    PatternSeq = "1:1:1:0:0";
            }            

            int buf_PatSW1 = (PatternSeq[2] - '0');
            int buf_PatSW2 = (PatternSeq[4] - '0')*2;
            int buf_PatSW3 = (PatternSeq[6] - '0')*3;
            int buf_PatSW4 = (PatternSeq[8] - '0')*4;
            switch (Field)
            {
                case "X":
                    {
                        LayTemp = Lays_WGap.Where(element => (element.Layer == buf_PatSW1 ||
                                                        element.Layer == buf_PatSW2 ||
                                                        element.Layer == buf_PatSW3 ||
                                                        element.Layer == buf_PatSW4))
                                    .Select(element => element.X)
                                    .ToArray();
                        //LayTemp = Lays.Select(element => element.X).ToArray();
                        break;
                    }
                case "Y":
                    {
                        LayTemp = Lays_WGap.Where(element => (element.Layer == buf_PatSW1 ||
                                                        element.Layer == buf_PatSW2 ||
                                                        element.Layer == buf_PatSW3 ||
                                                        element.Layer == buf_PatSW4))
                                    .Select(element => element.Y)
                                    .ToArray();
                        //LayTemp = Lays.Select(element => element.Y).ToArray();
                        break;
                    }
                case "Ori":
                    {
                        LayTemp = Lays_WGap.Where(element => (element.Layer == buf_PatSW1 ||
                                                        element.Layer == buf_PatSW2 ||
                                                        element.Layer == buf_PatSW3 ||
                                                        element.Layer == buf_PatSW4))
                                    .Select(element => element.Ori)
                                    .ToArray();
                        //LayTemp = Lays.Select(element => element.Ori).ToArray();
                        break;
                    }
            }

            //Create Data
            for (int i = 0; i < resp.Length; i++)
            {
                if (i < BundlePerLayer)
                {
                    resp[i] = LayTemp[i];
                    LastIndex++;
                    LastCopyIDX++;
                }
                else
                {
                    if ((i + 1) % MaxBundlerPerLayer == 1)
                        LastIndex = 0;

                    if (LastIndex < BundlePerLayer && LayTemp.Length > LastCopyIDX)
                    {
                        resp[i] = LayTemp[LastCopyIDX];
                        LastCopyIDX++;
                    }

                    LastIndex++;
                }
            }

            return resp;
        }
    }

    public struct Laying
    {
        //0Name, 1Step, 2Row, 3Bundle, 4AbutX, 5AbutY, 6Layer, 7ID, 8X, 9Y, 10Xb, 11Yb
        public string PatternName;
        public int Step;
        public int Row;
        //public string Bundle;
        public int AbutX;
        public int AbutY;
        public int Layer;
        public int X;
        public int Y;
        public double X2;
        public double Y2;
        public int Ori;
    }
}
