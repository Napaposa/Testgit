using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_ID4P.Class
{
    class PatternEngineSquaringClass
    {
        public struct Dimension
        {
            public int MaxY;
            public int MaxX;
        }
        public Dimension SourcePatternDimension;
        public Dimension SourcePatternDimensionWithOutGap;
        public Dimension RotatedPatternDimension;
        public Dimension RotatedPatternDimensionWithOutGap;
        public Dimension Layer1PatternDimension;
        public Dimension Layer1PatternDimensionWithOutGap;
        public Dimension ProcessingPatternDimension;              //For temporary use during pass to function
        public struct Pattern
        {
            public int x;
            public int y;
            public int ori;
        }
        public Pattern[] ProcessingPattern = new Pattern[32];   //For temporary use during pass to function
        public Pattern[] ProcessingPattern2 = new Pattern[32];
        //public Pattern[] SourcePattern = new Pattern[32];
        //public Pattern[] SourcePatternWithOutGap = new Pattern[32];
        public Pattern[,] SourcePattern = new Pattern[2,32];
        public Pattern[,] SourcePatternWithOutGap = new Pattern[2,32];
        public Pattern[,] RotatedPattern = new Pattern[2,32];
        public Pattern[,] RotatedPatternWithOutGap = new Pattern[2,32];
        public Pattern[,] LayerPattern = new Pattern[4, 32];
        public Pattern[,] LayerPatternWithOutGap = new Pattern[4, 32];
        public int PalletWidth;
        //*rename* public int PalletLong;
        public int PalletLength;
        public int BundleWidth;
        //*rename* public int BundleLong;
        public int BundleLength;
        public int BundleGap;
        public int BundlePerLayer;

        //Squaring parameter//
        public int SQ_Max_PeriX = Properties.Settings.Default.Peri_Max_OpenL;//1800; //mm
        public int SQ_Max_PeriY = Properties.Settings.Default.Peri_Max_OpenW;//1600; //mm
        public int SQ_Max_DistX = Properties.Settings.Default.SQ_Max_X;
        public int SQ_Max_DistY = Properties.Settings.Default.SQ_Max_Y;
        public int SQ_XaxisLeftZeroBiasFromRobotOrigin = -300; //mm
        //public int SQ_OpenSafeGap = 50;      //mm

        public int SQ_XaxisRightValue;
        public int SQ_YaxisValue;
        public int SQ_XaxisRightValueWithOutGap;
        public int SQ_YaxisValueWithOutGap;

        private bool flag_a = false;
        private bool flag_b = false;
        private bool flag_c = false;


        public struct OverHang
        {
            public int X;
            public int Y;
            public int Total;
            public int Xleft;
            public int Xright;
        }
        public OverHang LayerOverHang;
        public OverHang LayerOverHangWithOutGap;

        private string PatternType;
        private string PatternOptionCode;
        //public int PatternOverideBundleRot;
        //Pattern Rotate
        //private int BundleRotate = -1;
        public bool RotatePattern = false; // rename and mod ver3.0 2023-05-09
        //public bool PatternIsRotate = false;
        public bool SuggestRotate = false; // Rename ver3.0 2023-05-09

        //Incase of มัดกล่องในแนวขวางลอน
        public bool SwapSide;
        //Incase of W more than L
        public bool SpecialRotate;

        //===========================================================================
        //========================Pattern general function===========================
        //===========================================================================

        //===========================================================================
        //=========================Pattern rotation tool=============================
        //===========================================================================
        public void InternalBundleSemiCircleRotate()
        {
            for (int n = 0; n < BundlePerLayer; n++)
            {
                switch (ProcessingPattern[n].ori)
                {
                    case 0:         //0 degree
                        ProcessingPattern[n].ori = 2;
                        ProcessingPattern[n].x = ProcessingPattern[n].x - BundleWidth;
                        ProcessingPattern[n].y = ProcessingPattern[n].y + BundleLength;
                        break;
                    case 1:         //90 degree
                        ProcessingPattern[n].ori = 3;
                        ProcessingPattern[n].x = ProcessingPattern[n].x - BundleLength;
                        ProcessingPattern[n].y = ProcessingPattern[n].y - BundleWidth;
                        break;
                    case 2:         //180 degree
                        ProcessingPattern[n].ori = 0;
                        ProcessingPattern[n].x = ProcessingPattern[n].x + BundleWidth;
                        ProcessingPattern[n].y = ProcessingPattern[n].y - BundleLength;
                        break;
                    case 3:         //270 degree
                        ProcessingPattern[n].ori = 1;
                        ProcessingPattern[n].x = ProcessingPattern[n].x + BundleLength;
                        ProcessingPattern[n].y = ProcessingPattern[n].y + BundleWidth;
                        break;
                }
            }
        }
        //===========================================================================
        public void PatternRotate()
        {
            int i;
            int nX;
            int nY;
            int n = 0;
            int MaxX = ProcessingPatternDimension.MaxX;
            int MaxY = ProcessingPatternDimension.MaxY;
            int MostBottom = 0;

            /// Rotate Pattern Dimension ///
            n = MaxX;
            MaxX = MaxY;
            MaxY = n;

            /// Rotate Pattern 90 degree CW around original point ///
            /// which will resulted in
            /// - X-axis will be a horizontal line (Y-axis)
            /// - Y-axis will be a vertical line (X-axis)
            /// - new X-axis will be at the center of the rotated pattern
            /// - new Y-axis will be at the left edge of the rotated pattern
            /// 
            ///                            newx (-oldy)
            ///                                 l_________o
            ///            x                    l         ]
            ///     o______l_______             l         ]
            ///     [      l      ]          ___l_________]_____ newy (+oldx)
            ///     [      l      ]             l         ]
            ///  ___[______l______]__y          l         ]
            ///                                 l_________]
            ///                                 l         
            ///
            for (i = 0; i < BundlePerLayer; i++)
            {
                ProcessingPattern[i].ori = ProcessingPattern[i].ori + 1;
                if (ProcessingPattern[i].ori > 3) ProcessingPattern[i].ori = 0;
                nX = -ProcessingPattern[i].y;
                nY = ProcessingPattern[i].x;
                ProcessingPattern[i].x = nX;
                ProcessingPattern[i].y = nY;
            }

            /// Shift X axis to positive origin ///
            /// - Find bottomedge of each bundle
            ///
            ///    newx                         newx
            ///      l_________o                  l_________o   <--- x (200)
            ///      l         ]                  l         ]
            ///      l         ]                  l         ]
            ///   ___l_________]_____ newy     ___l_________]_____ newy
            ///      l         ]                  l         ]
            ///      l         ]                  l         ]
            ///      l_________]                  l_________]   <--- x - Bundle Length (200-400 = -200)
            ///      l                            l
            ///
            /// - Move newy axis to the bottom most edge of the layer
            /// 
            ///                                 newx
            ///                                   l_________o   <--- x = 200 + (-MostBottom)
            ///    newx                           l         ]          = 200 + ( -(-200) )
            ///      l                            l         ]          = 200 + ( 200 )
            ///      l_________o                  l         ]          = 400
            ///      l         ]                  l         ]
            ///      l         ]                  l         ]
            ///   ___l_________]_____ newy     ___l_________]_____ newy
            ///      l         ]
            ///      l         ]
            ///      l_________]
            ///      l
            ///      
            /// - Move newx axis to the center of the layer
            /// 
            ///     newx                              newx + ( -(MaxY / 2) ) = newx - (MaxY / 2)
            ///      l_________o                  _____l____o
            ///      l         ]                  [    l    ]
            ///      l         ]                  [    l    ]
            ///      l         ]                  [    l    ]
            ///      l         ]                  [    l    ]
            ///      l         ]                  [    l    ]
            ///   ___l_________]_____ newy     ___[____l____]_____ newy
            ///
            for (i = 0; i < BundlePerLayer; i++)
            {
                /// Find the bottom edge of this pattern ///
                /// - by comparing the bottom edge of each bundle in the pattern ///
                switch (ProcessingPattern[i].ori)
                {
                    case 0: n = ProcessingPattern[i].x - BundleWidth; break;
                    case 1: n = ProcessingPattern[i].x - BundleLength; break;
                    case 2: n = ProcessingPattern[i].x; break;
                    case 3: n = ProcessingPattern[i].x; break;
                }
                if (n < MostBottom) MostBottom = n;
            }
            XoffsetToProcessingPattern(-MostBottom);
            YoffsetToProcessingPattern(-(MaxY / 2));

            ProcessingPatternDimension.MaxX = MaxX;
            ProcessingPatternDimension.MaxY = MaxY;
        }
        //===========================================================================
        public void PatternMirror()
        {
            int i;
            int n = 0;
            int MostBottom = 0;

            /// Flip box ///
            for (i = 0; i < BundlePerLayer; i++)
            {
                switch (ProcessingPattern[i].ori)
                {
                    case 0:
                        ProcessingPattern[i].y = -ProcessingPattern[i].y - BundleLength;
                        break;
                    case 1:
                        ProcessingPattern[i].ori = 3;
                        ProcessingPattern[i].y = -ProcessingPattern[i].y;
                        ProcessingPattern[i].x = ProcessingPattern[i].x - BundleLength;
                        break;
                    case 2:
                        ProcessingPattern[i].y = -ProcessingPattern[i].y + BundleLength;
                        break;
                    case 3:
                        ProcessingPattern[i].ori = 1;
                        ProcessingPattern[i].y = -ProcessingPattern[i].y;
                        ProcessingPattern[i].x = ProcessingPattern[i].x + BundleLength;
                        break;
                }
            }

            /// Shift X axis to positive origin ///
            for (i = 0; i < BundlePerLayer; i++)
            {
                switch (ProcessingPattern[i].ori)
                {
                    case 0: n = ProcessingPattern[i].x - BundleWidth; break;
                    case 1: n = ProcessingPattern[i].x - BundleLength; break;
                    case 2: n = ProcessingPattern[i].x; break;
                    case 3: n = ProcessingPattern[i].x; break;
                }
                if (n < MostBottom) MostBottom = n;
            }
            XoffsetToProcessingPattern(-MostBottom);
        }
        //===========================================================================

        //===========================================================================
        //=======================Pattern data generator engine=======================
        //===========================================================================
        private void WriteBundlePosition(int PatIndex,int BDIndex, int X, int Y, int Ori)
        {
            SourcePattern[PatIndex, BDIndex].x = X;
            SourcePattern[PatIndex, BDIndex].y = Y;
            SourcePattern[PatIndex, BDIndex].ori = Ori;
        }
        //===========================================================================
        public bool PatternDataGenerator(string PatternType, string OptionCode)
        {
            bool ErrorFounded = false;
            PatternOptionCode = OptionCode;

            switch (PatternType)
            {
                case "I": ErrorFounded = (SpecialRotate ? I_PatternGenerator_SwapOri(BundleGap) : I_PatternGenerator_NormOri(BundleGap)); break;
                case "C": ErrorFounded = (SpecialRotate ? C_PatternGenerator_SwapOri(BundleGap, false) : C_PatternGenerator_NormOri(BundleGap,false)); break;
                case "S": ErrorFounded = (SpecialRotate ? S_PatternGenerator_SwapOri(BundleGap) : S_PatternGenerator_NormOri(BundleGap)); break;
                case "C_FixFace": ErrorFounded = (SpecialRotate ? C_PatternGenerator_SwapOri(BundleGap,true) : C_PatternGenerator_NormOri(BundleGap,true)); break;
                default: ErrorFounded = true; break;
            }
            if (!ErrorFounded)
            {
                for (int i = 0; i < 32; i++)
                {
                    ProcessingPattern[i] = SourcePattern[0, i];
                    ProcessingPattern2[i] = SourcePattern[1, i];
                }

                ProcessingPatternDimension = SourcePatternDimension;
                
                switch (PatternType)
                {
                    case "I": ErrorFounded = (SpecialRotate ? I_PatternGenerator_SwapOri(0) : I_PatternGenerator_NormOri(0)); break;
                    case "C": ErrorFounded = (SpecialRotate ? C_PatternGenerator_SwapOri(0, false) : C_PatternGenerator_NormOri(0,false)); break;
                    case "S": ErrorFounded = (SpecialRotate ? S_PatternGenerator_SwapOri(0) : S_PatternGenerator_NormOri(0)); break;
                    case "C_FixFace": ErrorFounded = (SpecialRotate ? C_PatternGenerator_SwapOri(0,true) : C_PatternGenerator_NormOri(0,true)); break;
                    default: ErrorFounded = true; break;
                }
                Array.Copy(SourcePattern, 0, SourcePatternWithOutGap, 0, 64);
                SourcePatternDimensionWithOutGap = SourcePatternDimension;

                for (int i = 0; i < 32; i++)
                {
                    SourcePattern[0, i] = ProcessingPattern[i];
                    SourcePattern[1, i] = ProcessingPattern2[i];
                }
                SourcePatternDimension = ProcessingPatternDimension;
            }
            return ErrorFounded;
        }
        //===========================================================================
        private bool I_PatternGenerator_NormOri(int Gap)
        {
            bool ErrorFounded = false;
            int W = BundleWidth;
            int L = BundleLength;
            int G = Gap;
            //int Gn = Gap/2;
            int Gn = Gap;
            int CenterY;
            int MaxX = 0;
            int MaxY = 0;
            int Indent_1 = 0;
            int Indent_2 = 0;
            int MinHangingDist = 30;
            PatternType = "I";
            switch (BundlePerLayer)
            {
                case 2:
                    if (PatternOptionCode == "21") // Verified ver3.0 2023-05-10
                    {
                        MaxX = W + G + W;
                        MaxY = L + (L / 2);
                        CenterY = MaxY / 2;
                        WriteBundlePosition(0,0, W,             CenterY - L,      0);
                        WriteBundlePosition(0,1, W + G,       - CenterY + L,      2);
                    }
                    break;
                case 3:     //Passed
                    if (PatternOptionCode == "21") // Verified ver3.0 2023-05-10
                    {
                        MaxX = W + G + L;
                        if ((G == 0 && flag_a) || (G != 0 && L > W + G + W))
                        {
                            MaxY = L;
                            flag_a = true;
                        }
                        else
                        {
                            MaxY = W + G + W;
                        }
                        //MaxY = (L > W + G + W) ?
                        //    L :
                        //    W + G + W;
                        CenterY = MaxY / 2;
                        WriteBundlePosition(0, 0, W,            - L / 2,        0);
                        WriteBundlePosition(0, 1, W + Gn + L,   - CenterY + W,  1);
                        WriteBundlePosition(0, 2, W + Gn,         CenterY - W,  3);

                        WriteBundlePosition(1, 0, L,            - CenterY + W, 1);
                        WriteBundlePosition(1, 1, 0,              CenterY - W, 3);
                        WriteBundlePosition(1, 2, L + G,          L / 2,    2);
                    }
                    break;
                case 5:     //Passed
                    if (PatternOptionCode == "32")
                    {
                        MaxX = W + G + L;
                        if ((G == 0 && flag_a) || (G != 0 && L + Gn + L > W + G + W + G + W))
                        {
                            MaxY = L + Gn + L;
                            flag_a = true;
                        }
                        else
                        {
                            MaxY = W + G + W + G + W;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist

                            if ((G == 0 && flag_b) || (G != 0 && (L - W - G) < MinHangingDist))
                            {
                                Indent_1 = W + G + MinHangingDist - L;
                                flag_b = true;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            //Indent_1 = ((L - W - G) < MinHangingDist) ?
                            //    W + G + MinHangingDist - L :
                            //    0;
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Pattern Centerline
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                            if ((G != 0) && (Indent_1 + L + (Gn / 2) > (MaxY / 2) || Indent_1 > (W / 3)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }

                        //if (L + Gn + L > W + G + W + G + W) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = L + Gn + L;
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W + G + W;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                        //    Indent_1 = ((L - W - G) < MinHangingDist) ?
                        //        W + G + MinHangingDist - L :
                        //        0;
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Pattern Centerline
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                        //    if (Indent_1 + L + (Gn / 2) > (MaxY / 2) || Indent_1 > (W/3))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        CenterY = MaxY / 2;
                        //1st Row
                        WriteBundlePosition(0,0, W,             - CenterY + Indent_1,   0);
                        WriteBundlePosition(0,1, 0,               CenterY - Indent_1,   2);
                        //2nd Row
                        WriteBundlePosition(0,2, W + Gn + L,    - CenterY + W,          1);
                        WriteBundlePosition(0,3, W + Gn,        - W / 2,                3);
                        WriteBundlePosition(0,4, W + Gn + L,      CenterY,              1);

                        //1st Row
                        WriteBundlePosition(1, 0, L,            - CenterY + W,          1);
                        WriteBundlePosition(1, 1, 0,            - W / 2,                3);
                        WriteBundlePosition(1, 2, L,              CenterY,              1);
                        //2nd Row
                        WriteBundlePosition(1, 3, L + G,        - CenterY + Indent_1 + L, 2);
                        WriteBundlePosition(1, 4, L + G + W,      CenterY - Indent_1 - L, 0);
                    }
                    break;
                case 6:     //Passed
                    if (PatternOptionCode == "42")
                    {
                        MaxX = W + G + L;

                        if ((L + Gn + L) / 2 > (W + G + W + G + W + G + W) / 2) // Verified ver3.0 2023-05-10
                        {
                            MaxY = L + Gn + L;
                            if (((G == 0) && (flag_c)) || ((G != 0) && (W + G + W < L)))
                            {
                                Indent_2 = L - W - G - W;
                                flag_c = true;
                            }
                            else
                            {
                                Indent_2 = 0;
                            }
                            flag_a = true;
                        }
                        else // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + G + W + G + W + G + W;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                            if (((G == 0) && (flag_b)) || ((G != 0) && ((L - W - G) < MinHangingDist)))
                            {
                                Indent_1 = W + G + MinHangingDist - L;
                                flag_b = true;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                            if ((G != 0) && (Indent_1 + L > W + G + W || Indent_1 > (W / 3)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }

                        //if ((L + Gn + L) / 2 > (W + G + W + G + W + G + W)/2) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = L + Gn + L;
                        //    Indent_2 = (W + G + W < L) ?
                        //        L - W - G - W:
                        //        0;
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W + G + W + G + W;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                        //    Indent_1 = ((L - W - G) < MinHangingDist) ?
                        //        W + G + MinHangingDist - L :
                        //        0;
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                        //    if (Indent_1 + L > W + G + W || Indent_1 > (W / 3))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        CenterY = MaxY / 2;

                        //1st Group
                        WriteBundlePosition(0,0, W,           - CenterY + Indent_1,           0);
                        WriteBundlePosition(0,1, W + Gn + L,  - CenterY + W,                  1);
                        WriteBundlePosition(0,2, W + Gn,      - CenterY + W + G + Indent_2,   3);
                        //2nd Group
                        WriteBundlePosition(0,3, L,             CenterY - W - G - Indent_2,   1);
                        WriteBundlePosition(0,4, 0,             CenterY - W,                  3);
                        WriteBundlePosition(0,5, MaxX - W,      CenterY - Indent_1,           2);


                        //1st Group
                        WriteBundlePosition(1, 0, 0,            - CenterY,                          3);
                        WriteBundlePosition(1, 1, L,            - CenterY + W + G + Indent_2 + W,   1);
                        WriteBundlePosition(1, 2, L + G,        - CenterY + Indent_1 + L,           2);
                        //2nd Group
                        WriteBundlePosition(1, 3, W,              CenterY - Indent_1 - L,           0);
                        WriteBundlePosition(1, 4, W + Gn,         CenterY - W - G - Indent_2 - W,   3);
                        WriteBundlePosition(1, 5, W + Gn + L,     CenterY       ,                   1);
                    }
                    break;
                case 7:     //Passed
                    if (PatternOptionCode == "34")
                    {
                        MaxX = W + G + W + G + L;

                        if (((G == 0) && (flag_a)) || (G != 0)&&(L + Gn + L > W + G + W + G + W)) // Verified ver3.0 2023-05-10
                        {
                            MaxY = L + Gn + L;
                            flag_a = true;
                        }
                        else // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + G + W + G + W;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                            if (((G == 0) && (flag_b)) || ((G != 0) && ((L - W - G) < MinHangingDist)))
                            {
                                Indent_1 = W + G + MinHangingDist - L;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Pattern Centerline
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                            if ((G != 0) && (Indent_1 + L + (Gn / 2) > (MaxY / 2) || Indent_1 > (W / 3)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }

                        //if (L + Gn + L > W + G + W + G + W) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = L + Gn + L;
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W + G + W;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                        //    Indent_1 = ((L - W - G) < MinHangingDist) ?
                        //        W + G + MinHangingDist - L :
                        //        0;
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Pattern Centerline
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                        //    if (Indent_1 + L + (Gn / 2) > (MaxY / 2) || Indent_1 > (W / 3))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        CenterY = MaxY / 2;

                        //1st Row
                        WriteBundlePosition(0,0, 0,                     - CenterY + Indent_1 + L,   2);
                        WriteBundlePosition(0,1, W,                       CenterY - Indent_1 - L,   0);
                        //2nd Row
                        WriteBundlePosition(0,2, W + G + W,             - CenterY + Indent_1,       0);
                        WriteBundlePosition(0,3, W + G,                   CenterY - Indent_1,       2);
                        //3rd Row
                        WriteBundlePosition(0,4, W + G + W + Gn,        - CenterY,                  3);
                        WriteBundlePosition(0,5, W + G + W + Gn + L,      W / 2,                    1);
                        WriteBundlePosition(0,6, W + G + W + Gn,          CenterY - W,              3);

                        //1st Row
                        WriteBundlePosition(1, 0, L,                    - CenterY + W,              1);
                        WriteBundlePosition(1, 1, 0,                    - W / 2,                    3);
                        WriteBundlePosition(1, 2, L,                      CenterY,                  1);
                        //2nd Row
                        WriteBundlePosition(1,3, L + G + W,             - CenterY + Indent_1,       0);
                        WriteBundlePosition(1,4, L + G,                   CenterY - Indent_1,       2);
                        //3rd Row
                        WriteBundlePosition(1,5, L + G + W + G,         - CenterY + Indent_1 + L,   2);
                        WriteBundlePosition(1,6, L + G + W + G + W,       CenterY - Indent_1 - L,   0);
                    }
                    break;
                case 10:
                    if (PatternOptionCode == "43")
                    {
                        MaxX = W + G + L + Gn + L;
                        if ((L + Gn + L) / 2 > (W + G + W + G + W + G + W) / 2) // Verified ver3.0 2023-05-10
                        {
                            MaxY = L + Gn + L;
                            if (((G == 0) && (flag_c)) || ((G != 0) && (W + G + W < L)))
                            {
                                Indent_2 = L - W - G - W;
                                flag_c = true;
                            }
                            else
                            {
                                Indent_2 = 0;
                            }
                            flag_a = true;
                        }
                        else // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + G + W + G + W + G + W;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                            if (((G == 0) && (flag_b)) || ((G != 0) && ((L - W - G) < MinHangingDist)))
                            {
                                Indent_1 = W + G + MinHangingDist - L;
                                flag_b = true;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                            if ((G != 0) && (Indent_1 + L > W + G + W || Indent_1 > (W / 3)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }

                        //if ((L + Gn + L) / 2 > (W + G + W + G + W + G + W) / 2) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = L + Gn + L;
                        //    Indent_2 = (W + G + W < L) ?
                        //        L - W - G - W :
                        //        0;
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W + G + W + G + W;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                        //    Indent_1 = ((L - W - G) < MinHangingDist) ?
                        //        W + G + MinHangingDist - L :
                        //        0;
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                        //    if (Indent_1 + L > W + G + W || Indent_1 > (W / 3))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        CenterY = MaxY / 2;

                        //1st Row
                        WriteBundlePosition(0,0, W,                     - CenterY + Indent_1,           0);
                        WriteBundlePosition(0,1, 0,                       CenterY - Indent_1,           2);
                        //2nd Row
                        WriteBundlePosition(0,2, W + Gn + L,            - CenterY + W,                  1);
                        WriteBundlePosition(0,3, W + Gn,                - CenterY + W + G + Indent_2,   3);
                        WriteBundlePosition(0,4, W + Gn + L,              CenterY - W - G - Indent_2,   1);
                        WriteBundlePosition(0,5, W + Gn,                  CenterY - W,                  3);
                        //3rd Row
                        WriteBundlePosition(0,6, W + Gn + L + Gn + L,   - CenterY + W,                  1);
                        WriteBundlePosition(0,7, W + Gn + L + Gn,       - CenterY + W + G + Indent_2,   3);
                        WriteBundlePosition(0,8, W + Gn + L + Gn + L,     CenterY - W - G - Indent_2,   1);
                        WriteBundlePosition(0,9, W + Gn + L + Gn,         CenterY - W,                  3);

                        //1st Row
                        WriteBundlePosition(1,0, L,                     - CenterY + W,                  1);
                        WriteBundlePosition(1,1, 0,                     - CenterY + W + G + Indent_2,   3);
                        WriteBundlePosition(1,2, L,                       CenterY - W - G - Indent_2,   1);
                        WriteBundlePosition(1,3, 0,                       CenterY - W,                  3);
                        //2nd Row
                        WriteBundlePosition(1,4, L + Gn + L,            - CenterY + W,                  1);
                        WriteBundlePosition(1,5, L + Gn,                - CenterY + W + G + Indent_2,   3);
                        WriteBundlePosition(1,6, L + Gn + L,              CenterY - W - G - Indent_2,   1);
                        WriteBundlePosition(1,7, L + Gn,                  CenterY - W,                  3);
                        //3rd Row   
                        WriteBundlePosition(1,8, L + Gn + L + G + W,    - CenterY + Indent_1,           0);
                        WriteBundlePosition(1,9, L + Gn + L + G,          CenterY - Indent_1,           2);
                    }
                    break;
                case 11:
                    if (PatternOptionCode == "34")
                    {
                        MaxX = W + G + L + Gn + L;
                        if (((G == 0) && (flag_a)) || ((G != 0) && ((L + Gn + L + Gn + L > W + G + W + G + W + G + W)))) // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + Gn + L + Gn + L;
                            // 2nd Vertical Bundle must overlap middle Horizontal Bundle atleast MinHangingDist
                            if (((G == 0) && (flag_b)) || ((G != 0) && (L + Gn + MinHangingDist > W + G + W)))
                            {
                                Indent_1 = L + Gn + MinHangingDist - W + G + W;
                                flag_b = true;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            flag_a = true;
                            // If the Indent is
                            //  -   2nd Vertical Bundle Overlapping Rim Horizontal Bundle less than MinHangingDist
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  This pattern is no allow
                            if ((G != 0) && (( W + G + Indent_1 > L - MinHangingDist || W + G + Indent_1 + W + (G / 2) > (MaxY / 2))))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }
                        else // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + G + W + G + W + G + W;
                        }

                        //if (L + Gn + L + Gn + L > W + G + W + G + W + G + W) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + Gn + L + Gn + L;
                            // 2nd Vertical Bundle must overlap middle Horizontal Bundle atleast MinHangingDist
                        //    Indent_1 = (L + Gn + MinHangingDist > W + G + W) ?
                        //        L + Gn + MinHangingDist - W + G + W :
                        //        0;
                            // If the Indent is
                            //  -   2nd Vertical Bundle Overlapping Rim Horizontal Bundle less than MinHangingDist
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  This pattern is no allow
                        //    if (W + G + Indent_1 > L - MinHangingDist || W + G + Indent_1 + W + (G / 2) > (MaxY / 2))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W + G + W + G + W;
                        //}
                        CenterY = MaxY / 2;
                        
                        //1st Row
                        WriteBundlePosition(0,0,  0,                    - CenterY + L,                  2);
                        WriteBundlePosition(0,1,  W,                    - L / 2,                        0);
                        WriteBundlePosition(0,2,  0,                      CenterY,                      2);
                        //2nd Row
                        WriteBundlePosition(0,3,  W + Gn + L,           - CenterY + W,                  1);
                        WriteBundlePosition(0,4,  W + Gn,               - CenterY + W + Indent_1 + G,   3);
                        WriteBundlePosition(0,5,  W + Gn + L,             CenterY - W - Indent_1 - G,   1);
                        WriteBundlePosition(0,6,  W + Gn,                 CenterY - W,                  3);
                        //3rd Row
                        WriteBundlePosition(0,7,  W + Gn + L + Gn + L,  - CenterY + W,                  1);
                        WriteBundlePosition(0,8,  W + Gn + L + Gn,      - CenterY + W + Indent_1 + G,   3);
                        WriteBundlePosition(0,9,  W + Gn + L + Gn + L,    CenterY - W - Indent_1 - G,   1);
                        WriteBundlePosition(0,10, W + Gn + L + Gn,        CenterY - W,                  3);

                        //1st Row
                        WriteBundlePosition(1, 0,   L,              - CenterY + W,                      1);
                        WriteBundlePosition(1, 1,   0,              - CenterY + W + Indent_1 + G,       3);
                        WriteBundlePosition(1, 2,   L,                CenterY - W - Indent_1 - G,       1);
                        WriteBundlePosition(1, 3,   0,                CenterY - W,                      3);
                        //2nd Row
                        WriteBundlePosition(1, 4,   L + Gn + L,     - CenterY + W,                      1);
                        WriteBundlePosition(1, 5,   L + Gn,         - CenterY + W + Indent_1 + G,       3);
                        WriteBundlePosition(1, 6,   L + Gn + L,       CenterY - W - Indent_1 - G,       1);
                        WriteBundlePosition(1, 7,   L + Gn,           CenterY - W,                      3);
                        //3rd Row
                        WriteBundlePosition(1, 8,   MaxX,           - CenterY,                          0);
                        WriteBundlePosition(1, 9,   MaxX - W,         L / 2,                            2);
                        WriteBundlePosition(1, 10,  MaxX,             CenterY - L,                      0);
                    }
                    break;
                default:
                    ErrorFounded = true;
                    break;
            }
            SourcePatternDimension.MaxX = MaxX;
            SourcePatternDimension.MaxY = MaxY;
            return ErrorFounded;
        }

        private bool I_PatternGenerator_SwapOri(int Gap)
        {
            bool ErrorFounded = false;
            int W = BundleWidth;
            int L = BundleLength;
            int G = Gap;
            int Gn = Gap/2;
            int CenterY;
            int MaxX = 0;
            int MaxY = 0;
            int Indent_1 = 0;
            int Indent_2 = 0;
            int MinHangingDist = 30;
            PatternType = "I";
            switch (BundlePerLayer)
            {
                case 2:
                    if (PatternOptionCode == "21") // Verified ver3.0 2023-05-17
                    {
                        MaxX = L + Gn + L;
                        MaxY = W + (W / 2);
                        CenterY = MaxY / 2;
                        WriteBundlePosition(0,0, 0,         CenterY - W,  3);
                        WriteBundlePosition(0,1, MaxX,    - CenterY + W,  1);
                    }
                    break;
                case 3:     //Passed
                    if (PatternOptionCode == "21") // Verified ver3.0 2023-05-17
                    {
                        MaxX = L + G + W;
                        if ((G == 0 && flag_a) || (G != 0 && W > L + Gn + L))
                        {
                            MaxY = W;
                            flag_a = true;
                        }
                        else
                        {
                            MaxY = L + Gn + L;
                        }
                        //MaxY = (W > L + Gn + L) ?
                        //    W;
                        //    L + Gn + L;
                        CenterY = MaxY / 2;
                        WriteBundlePosition(0,0, 0,           - W / 2,        3);
                        WriteBundlePosition(0,1, MaxX,        - CenterY,      0);
                        WriteBundlePosition(0,2, MaxX - W,      CenterY,      2);

                        WriteBundlePosition(1, 0, W,            - CenterY,      0);
                        WriteBundlePosition(1, 1, 0,              CenterY,      2);
                        WriteBundlePosition(1, 2, W + Gn + L,     W / 2,        1);
                    }
                    break;
                case 5:     //Passed
                    if (PatternOptionCode == "32") // Verified ver3.0 2023-05-17
                    {
                        MaxX = L + G + W;
                        if ((G == 0 && flag_a) || (G != 0 && W + G + W > L + Gn + L + Gn + L))
                        {
                            MaxY = W + G + W;
                            flag_a = true;
                        }
                        else
                        {
                            MaxY = L + Gn + L + Gn + L;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist

                            if ((G == 0 && flag_b) || (G != 0 && ((W - L - Gn) < MinHangingDist)))
                            {
                                Indent_1 = L + Gn + MinHangingDist - W;
                                flag_b = true;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Pattern Centerline
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                            if ((G != 0) && (Indent_1 + W + (G / 2) > (MaxY / 2) || Indent_1 > (L / 3)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }
                        //if (W + G + W > L + Gn + L + Gn + L)
                        //{
                        //    MaxY = W + G + W;
                        //}
                        //else // Verified ver3.0 2023-05-17
                        //{
                        //    MaxY = L + Gn + L + Gn + L;
                        // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                        //    Indent_1 = ((W - L - Gn) < MinHangingDist) ?
                        //        L + Gn + MinHangingDist - W :
                        //        0;
                        // If the Indent is
                        //  -   Making the Horizontal Bundle exceed Pattern Centerline
                        //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                        //  This pattern is no allow
                        //    if (Indent_1 + W + (G / 2) > (MaxY / 2) || Indent_1 > (L / 3))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        CenterY = MaxY / 2;
                        //1st Row
                        WriteBundlePosition(0,0, 0,               - CenterY + Indent_1, 3);
                        WriteBundlePosition(0,1, L,               CenterY - Indent_1,   1);
                        //2nd Row
                        WriteBundlePosition(0,2, MaxX,            - CenterY,            0);
                        WriteBundlePosition(0,3, MaxX - W,        L / 2,                2);
                        WriteBundlePosition(0,4, MaxX,            CenterY - L,          0);

                        //1st Row
                        WriteBundlePosition(1, 0, W,            - CenterY,              0);
                        WriteBundlePosition(1, 1, 0,              L / 2,                2);
                        WriteBundlePosition(1, 2, W,              CenterY - L,          0);
                        //2nd Row
                        WriteBundlePosition(1, 3, W + Gn + L,   - CenterY + Indent_1 + L, 1);
                        WriteBundlePosition(1, 4, W + Gn,         CenterY - Indent_1 - L, 3);
                    }
                    break;
                case 6:     //Passed
                    if (PatternOptionCode == "42")
                    {
                        MaxX = L + G + W;

                        if ((G == 0 && flag_a) || ((G != 0)&&((W + G + W) / 2 > (L + Gn + L + Gn + L + Gn + L) / 2))) // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + G + W;
                            // The inner Vertical Bundle inner edge must be at Horizontal Inner Rim
                            if (((G == 0) && (flag_c)) || ((G != 0) && (L + Gn + L < W)))
                            {
                                Indent_2 = W - L - Gn - L;
                                flag_c = true;
                            }
                            else
                            {
                                Indent_2 = 0;
                            }
                            flag_a = true;
                        }
                        else // Verified ver3.0 2023-05-10
                        {
                            MaxY = L + Gn + L + Gn + L + Gn + L;
                            // Horizontal Bundle must overlap inner Verical Bundle atleast MinHangingDist
                            if (((G == 0) && (flag_b)) || ((G != 0) && ((W - L - Gn) < MinHangingDist)))
                            {
                                Indent_1 = L + Gn + MinHangingDist - W;
                                flag_b = true;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                            if ((G != 0) && (Indent_1 + W + (G / 2) > L + Gn + L + (Gn / 2) || Indent_1 > (L / 3)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }
                        //if ((W + G + W) / 2 > (L + Gn + L + Gn + L + Gn + L) / 2) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W;
                            // The inner Vertical Bundle inner edge must be at Horizontal Inner Rim
                        //    Indent_2 = (L + Gn + L < W) ?
                        //        W - L -Gn - L:
                        //        0;
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = L + Gn + L + Gn + L + Gn + L;
                            // Horizontal Bundle must overlap inner Verical Bundle atleast MinHangingDist
                        //    Indent_1 = ((W - L - Gn) < MinHangingDist) ?
                        //        L + Gn + MinHangingDist - W :
                        //        0;
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                        //    if (Indent_1 + W + (G / 2) > L + Gn + L + (Gn / 2) || Indent_1 > (L / 3))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        CenterY = MaxY / 2;

                        //1st Group
                        WriteBundlePosition(0,0, 0,           - CenterY + Indent_1,                 3);
                        WriteBundlePosition(0,1, MaxX,        - CenterY,                            0);
                        WriteBundlePosition(0,2, MaxX - W,    - CenterY + L + Gn + Indent_2 + L,    2);
                        //2nd Group
                        WriteBundlePosition(0,3, W,             CenterY - L - Gn - Indent_2 - L,    0);
                        WriteBundlePosition(0,4, 0,             CenterY,                            2);
                        WriteBundlePosition(0,5, W + Gn + L,    CenterY - Indent_1,                 1);

                        //1st Group
                        WriteBundlePosition(1, 0, 0,            - CenterY + L,                  2);
                        WriteBundlePosition(1, 1, W,            - CenterY + L + Gn + Indent_2,  0);
                        WriteBundlePosition(1, 2, W + Gn + L,   - CenterY + Indent_1 + W,       1);
                        //2nd Group
                        WriteBundlePosition(1, 3, 0,              CenterY - Indent_1 - W,       3);
                        WriteBundlePosition(1, 4, L + G,          CenterY - L - Gn - Indent_2,  2);
                        WriteBundlePosition(1, 5, L + G + W,      CenterY - L,                  0);

                    }
                    break;
                case 7:     //Passed
                    if (PatternOptionCode == "34")
                    {
                        MaxX = L + Gn + L + G + W;
                        if (((G == 0) && (flag_a)) || ((G != 0) &&(W + G + W > L + Gn + L + Gn + L))) // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + G + W;
                            flag_a = true;
                        }
                        else // Verified ver3.0 2023-05-10
                        {
                            MaxY = L + Gn + L + Gn + L;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                            if (((G == 0) && (flag_b)) || ((G != 0) && ((W - L - Gn) < MinHangingDist)))
                            {
                                Indent_1 = L + Gn + MinHangingDist - W;
                                flag_b= true;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Pattern Centerline
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                            if ((G != 0) && (Indent_1 + W + (G / 2) > (MaxY / 2) || Indent_1 > (L / 3)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }

                        //if (W + G + W > L + Gn + L + Gn + L) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W;
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = L + Gn + L + Gn + L;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                        //    Indent_1 = ((W - L - Gn) < MinHangingDist) ?
                        //        L + Gn + MinHangingDist - W :
                        //        0;
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Pattern Centerline
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                        //    if (Indent_1 + W + (G / 2) > (MaxY / 2) || Indent_1 > (L / 3))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        CenterY = MaxY / 2;

                        //1st Row
                        WriteBundlePosition(0,0, 0,           - CenterY + Indent_1,   3);
                        WriteBundlePosition(0,1, L,             CenterY - Indent_1,   1);
                        //2nd Row
                        WriteBundlePosition(0,2, L + Gn,      - CenterY + Indent_1,   3);
                        WriteBundlePosition(0,3, L + Gn + L,    CenterY - Indent_1,   1);
                        //3rd Row
                        WriteBundlePosition(0,4, MaxX,        - CenterY,              0);
                        WriteBundlePosition(0,5, MaxX - W,      L / 2,                2);
                        WriteBundlePosition(0,6, MaxX,          CenterY - L,          0);

                        //1st Row
                        WriteBundlePosition(1, 0, 0,                    - CenterY + L,          2);
                        WriteBundlePosition(1, 1, W,                    - L / 2,                0);
                        WriteBundlePosition(1, 2, 0,                      CenterY,              2);
                        //2nd Row
                        WriteBundlePosition(1, 3, W + Gn,               - CenterY + Indent_1, 3);
                        WriteBundlePosition(1, 4, W + Gn + L,             CenterY - Indent_1, 1);
                        //3rd Row
                        WriteBundlePosition(1, 5, W + Gn + L + Gn,      - CenterY + Indent_1, 3);
                        WriteBundlePosition(1, 6, W + Gn + L + Gn + L,    CenterY - Indent_1, 1);

                    }
                    break;
                case 10:
                    if (PatternOptionCode == "43")
                    {
                        MaxX = L + G + W + G + W;
                        if (((G == 0) && (flag_a)) || ((G != 0) && ((W + G + W) / 2 > (L + Gn + L + Gn + L + Gn + L) / 2))) // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + G + W;
                            if (((G == 0) && (flag_c)) || ((G != 0) && (L + Gn + L < W)))
                            {
                                Indent_2 = W - L - Gn - L;
                                flag_c = true;
                            }
                            else
                            {
                                Indent_2 = 0;
                            }
                            flag_a = true;
                        }
                        else // Verified ver3.0 2023-05-10
                        {
                            MaxY = L + Gn + L + Gn + L + Gn + L;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                            if (((G == 0) && (flag_b)) || ((G != 0) && ((W - L - Gn) < MinHangingDist)))
                            {
                                Indent_1 = L + Gn + MinHangingDist - W;
                                flag_b = true;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                            if ((G != 0) && (Indent_1 + W + (G / 2) > (MaxY / 2) || Indent_1 > (L / 3)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }

                        //if ((W + G + W) / 2 > (L + Gn + L + Gn + L + Gn + L) / 2) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W;
                        //    Indent_2 = (L + Gn + L < W) ?
                        //        W - L - Gn - L :
                        //        0;
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = L + Gn + L + Gn + L + Gn + L;
                            // Horizontal Bundle must overlap middle Verical Bundle atleast MinHangingDist
                        //    Indent_1 = ((W - L - Gn) < MinHangingDist) ?
                        //        L + Gn + MinHangingDist - W :
                        //        0;
                            // If the Indent is
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  -   Greater than 1/3 of Bundle Width (not enough placing area for next layer Vertical Bundle)
                            //  This pattern is no allow
                        //    if (Indent_1 + W + (G / 2) > (MaxY / 2) || Indent_1 > (L / 3))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        CenterY = MaxY / 2;

                        //1st Row
                        WriteBundlePosition(0,0, L,           - CenterY + W + Indent_1,             1);
                        WriteBundlePosition(0,1, 0,           CenterY - W - Indent_1,               3);
                        //2nd Row
                        WriteBundlePosition(0,2, L + G + W,   - CenterY,                            0);
                        WriteBundlePosition(0,3, L + G,       - CenterY + L + Gn + Indent_2 + L,    2);
                        WriteBundlePosition(0,4, L + G + W,     CenterY - L - Gn - Indent_2 - L,    0);
                        WriteBundlePosition(0,5, L + G,         CenterY ,                           2);
                        //3rd Row
                        WriteBundlePosition(0,6, MaxX,        - CenterY,                            0);
                        WriteBundlePosition(0,7, MaxX - W,    - CenterY + L + Gn + Indent_2 + L,    2);
                        WriteBundlePosition(0,8, MaxX,          CenterY - L - Gn - Indent_2 - L,    0);
                        WriteBundlePosition(0,9, MaxX - W,      CenterY ,                           2);

                        //1st Row
                        WriteBundlePosition(1,0, W,                     - CenterY,                          0);
                        WriteBundlePosition(1,1, 0,                     - CenterY + L + Gn + Indent_2 + L,  2);
                        WriteBundlePosition(1,2, W,                       CenterY - L - Gn - Indent_2 - L,  0);
                        WriteBundlePosition(1,3, 0,                       CenterY,                          2);
                        //2nd Row
                        WriteBundlePosition(1,4, W + G + W,             - CenterY,                          0);
                        WriteBundlePosition(1,5, W + G,                 - CenterY + L + Gn + Indent_2 + L,  2);
                        WriteBundlePosition(1,6, W + G + W,               CenterY - L - Gn - Indent_2 - L,  0);
                        WriteBundlePosition(1,7, W + G,                   CenterY,                          2);
                        //3rd Row
                        WriteBundlePosition(1,8, W + G + W + Gn + L,    - CenterY + W + Indent_1,           1);
                        WriteBundlePosition(1,9, W + G + W + Gn,          CenterY - W - Indent_1,           3);

                    }
                    break;
                case 11:
                    if (PatternOptionCode == "34")
                    {
                        MaxX = L + G + W + G + W;
                        if (((G == 0) && (flag_a)) || ((G != 0) && (W + G + W + G + W > L + Gn + L + Gn + L + Gn + L))) // Verified ver3.0 2023-05-10
                        {
                            MaxY = W + G + W + G + W;
                            // 2nd Vertical Bundle must overlap middle Horizontal Bundle atleast MinHangingDist
                            if (((G == 0) && (flag_b)) || ((G != 0) && (L + Gn + MinHangingDist > W + G + W)))
                            {
                                Indent_1 = W + G + MinHangingDist - L - Gn - L;
                            }
                            else
                            {
                                Indent_1 = 0;
                            }
                            flag_a = true;
                            // If the Indent is
                            //  -   2nd Vertical Bundle Overlapping Rim Horizontal Bundle less than MinHangingDist
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  This pattern is no allow
                            if ((G != 0) && (L + Gn + Indent_1 > L - MinHangingDist || L + Gn + Indent_1 + L + (Gn / 2) > (MaxY / 2)))
                            {
                                ErrorFounded = true;
                                break;
                            }
                        }
                        else // Verified ver3.0 2023-05-10
                        {
                            MaxY = L + Gn + L + Gn + L + Gn + L;
                        }


                        //if (W + G + W + G + W > L + Gn + L + Gn + L + Gn + L) // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = W + G + W + G + W;
                            // 2nd Vertical Bundle must overlap middle Horizontal Bundle atleast MinHangingDist
                        //    Indent_1 = (L + Gn + MinHangingDist > W + G + W) ?
                        //        W + G + MinHangingDist - L - Gn - L :
                        //        0;
                            // If the Indent is
                            //  -   2nd Vertical Bundle Overlapping Rim Horizontal Bundle less than MinHangingDist
                            //  -   Making the Horizontal Bundle exceed Group Edge (Inside edge of inner Vertical Bundle of the group)
                            //  This pattern is no allow
                        //    if (L + Gn + Indent_1 > L - MinHangingDist || L + Gn + Indent_1 + L + (Gn / 2) > (MaxY / 2))
                        //    {
                        //        ErrorFounded = true;
                        //        break;
                        //    }
                        //}
                        //else // Verified ver3.0 2023-05-10
                        //{
                        //    MaxY = L + Gn + L + Gn + L + Gn + L;
                        //}
                        CenterY = MaxY / 2;

                        //1st Row
                        WriteBundlePosition(0,0,  L,          - CenterY + W,                      1);
                        WriteBundlePosition(0,1,  0,          - (W / 2),                          3);
                        WriteBundlePosition(0,2,  0,            CenterY - W,                      3);
                        //2nd Row
                        WriteBundlePosition(0,3,  L + G + W,  - CenterY,                          0);
                        WriteBundlePosition(0,4,  L + G,      - CenterY + L + Gn + Indent_1 + L,  2);
                        WriteBundlePosition(0,5,  L + G + W,  CenterY - L - Gn - Indent_1 - L,    0);
                        WriteBundlePosition(0,6,  L + G,      CenterY,                            2);
                        //3rd Row
                        WriteBundlePosition(0,7,  MaxX,       - CenterY, 0);
                        WriteBundlePosition(0,8,  MaxX - W,   - CenterY + L + Gn + Indent_1 + L,  2);
                        WriteBundlePosition(0,9,  MaxX,       CenterY - L - Gn - Indent_1 - L,    0);
                        WriteBundlePosition(0,10, MaxX - W,   CenterY,                            2);

                        //1st Row
                        WriteBundlePosition(1, 0,   W,          - CenterY,                          0);
                        WriteBundlePosition(1, 1,   0,          - CenterY + L + Gn + Indent_1 + L,  2);
                        WriteBundlePosition(1, 2,   W,            CenterY - L - Gn - Indent_1 - L,  0);
                        WriteBundlePosition(1, 3,   0,            CenterY,                          2);
                        //2nd Row
                        WriteBundlePosition(1, 4,   W + G + W,  - CenterY,                          0);
                        WriteBundlePosition(1, 5,   W + G,      - CenterY + L + Gn + Indent_1 + L,  2);
                        WriteBundlePosition(1, 6,   W + G + W,    CenterY - L - Gn - Indent_1 - L,  0);
                        WriteBundlePosition(1, 7,   W + G,        CenterY,                          2);
                        //3rd Row
                        WriteBundlePosition(1, 8,   W + G + W + Gn + L,       - CenterY + W,                      1);
                        WriteBundlePosition(1, 9,   W + G + W + Gn + L,         W / 2,                            1);
                        WriteBundlePosition(1, 10,  W + G + W + Gn,     CenterY - W,                      3);
                    }
                    break;
                default:
                    ErrorFounded = true;
                    break;
            }
            SourcePatternDimension.MaxX = MaxX;
            SourcePatternDimension.MaxY = MaxY;
            return ErrorFounded;
        }
        //===========================================================================
        private bool C_PatternGenerator_NormOri(int Gap, bool FixFace)
        {
            bool ErrorFounded = false;
            int W = BundleWidth;
            int L = BundleLength;
            int G = Gap;
            int Gn = Gap / 2;
            int CenterY;
            int x;
            int y;
            int maxRow;
            int maxCol;
            int Row = 0;
            int Col = 0;
            int MaxX = 0;
            int MaxY = 0;
            List<int> NonFixBundle = new List<int>();
            PatternType = "C";
            switch (BundlePerLayer)
            {
                case 1: // Verified ver3.0 2023-05-12
                    MaxX = W;
                    MaxY = L;
                    CenterY = MaxY / 2;
                    WriteBundlePosition(0,0, W, -CenterY, 0);
                    break;
                case 2:
                    if (PatternOptionCode == "21") // Verified ver3.0 2023-05-12
                    {
                        MaxX = L;
                        MaxY = W + G + W;
                        CenterY = MaxY / 2;
                        NonFixBundle = new List<int> { 0 };
                        for (int i = 0; i < BundlePerLayer; i++)
                        {
                            x = 0;
                            y = -CenterY + (i * (W + G));
                            if (!FixFace && NonFixBundle.Contains(i))
                            {
                                x = x + L;
                                y = y + W;
                                WriteBundlePosition(0,i, x, y, 1);
                            }
                            else
                                WriteBundlePosition(0,i, x, y, 3);
                        }
                    }
                    break;
                case 3:
                    switch (PatternOptionCode)
                    {
                        case "31": // Verified ver3.0 2023-05-17
                            MaxX = L;
                            MaxY = W + G + W + G + W;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 0, 2 };
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                x = 0;
                                y = - CenterY + (i * (W + G));
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x + L;
                                    y = y + W;
                                    WriteBundlePosition(0,i, x, y, 1);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 3);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 4:     //Passed
                    switch (PatternOptionCode)
                    {
                        case "22": // Verified ver3.0 2023-05-12
                            MaxX = W + G + W;
                            MaxY = L + Gn + L;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 1, 3 };
                            maxRow = 2;
                            maxCol = 2;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0)?
                                    Row + 1:
                                    Row;
                                x = W + (Row * (W + G));
                                y = -CenterY + (L + Gn) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x - W;
                                    y = y + L;
                                    WriteBundlePosition(0,i, x, y, 2);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 0);
                            }
                            break;
                        case "41": // Verified ver3.0 2023-05-12
                            MaxX = L;
                            MaxY = W + G + W + G + W + G + W;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 0, 2 };
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                x = 0;
                                y = -CenterY + (i * (W + G));
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x + L;
                                    y = y + W;
                                    WriteBundlePosition(0,i, x, y, 1);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 3);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 6:     //Passed
                    switch (PatternOptionCode)
                    {
                        case "32": // Verified ver3.0 2023-05-12
                            MaxX = W + G + W + G + W;
                            MaxY = L + Gn + L;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 1, 3, 5 };
                            maxRow = 3;
                            maxCol = 2;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = W + (Row * (W + G));
                                y = -CenterY + (L + Gn) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x - W;
                                    y = y + L;
                                    WriteBundlePosition(0,i, x, y, 2);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 0);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 8:
                    switch (PatternOptionCode)
                    {
                        case "24": // Verified ver3.0 2023-05-12
                            MaxX = W + G + W + G + W + G + W;
                            MaxY = L + Gn + L;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 1, 3, 5, 7 };
                            maxRow = 4;
                            maxCol = 2;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = W + (Row * (W + G));
                                y = -CenterY + (L + Gn) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x - W;
                                    y = y + L;
                                    WriteBundlePosition(0,i, x, y, 2);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 0);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 9:
                    switch (PatternOptionCode)
                    {
                        case "33": // Verified ver3.0 2023-05-12
                            MaxX = W + G + W + G + W;
                            MaxY = L + Gn + L + Gn + L;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 2, 3, 4, 7, 8 };
                            maxRow = 3;
                            maxCol = 3;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = W + (Row * (W + G));
                                y = -CenterY + (L + Gn) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x - W;
                                    y = y + L;
                                    WriteBundlePosition(0,i, x, y, 2);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 0);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 12:
                    switch (PatternOptionCode)
                    {
                        case "43": // Verified ver3.0 2023-05-12
                            MaxX = W + G + W + G + W;
                            MaxY = L + Gn + L + Gn + L + Gn + L;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 2, 3, 4, 5, 10, 11 };
                            maxRow = 3;
                            maxCol = 4;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = W + (Row * (W + G));
                                y = -CenterY + (L + Gn) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x - W;
                                    y = y + L;
                                    WriteBundlePosition(0,i, x, y, 2);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 0);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                default:
                    ErrorFounded = true;
                    break;
            }
            SourcePatternDimension.MaxX = MaxX;
            SourcePatternDimension.MaxY = MaxY;
            return ErrorFounded;
        }

        //[04-07-2020]Support มัดกล่องในแนวขวางลอน  @Beer
        private bool C_PatternGenerator_SwapOri(int Gap, bool FixFace)
        {
            bool ErrorFounded = false;
            int W = BundleWidth;
            int L = BundleLength;
            int G = Gap;
            int Gn = Gap / 2;
            int CenterY;
            int x;
            int y;
            int maxRow;
            int maxCol;
            int Row = 0;
            int Col = 0;
            int MaxX = 0;
            int MaxY = 0;
            List<int> NonFixBundle = new List<int>();
            PatternType = "C";
            switch (BundlePerLayer)
            {
                case 1: // Verified ver3.0 2023-05-12
                    MaxX = L;
                    MaxY = W;
                    CenterY = MaxY / 2;
                    WriteBundlePosition(0,0, 0, -CenterY, 3);
                    break;
                case 2:
                    switch (PatternOptionCode)
                    {
                        case "21": // Verified ver3.0 2023-05-12
                            MaxX = W;
                            MaxY = L + Gn + L;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 0 };
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                x = W;
                                y = -CenterY + (i * (L + Gn));
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x - W;
                                    y = y + L;
                                    WriteBundlePosition(0,i, x, y, 2);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 0);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 3:
                    switch (PatternOptionCode)
                    {
                        case "31": // Verified ver3.0 2023-05-12
                            MaxX = W;
                            MaxY = L + Gn + L + Gn + L;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 0, 2 };
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                x = W;
                                y = -CenterY + (i * (L + Gn));
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x - W;
                                    y = y + L;
                                    WriteBundlePosition(0,i, x, y, 2);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 0);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 4:
                    switch (PatternOptionCode)
                    {
                        case "22": // Verified ver3.0 2023-05-12
                            MaxX = L + Gn + L;
                            MaxY = W + G + W;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 1, 3 };
                            maxRow = 2;
                            maxCol = 2;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = Row * (L + Gn);
                                y = -CenterY + (W + G) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x + L;
                                    y = y + W;
                                    WriteBundlePosition(0,i, x, y, 1);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 3);
                            }
                            break;
                        case "41": // Verified ver3.0 2023-05-12
                            MaxX = W;
                            MaxY = L + Gn + L + Gn + L + Gn + L;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 0, 2 };
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                x = W;
                                y = -CenterY + (i * (L + Gn));
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x - W;
                                    y = y + L;
                                    WriteBundlePosition(0,i, x, y, 2);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 0);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 6:
                    switch (PatternOptionCode)
                    {
                        case "32": // Verified ver3.0 2023-05-12
                            MaxX = L + Gn + L + Gn + L;
                            MaxY = W + G + W;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 1, 3, 5 };
                            maxRow = 3;
                            maxCol = 2;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = Row * (L + Gn);
                                y = -CenterY + (W + G) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x + L;
                                    y = y + W;
                                    WriteBundlePosition(0,i, x, y, 1);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 3);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 8:
                    switch (PatternOptionCode)
                    {
                        case "24": // Verified ver3.0 2023-05-12
                            MaxX = L + Gn + L + Gn + L + Gn + L;
                            MaxY = W + G + W;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 1, 3, 5, 7 };
                            maxRow = 4;
                            maxCol = 2;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = Row * (L + Gn);
                                y = -CenterY + (W + G) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x + L;
                                    y = y + W;
                                    WriteBundlePosition(0,i, x, y, 1);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 3);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 9:
                    switch (PatternOptionCode)
                    {
                        case "33": // Verified ver3.0 2023-05-12
                            MaxX = L + Gn + L + Gn + L;
                            MaxY = W + G + W + G + W;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 2, 3, 4, 7, 8 };
                            maxRow = 3;
                            maxCol = 3;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = Row * (L + Gn);
                                y = -CenterY + (W + G) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x + L;
                                    y = y + W;
                                    WriteBundlePosition(0,i, x, y, 1);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 3);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 12:
                    switch (PatternOptionCode)
                    {
                        case "43": // Verified ver3.0 2023-05-12
                            MaxX = L + Gn + L + Gn + L;
                            MaxY = W + G + W + G + W + G + W;
                            CenterY = MaxY / 2;
                            NonFixBundle = new List<int> { 2, 3, 4, 5, 10, 11 };
                            maxRow = 3;
                            maxCol = 4;
                            for (int i = 0; i < BundlePerLayer; i++)
                            {
                                Row = (i % maxCol == 0 && i != 0) ?
                                    Row + 1 :
                                    Row;
                                x = Row * (L + Gn);
                                y = -CenterY + (W + G) * (i % maxCol);
                                if (!FixFace && NonFixBundle.Contains(i))
                                {
                                    x = x + L;
                                    y = y + W;
                                    WriteBundlePosition(0,i, x, y, 1);
                                }
                                else
                                    WriteBundlePosition(0,i, x, y, 3);
                            }
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                default:
                    ErrorFounded = true;
                    break;
            }
            SourcePatternDimension.MaxX = MaxX;
            SourcePatternDimension.MaxY = MaxY;
            return ErrorFounded;
        }
        //===========================================================================
        private bool S_PatternGenerator_NormOri(int Gap)
        {
            bool ErrorFounded = false;
            int W = BundleWidth;
            int L = BundleLength;
            int G = Gap;
            int Gn = Gap / 2;
            int CenterY;
            int MaxX = 0;
            int MaxY = 0;
            int[,,] subBDGroup = new int[2, 3, 3];
            int b2 = 1;
            int b3 = 2;
            int b4 = 3;
            int b5 = 4;
            int b6 = 5;
            int b7 = 6;
            int b8 = 7;
            int b9 = 8;
            PatternType = "S";
            switch (BundlePerLayer)
            {
                case 4:
                    switch (PatternOptionCode)
                    {
                        case "22": // Verified ver3.0 2023-05-15
                            MaxY = W + L + G;
                            MaxX = W + L + G;
                            CenterY = MaxY / 2;
                                if (L < W)
                                {
                                    b2 = 2;
                                    b3 = 1;
                                }
                                WriteBundlePosition(0,0, W, -CenterY, 0);
                                WriteBundlePosition(0,b2, W + Gn + L, -CenterY + W, 1);
                                WriteBundlePosition(0,b3, 0, CenterY - W, 3);
                                WriteBundlePosition(0,3, MaxX - W, -CenterY + W + Gn + L, 2);

                                WriteBundlePosition(1,0, L, -CenterY + W, 1);
                                WriteBundlePosition(1,b2, W, -CenterY + W + Gn, 0);
                                WriteBundlePosition(1,b3, L + G, -CenterY + L, 2);
                                WriteBundlePosition(1,3, W + Gn, -CenterY + L + G, 3);
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 8:
                    switch (PatternOptionCode)
                    {
                        case "44": // Verified ver3.0 2023-05-12
                            MaxX = 2 * (W + G) + L;
                            MaxY = 2 * (W + G) + L;
                            CenterY = MaxY / 2;

                            if (L > W + G + W)
                            {
                                b3 = 4;
                                b4 = 5;
                                b5 = 2;
                                b6 = 3;
                            }
                            subBDGroup = S_PatternGenerator_NormOri_SubBD(G, Gn, W, L, 2);

                            //1st layer
                            //bottom left corner
                            //1st row
                            WriteBundlePosition(0,0, subBDGroup[0, 0, 0], -CenterY + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(0,1, subBDGroup[0, 1, 0], -CenterY + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);

                            //bottom right corner
                            //1st column
                            WriteBundlePosition(0,b3, subBDGroup[1, 0, 0], -CenterY + L + G + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(0,b4, subBDGroup[1, 1, 0], -CenterY + L + G + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);

                            //top left corner
                            //1st column
                            WriteBundlePosition(0,b5, W + G + W + Gn + subBDGroup[1, 0, 0], -CenterY + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(0,b6, W + G + W + Gn + subBDGroup[1, 1, 0], -CenterY + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);

                            //top right corner
                            WriteBundlePosition(0,6, L + G + subBDGroup[0, 0, 0], -CenterY + W + G + W + Gn + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(0,7, L + G + subBDGroup[0, 1, 0], -CenterY + W + G + W + Gn + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);

                            //2nd layer
                            //bottom left corner
                            //1st row
                            WriteBundlePosition(1, 0, subBDGroup[1, 0, 0], -CenterY + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(1, 1, subBDGroup[1, 1, 0], -CenterY + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);

                            //top left corner
                            WriteBundlePosition(1, b3, L + G + subBDGroup[0, 0, 0], -CenterY + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(1, b4, L + G + subBDGroup[0, 1, 0], -CenterY + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);

                            //bottom right corner
                            WriteBundlePosition(1, b5, subBDGroup[0, 0, 0], -CenterY + W + G + W + Gn + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(1, b6, subBDGroup[0, 1, 0], -CenterY + W + G + W + Gn + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);

                            //top right corner
                            WriteBundlePosition(1, 6, W + G + W + Gn + subBDGroup[1, 0, 0], -CenterY + L + G + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(1, 7, W + G + W + Gn + subBDGroup[1, 1, 0], -CenterY + L + G + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 12:
                    switch (PatternOptionCode)
                    {
                        case "66": // Verified ver3.0 2023-05-12
                            if (L > W + G + W + G + W)
                            {
                                b4 = 6;
                                b5 = 7;
                                b6 = 8;
                                b7 = 3;
                                b8 = 4;
                                b9 = 5;
                            }
                            MaxY = (3 * W) + L + (3 * G);
                            MaxX = (3 * W) + L + (3 * G);
                            CenterY = MaxY / 2;
                            subBDGroup = S_PatternGenerator_NormOri_SubBD(G,Gn,W,L,3);

                            //bottom left corner
                            //1st row
                            WriteBundlePosition(0,0, subBDGroup[0, 0, 0], -CenterY + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(0,1, subBDGroup[0, 1, 0], -CenterY + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);
                            WriteBundlePosition(0,2, subBDGroup[0, 2, 0], -CenterY + subBDGroup[0, 2, 1], subBDGroup[0, 2, 2]);

                            //bottom right corner
                            //1st column
                            WriteBundlePosition(0,b4, subBDGroup[1, 0, 0], -CenterY + L + G + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(0,b5, subBDGroup[1, 1, 0], -CenterY + L + G + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            WriteBundlePosition(0,b6, subBDGroup[1, 2, 0], -CenterY + L + G + subBDGroup[1, 2, 1], subBDGroup[1, 2, 2]);

                            //top left corner
                            //1st column
                            WriteBundlePosition(0,b7, W + G + W + G + W + Gn + subBDGroup[1, 0, 0], -CenterY + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(0,b8, W + G + W + G + W + Gn + subBDGroup[1, 1, 0], -CenterY + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            WriteBundlePosition(0,b9, W + G + W + G + W + Gn + subBDGroup[1, 2, 0], -CenterY + subBDGroup[1, 2, 1], subBDGroup[1, 2, 2]);

                            //top right corner
                            WriteBundlePosition(0,9, L + G + subBDGroup[0, 0, 0], -CenterY + W + G + W + G + W + Gn + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(0,10, L + G + subBDGroup[0, 1, 0], -CenterY + W + G + W + G + W + Gn + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);
                            WriteBundlePosition(0,11, L + G + subBDGroup[0, 2, 0], -CenterY + W + G + W + G + W + Gn + subBDGroup[0, 2, 1], subBDGroup[0, 2, 2]);

                            //2nd layer
                            //bottom left corner
                            //1st row
                            WriteBundlePosition(1, 0, subBDGroup[1, 0, 0], -CenterY + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(1, 1, subBDGroup[1, 1, 0], -CenterY + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            WriteBundlePosition(1, 2, subBDGroup[1, 2, 0], -CenterY + subBDGroup[1, 2, 1], subBDGroup[1, 2, 2]);

                            //top left corner
                            WriteBundlePosition(1, b4, L + G + subBDGroup[0, 0, 0], -CenterY + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(1, b5, L + G + subBDGroup[0, 1, 0], -CenterY + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);
                            WriteBundlePosition(1, b6, L + G + subBDGroup[0, 2, 0], -CenterY + subBDGroup[0, 2, 1], subBDGroup[0, 2, 2]);

                            //bottom right corner
                            WriteBundlePosition(1, b7, subBDGroup[0, 0, 0], -CenterY + W + G + W + G + W + Gn + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(1, b8, subBDGroup[0, 1, 0], -CenterY + W + G + W + G + W + Gn + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);
                            WriteBundlePosition(1, b9, subBDGroup[0, 2, 0], -CenterY + W + G + W + G + W + Gn + subBDGroup[0, 2, 1], subBDGroup[0, 2, 2]);

                            //top right corner
                            WriteBundlePosition(1, 9, W + G + W + G + W + Gn + subBDGroup[1, 0, 0], -CenterY + L + G + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(1, 10, W + G + W + G + W + Gn + subBDGroup[1, 1, 0], -CenterY + L + G + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            WriteBundlePosition(1, 11, W + G + W + G + W + Gn + subBDGroup[1, 2, 0], -CenterY + L + G + subBDGroup[1, 2, 1], subBDGroup[1, 2, 2]);
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                default:
                    ErrorFounded = true;
                    break;
            }
            SourcePatternDimension.MaxX = MaxX;
            SourcePatternDimension.MaxY = MaxY;
            return ErrorFounded;
        }

        //[04-07-2020]Support มัดกล่องในแนวขวางลอน  @Beer
        private bool S_PatternGenerator_SwapOri(int Gap)
        {
            bool ErrorFounded = false;
            int W = BundleWidth;
            int L = BundleLength;
            int G = Gap;
            int Gn = Gap / 2;
            int CenterY;
            int MaxX = 0;
            int MaxY = 0;
            int[,,] subBDGroup = new int[2, 3, 3];
            int b2 = 1;
            int b3 = 2;
            int b4 = 3;
            int b5 = 4;
            int b6 = 5;
            int b7 = 6;
            int b8 = 7;
            int b9 = 8;
            PatternType = "S";
            switch (BundlePerLayer)
            {
                case 4:
                    switch (PatternOptionCode)
                    {
                        case "22": // Verified ver3.0 2023-05-15
                            MaxY = W + L + G;
                            MaxX = W + L + G;
                            CenterY = MaxY / 2;
                            if (L > W)
                            {
                                b2 = 2;
                                b3 = 1;
                            }
                            WriteBundlePosition(0,0, 0, -CenterY, 3);
                            WriteBundlePosition(0,b2, L + G + W, -CenterY, 0);
                            WriteBundlePosition(0,b3, 0, -CenterY + W + Gn + L, 2);
                            WriteBundlePosition(0,3, W + G + L, -CenterY + L + G + W, 1);

                            WriteBundlePosition(1, 0, W, -CenterY, 0);
                            WriteBundlePosition(1, b2, 0, -CenterY + L + G, 3);
                            WriteBundlePosition(1, b3, W + Gn + L, -CenterY + W, 1);
                            WriteBundlePosition(1, 3, L + G, -CenterY + W + Gn + L, 2);
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 8:
                    switch (PatternOptionCode)
                    {
                        case "44": // Verified ver3.0 2023-05-12
                            if (W > L + Gn + L)
                            {
                                b3 = 4;
                                b4 = 5;
                                b5 = 2;
                                b6 = 3;
                            }
                            MaxY = (2 * L) + Gn + W + G;
                            MaxX = (2 * L) + Gn + W + G;
                            CenterY = MaxY / 2;

                            subBDGroup = S_PatternGenerator_SwapOri_SubBD(G, Gn, W, L, 2);

                            //bottom left corner
                            //1st row
                            WriteBundlePosition(0,0, subBDGroup[0, 0, 0], -CenterY + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(0,1, subBDGroup[0, 1, 0], -CenterY + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);

                            //bottom right corner
                            //1st column
                            WriteBundlePosition(0,b3, subBDGroup[1, 0, 0], -CenterY + W + Gn + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(0,b4, subBDGroup[1, 1, 0], -CenterY + W + Gn + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);

                            //top left corner
                            //1st column
                            WriteBundlePosition(0,b5, L + Gn + L + G + subBDGroup[1, 0, 0], -CenterY + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(0,b6, L + Gn + L + G + subBDGroup[1, 1, 0], -CenterY + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);

                            //top right corner
                            WriteBundlePosition(0,6, W + Gn + subBDGroup[0, 0, 0], -CenterY + L + Gn + L + G + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(0,7, W + Gn + subBDGroup[0, 1, 0], -CenterY + L + Gn + L + G + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);

                            //bottom left corner
                            //1st column
                            WriteBundlePosition(1, 0, subBDGroup[1, 0, 0], -CenterY + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(1, 1, subBDGroup[1, 1, 0], -CenterY + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);

                            //bottom right corner
                            //1st row
                            WriteBundlePosition(1, b3, W + Gn + subBDGroup[0, 0, 0], -CenterY + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(1, b4, W + Gn + subBDGroup[0, 1, 0], -CenterY + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);

                            //top left corner
                            //1st row
                            WriteBundlePosition(1, b5, subBDGroup[0, 0, 0], -CenterY + L + Gn + L + G + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(1, b6, subBDGroup[0, 1, 0], -CenterY + L + Gn + L + G + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);

                            //top right corner
                            WriteBundlePosition(1, 6, L + Gn + L + G + subBDGroup[1, 0, 0], -CenterY + W + Gn + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(1, 7, L + Gn + L + G + subBDGroup[1, 1, 0], -CenterY + W + Gn + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                case 12:
                    switch (PatternOptionCode)
                    {
                        case "66": // Verified ver3.0 2023-05-12
                            if (W > L + Gn + L + Gn + L)
                            {
                                b4 = 6;
                                b5 = 7;
                                b6 = 8;
                                b7 = 3;
                                b8 = 4;
                                b9 = 5;
                            }
                            MaxY = (3 * L) + (2 * Gn) + G + W;
                            MaxX = (3 * L) + (2 * Gn) + G + W;
                            CenterY = MaxY / 2;
                            subBDGroup = S_PatternGenerator_SwapOri_SubBD(G, Gn, W, L, 3);

                            //bottom left corner
                            //1st row
                            WriteBundlePosition(0,0, subBDGroup[0, 0, 0], -CenterY + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(0,1, subBDGroup[0, 1, 0], -CenterY + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);
                            WriteBundlePosition(0,2, subBDGroup[0, 2, 0], -CenterY + subBDGroup[0, 2, 1], subBDGroup[0, 2, 2]);

                            //bottom right corner
                            //1st column
                            WriteBundlePosition(0,b4, subBDGroup[1, 0, 0], -CenterY + W + Gn + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(0,b5, subBDGroup[1, 1, 0], -CenterY + W + Gn + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            WriteBundlePosition(0,b6, subBDGroup[1, 2, 0], -CenterY + W + Gn + subBDGroup[1, 2, 1], subBDGroup[1, 2, 2]);

                            //top left corner
                            //1st column
                            WriteBundlePosition(0,b7, L + Gn + L + Gn + L + G + subBDGroup[1, 0, 0], -CenterY + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(0,b8, L + Gn + L + Gn + L + G + subBDGroup[1, 1, 0], -CenterY + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            WriteBundlePosition(0,b9, L + Gn + L + Gn + L + G + subBDGroup[1, 2, 0], -CenterY + subBDGroup[1, 2, 1], subBDGroup[1, 2, 2]);

                            //top right corner
                            WriteBundlePosition(0,9, W + G + subBDGroup[0, 0, 0], -CenterY + L + Gn + L + Gn + L + G + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(0,10, W + G + subBDGroup[0, 1, 0], -CenterY + L + Gn + L + Gn + L + G + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);
                            WriteBundlePosition(0,11, W + G + subBDGroup[0, 2, 0], -CenterY + L + Gn + L + Gn + L + G + subBDGroup[0, 2, 1], subBDGroup[0, 2, 2]);


                            //bottom left corner
                            //1st column
                            WriteBundlePosition(1, 0, subBDGroup[1, 0, 0], -CenterY + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(1, 1, subBDGroup[1, 1, 0], -CenterY + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            WriteBundlePosition(1, 2, subBDGroup[1, 2, 0], -CenterY + subBDGroup[1, 2, 1], subBDGroup[1, 2, 2]);

                            //bottom right corner
                            //1st row
                            WriteBundlePosition(1, b4, W + Gn + subBDGroup[0, 0, 0], -CenterY + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(1, b5, W + Gn + subBDGroup[0, 1, 0], -CenterY + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);
                            WriteBundlePosition(1, b6, W + Gn + subBDGroup[0, 2, 0], -CenterY + subBDGroup[0, 2, 1], subBDGroup[0, 2, 2]);

                            //top left corner
                            //1st row
                            WriteBundlePosition(1, b7, subBDGroup[0, 0, 0], -CenterY + L + Gn + L + Gn + L + G + subBDGroup[0, 0, 1], subBDGroup[0, 0, 2]);
                            WriteBundlePosition(1, b8, subBDGroup[0, 1, 0], -CenterY + L + Gn + L + Gn + L + G + subBDGroup[0, 1, 1], subBDGroup[0, 1, 2]);
                            WriteBundlePosition(1, b9, subBDGroup[0, 2, 0], -CenterY + L + Gn + L + Gn + L + G + subBDGroup[0, 2, 1], subBDGroup[0, 2, 2]);

                            //top right corner
                            WriteBundlePosition(1, 9, L + Gn + L + Gn + L + G + subBDGroup[1, 0, 0], -CenterY + W + Gn + subBDGroup[1, 0, 1], subBDGroup[1, 0, 2]);
                            WriteBundlePosition(1, 10, L + Gn + L + Gn + L + G + subBDGroup[1, 1, 0], -CenterY + W + Gn + subBDGroup[1, 1, 1], subBDGroup[1, 1, 2]);
                            WriteBundlePosition(1, 11, L + Gn + L + Gn + L + G + subBDGroup[1, 2, 0], -CenterY + W + Gn + subBDGroup[1, 2, 1], subBDGroup[1, 2, 2]);
                            break;
                        default:
                            ErrorFounded = true;
                            break;
                    }
                    break;
                default:
                    ErrorFounded = true;
                    break;
            }
            SourcePatternDimension.MaxX = MaxX;
            SourcePatternDimension.MaxY = MaxY;
            return ErrorFounded;
        }

        private int[,,] S_PatternGenerator_NormOri_SubBD(int G, int Gn, int W, int L, int subBD)
        {
            int[,,] subBDGroup = new int[2, 3, 3];
            // Vertical Group
            for (int i = 0; i < subBD; i++)
            {
                subBDGroup[0, i, 2] = (i % 2 == 0) ?
                    0 :
                    2; // ori
                subBDGroup[0, i, 0] = i * (W + G); // x
                subBDGroup[0, i, 1] = L; // y
                if (subBDGroup[0, i, 2] == 0)
                {
                    subBDGroup[0, i, 0] = W + subBDGroup[0, i, 0]; // x
                    subBDGroup[0, i, 1] = subBDGroup[0, i, 1] - L;// y
                }
            }
            // Horizontal Group
            for (int i = 0; i < subBD; i++)
            {
                subBDGroup[1, i, 2] = (i % 2 == 0) ? // ori
                    1 :
                    3;
                subBDGroup[1, i, 0] = 0; // x
                subBDGroup[1, i, 1] = i * (W + G); // y
                if (subBDGroup[1, i, 2] == 1)
                {
                    subBDGroup[1, i, 0] = subBDGroup[1, i, 0] + L; // x
                    subBDGroup[1, i, 1] = W + subBDGroup[1, i, 1];// y
                }
            }
            return subBDGroup;
        }
        private int[,,] S_PatternGenerator_SwapOri_SubBD(int G, int Gn, int W, int L, int subBD)
        {
            int[,,] subBDGroup = new int[2, 3, 3];
            // Horizontal Group
            for (int i = 0; i < subBD; i++)
            {
                subBDGroup[1, i, 2] = (i % 2 == 0) ?
                    0 :
                    2; // ori
                subBDGroup[1, i, 0] = W; // x
                subBDGroup[1, i, 1] = i * (L + Gn); // y
                if (subBDGroup[1, i, 2] == 2)
                {
                    subBDGroup[1, i, 0] = subBDGroup[1, i, 0] - W; // x
                    subBDGroup[1, i, 1] = subBDGroup[1, i, 1] + L ;// y
                }
            }
            // Vertical Group
            for (int i = 0; i < subBD; i++)
            {
                subBDGroup[0, i, 2] = (i % 2 == 0) ? // ori
                    3 :
                    1;
                subBDGroup[0, i, 0] = i * (L + Gn); // x
                subBDGroup[0, i, 1] = 0; // y
                if (subBDGroup[0, i, 2] == 1)
                {
                    subBDGroup[0, i, 0] = subBDGroup[0, i, 0]+ L; // x
                    subBDGroup[0, i, 1] = subBDGroup[0, i, 1] + W;// y
                }
            }
            return subBDGroup;
        }

        //===========================================================================
        private Dimension Get_PatternDimension(Pattern[] PatternArray)
        {
            Dimension Result;
            int i;
            int MostRight = 0;
            int MostTop = 0;
            int L = BundleLength;
            int W = BundleWidth;
            Result.MaxX = 0;
            Result.MaxY = 0;

            for (i = 0; i < BundlePerLayer; i++)
            {
                switch (PatternArray[i].ori)
                {
                    case 0: MostRight = PatternArray[i].y + L; break;
                    case 1: MostRight = PatternArray[i].y; break;
                    case 2: MostRight = PatternArray[i].y; break;
                    case 3: MostRight = PatternArray[i].y + W; break;
                }
                if (MostRight > Result.MaxY) Result.MaxY = MostRight;
            }
            for (i = 0; i < BundlePerLayer; i++)
            {
                switch (PatternArray[i].ori)
                {
                    case 0: MostTop = PatternArray[i].x; break;
                    case 1: MostTop = PatternArray[i].x; break;
                    case 2: MostTop = PatternArray[i].x + W; break;
                    case 3: MostTop = PatternArray[i].x + L; break;
                }
                if (MostTop > Result.MaxX) Result.MaxX = MostTop;
            }
            Result.MaxY = Result.MaxY * 2;

            //if (SwapSide == false)
            //{
            //    for (i = 0; i < BundlePerLayer; i++)
            //    {
            //        switch (PatternArray[i].ori)
            //        {
            //            case 0: MostRight = PatternArray[i].y + L; break;
            //            case 1: MostRight = PatternArray[i].y; break;
            //            case 2: MostRight = PatternArray[i].y; break;
            //            case 3: MostRight = PatternArray[i].y + W; break;
            //        }
            //        if (MostRight > Result.MaxY) Result.MaxY = MostRight;
            //    }
            //    for (i = 0; i < BundlePerLayer; i++)
            //    {
            //        switch (PatternArray[i].ori)
            //        {
            //            case 0: MostTop = PatternArray[i].x; break;
            //            case 1: MostTop = PatternArray[i].x; break;
            //            case 2: MostTop = PatternArray[i].x + W; break;
            //            case 3: MostTop = PatternArray[i].x + L; break;
            //        }
            //        if (MostTop > Result.MaxX) Result.MaxX = MostTop;
            //    }
            //    Result.MaxY = Result.MaxY * 2;
            //}
            //else
            //{
            //    for (i = 0; i < BundlePerLayer; i++)
            //    {
            //        switch (PatternArray[i].ori)
            //        {
            //            case 0: MostRight = PatternArray[i].y + W; break;
            //            case 1: MostRight = PatternArray[i].y; break;
            //            case 2: MostRight = PatternArray[i].y; break;
            //            case 3: MostRight = PatternArray[i].y + L; break;
            //        }
            //        if (MostRight > Result.MaxY) Result.MaxY = MostRight;
            //    }
            //    for (i = 0; i < BundlePerLayer; i++)
            //    {
            //        switch (PatternArray[i].ori)
            //        {
            //            case 0: MostTop = PatternArray[i].x; break;
            //            case 1: MostTop = PatternArray[i].x; break;
            //            case 2: MostTop = PatternArray[i].x + L; break;
            //            case 3: MostTop = PatternArray[i].x + W; break;
            //        }
            //        if (MostTop > Result.MaxX) Result.MaxX = MostTop;
            //    }
            //    Result.MaxY = Result.MaxY * 2;
            //}

            return Result;
        }
        //===========================================================================
        //===========Pattern direction evaluation and overhang control===============
        //===========================================================================
        private void YoffsetToProcessingPattern(int Yoffset)
        {
            for (int i = 0; i < BundlePerLayer; i++)
            {
                ProcessingPattern[i].y = ProcessingPattern[i].y + Yoffset;
            }
        }
        //===========================================================================
        private void XoffsetToProcessingPattern(int Xoffset)
        {
            for (int i = 0; i < BundlePerLayer; i++)
            {
                ProcessingPattern[i].x = ProcessingPattern[i].x + Xoffset;
            }
        }
        //===========================================================================
        public void ClearLayerPattern()
        {
            Array.Clear(ProcessingPattern, 0, 32);
            for (int i = 0; i < 4; i++)
            {
                WriteToLayerPattern(i, ProcessingPattern);
            }
        }
        //===========================================================================
        public void ReadLayerPatternToProcessingPattern(int LayerIndex)
        {
            for (int i = 0; i < 32; i++)
            {
                ProcessingPattern[i] = LayerPattern[LayerIndex, i];
            }
            ProcessingPatternDimension = Get_PatternDimension(ProcessingPattern);
        }
        //===========================================================================
        public void ReadLayerPatternWithOutGapToProcessingPattern(int LayerIndex)
        {
            for (int i = 0; i < 32; i++)
            {
                ProcessingPattern[i] = LayerPatternWithOutGap[LayerIndex, i];
            }
            ProcessingPatternDimension = Get_PatternDimension(ProcessingPattern);
        }
        //===========================================================================
        public void WriteToLayerPattern(int LayerIndex, Pattern[,] PatternArray)
        {
            for (int i = 0; i < 32; i++)
            {
                LayerPattern[LayerIndex, i] = PatternArray[LayerIndex,i];
            }
        }
        public void WriteToLayerPattern(int LayerIndex, Pattern[] PatternArray)
        {
            for (int i = 0; i < 32; i++)
            {
                LayerPattern[LayerIndex, i] = PatternArray[i];
            }
        }
        //===========================================================================
        public void WriteToLayerPatternWithOutGap(int LayerIndex, Pattern[,] PatternArray)
        {
            for (int i = 0; i < 32; i++)
            {
                LayerPatternWithOutGap[LayerIndex, i] = PatternArray[LayerIndex % 2, i];
            }
        }
        public void WriteToLayerPatternWithOutGap(int LayerIndex, Pattern[] PatternArray)
        {
            for (int i = 0; i < 32; i++)
            {
                LayerPatternWithOutGap[LayerIndex, i] = PatternArray[i];
            }
        }

        #region CreatePatternLayer
        //=== Create 'RotatedPattern' and Assign the least OVH direction to 'LayerPattern'* =========
        // - If Source Pattern is already Underhang choose Source Pattern
        // - If both are overhang choose the least overhang
        // - Store the choosen pattern in LayerPattern
        public void Layer1PatternDirectionEvaluation()
        {
            ///*** Noted ***
            ///1. You must have first basic pattern generated stored in 'ProcessingPattern[]' before process to this function.
            ///2. Pattern which are not allow to rotate are
            ///     Pattern type "S" (Rotate dimensiont is equal to original one)

            OverHang OverHangA;
            OverHang OverHangB;
            Array.Clear(LayerPattern, 0, LayerPattern.Length);
            Array.Clear(LayerPatternWithOutGap, 0, LayerPatternWithOutGap.Length);

            //***** temp to test un rotated pattern
            //for (int i = 0; i < 32; i++)
            //{
            //    WriteToLayerPattern(0, SourcePattern);
            //    WriteToLayerPattern(1, SourcePattern);
            //    WriteToLayerPatternWithOutGap(0, SourcePatternWithOutGap);
            //    WriteToLayerPatternWithOutGap(1, SourcePatternWithOutGap);
            //}
            //Layer1PatternDimension = SourcePatternDimension;
            //Layer1PatternDimensionWithOutGap = SourcePatternDimensionWithOutGap;

            //return; 

            //Generate RotatedPatternWithOutGap
            //Array.Copy(SourcePatternWithOutGap, 0, ProcessingPattern, 0, 32);
            for (int i = 0; i < 32; i++)
                ProcessingPattern[i] = SourcePatternWithOutGap[0, i];
            ProcessingPatternDimension = SourcePatternDimensionWithOutGap;
            PatternRotate();
            //Array.Copy(ProcessingPattern, 0, RotatedPatternWithOutGap, 0, 32);
            for (int i = 0; i < 32; i++)
                RotatedPatternWithOutGap[0, i] = ProcessingPattern[i];
            //RotatedPatternDimensionWithOutGap = Get_PatternDimension(RotatedPatternWithOutGap);
            //RotatedPatternDimensionWithOutGap = Get_PatternDimension(ProcessingPattern);
            RotatedPatternDimensionWithOutGap = ProcessingPatternDimension; 
            for (int i = 0; i < 32; i++)
                ProcessingPattern[i] = SourcePatternWithOutGap[1, i];
            ProcessingPatternDimension = SourcePatternDimensionWithOutGap;
            PatternRotate();
            for (int i = 0; i < 32; i++)
                RotatedPatternWithOutGap[1, i] = ProcessingPattern[i];

            // To check unrotate pattern
            LayerOverHang = CalculateOverHangFromLayerDimension(SourcePatternDimension);
            LayerOverHangWithOutGap = CalculateOverHangFromLayerDimension(SourcePatternDimensionWithOutGap);

            //Generate RotatedPattern
            //Array.Copy(SourcePattern, 0, ProcessingPattern, 0, 32);
            for (int i = 0; i < 32; i++)
                ProcessingPattern[i] = SourcePattern[0, i];
            ProcessingPatternDimension = SourcePatternDimension;
            PatternRotate();
            //Array.Copy(ProcessingPattern, 0, RotatedPattern, 0, 32);
            for (int i = 0; i < 32; i++)
                RotatedPattern[0,i] = ProcessingPattern[i];
            //RotatedPatternDimension = Get_PatternDimension(ProcessingPattern);
            RotatedPatternDimension = ProcessingPatternDimension;
            for (int i = 0; i < 32; i++)
                ProcessingPattern[i] = SourcePattern[1, i];
            ProcessingPatternDimension = SourcePatternDimension;
            PatternRotate();
            for (int i = 0; i < 32; i++)
                RotatedPattern[1,i] = ProcessingPattern[i];

            LayerOverHang.X = 0;
            LayerOverHang.Y = 0;
            LayerOverHang.Total = 0;
            LayerOverHangWithOutGap.X = 0;
            LayerOverHangWithOutGap.Y = 0;
            LayerOverHangWithOutGap.Total = 0;

            //Check SourcePattern can fit in to pallet without any overhang or not
            if ((SourcePatternDimension.MaxY <= PalletWidth) && (SourcePatternDimension.MaxX <= PalletLength))
            {
                //Perfect! then let directly apply 'SourcePattern' to Layer1Pattern
                SuggestRotate = false; //@Beer Check PatternRotate 2020-04-01
            }
            else
            {
                //Shit! How about the 'RotatedPattern'? Can it fit to pallet?
                //By the way, rotate the pattern S is not help since all dimensions are equal.
                if (PatternType == "S")
                {
                    //There is no option. It is only 'SourcePattern' that possible even it is overhang.
                    SuggestRotate = false; //@Beer Check PatternRotate 2020-04-01
                }
                else
                {
                    if ((RotatedPatternDimension.MaxY <= PalletWidth) && (RotatedPatternDimension.MaxX <= PalletLength))
                    //if ((RotatedPatternDimension.MaxY <= PalletWidth) && (RotatedPatternDimension.MaxX <= PalletLong) && true != true) //TestOnly
                    {
                        //Lucky me! then let directly apply 'RotatePattern' to Layer1Pattern
                        SuggestRotate = true; //@Beer Check PatternRotate 2020-04-01
                    }
                    else
                    {
                        //Shit! No choice. Let's next compare 'SourcePattern' and 'RotatedPattern' which one has less overhang.
                        ////ข้างล่างนี้คืออัลกอลิทิ่มที่สนใจเลือก pattern ที่มีผลรวม overhang ทั้งสามด้าน น้อยที่สุด

                        if (SourcePatternDimension.MaxY <= SourcePatternDimension.MaxX)
                        //if (SourcePatternDimension.MaxY <= SourcePatternDimension.MaxX || true == true)//TestOnly
                        {
                            SuggestRotate = false; //@Beer Check PatternRotate 2020-04-01
                        }
                        else
                        {
                            SuggestRotate = true; //@Beer Check PatternRotate 2020-04-01
                        }
                    }
                }
            }

            if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
            {
                //There is no option. It is only 'SourcePattern' that possible even it is overhang.
                // Calculate Overhang
                LayerOverHang = CalculateOverHangFromLayerDimension(SourcePatternDimension);
                LayerOverHangWithOutGap = CalculateOverHangFromLayerDimension(SourcePatternDimensionWithOutGap);
                //Write to Layer Pattern
                WriteToLayerPattern(0, SourcePattern);
                Layer1PatternDimension = SourcePatternDimension;
                WriteToLayerPatternWithOutGap(0, SourcePatternWithOutGap);
                Layer1PatternDimensionWithOutGap = SourcePatternDimensionWithOutGap;
                WriteToLayerPattern(1, SourcePattern);
                WriteToLayerPatternWithOutGap(1, SourcePatternWithOutGap);
            }
            else
            {
                // Calculate Overhang
                LayerOverHang = CalculateOverHangFromLayerDimension(RotatedPatternDimension);
                LayerOverHangWithOutGap = CalculateOverHangFromLayerDimension(RotatedPatternDimensionWithOutGap);
                //Write to Layer Pattern
                WriteToLayerPattern(0, RotatedPattern);
                Layer1PatternDimension = RotatedPatternDimension;
                WriteToLayerPatternWithOutGap(0, RotatedPatternWithOutGap);
                Layer1PatternDimensionWithOutGap = RotatedPatternDimensionWithOutGap;
                WriteToLayerPattern(1, RotatedPattern);
                WriteToLayerPatternWithOutGap(1, RotatedPatternWithOutGap);
            }

        }
        //=== Calculate Overhang ====================================================================
        // - Overhang in X, Y Direction
        // - Overhang Area
        private OverHang CalculateOverHangFromLayerDimension(Dimension LayerDimension)
        {
            OverHang Result;
            Result.Y = (LayerDimension.MaxY - PalletWidth);
            Result.X = (LayerDimension.MaxX - PalletLength);
            // *checkagain* total is the total overhang area but with this formula there will be an overlap area at the 4 corners.
            Result.Total = (Result.Y * LayerDimension.MaxX) + (Result.X * LayerDimension.MaxY);
            Result.Xleft = 0;
            Result.Xright = 0;
            return Result;
        }
        // === Create Pattern for other Layer and store it in 'LayerPattern' array ===================
        // - use LayerPattern-0 as default pattern
        public void NextLayerDataGenerator()
        {
            ReadLayerPatternToProcessingPattern(0);
            switch (PatternType)
            {
                case "C":               //Passed
                    InternalBundleSemiCircleRotate();
                    WriteToLayerPattern(1, ProcessingPattern);
                    break;
                case "C_FixFace":
                    InternalBundleSemiCircleRotate();
                    WriteToLayerPattern(1, ProcessingPattern);
                    break;
                case "I":
                    //for (int i = 0; i < 32; i++)
                    //{
                    //    ProcessingPattern[i] = SourcePattern[1, i];
                    //}
                    //WriteToLayerPattern(1, ProcessingPattern);
                    break;
                case "S":               //Passed
                    for (int i = 0; i < 32; i++)
                    {
                        ProcessingPattern[i] = SourcePattern[0, i];
                    }
                    WriteToLayerPattern(0, ProcessingPattern);
                    InternalBundleSemiCircleRotate();
                    WriteToLayerPattern(2, ProcessingPattern);

                    for (int i = 0; i < 32; i++)
                    {
                        ProcessingPattern[i] = SourcePattern[1, i];
                    }
                    WriteToLayerPattern(1, ProcessingPattern);

                    InternalBundleSemiCircleRotate();
                    WriteToLayerPattern(3, ProcessingPattern);
                    break;
            }
        }
        //===========================================================================
        public void NextLayerDataGeneratorWithOutGap()
        {
            ReadLayerPatternWithOutGapToProcessingPattern(0);
            switch (PatternType)
            {
                case "C":               //Passed
                    InternalBundleSemiCircleRotate();
                    WriteToLayerPatternWithOutGap(1, ProcessingPattern);
                    break;
                case "C_FixFace":
                    InternalBundleSemiCircleRotate();
                    WriteToLayerPatternWithOutGap(1, ProcessingPattern);
                    break;
                case "I":
                    //for (int i = 0; i < 32; i++)
                    //{
                    //    ProcessingPattern[i] = SourcePattern[1, i];
                    //}
                    //WriteToLayerPatternWithOutGap(1, ProcessingPattern);
                    break;
                case "S":               //Passed
                    for (int i = 0; i < 32; i++)
                    {
                        ProcessingPattern[i] = SourcePatternWithOutGap[0, i];
                    }
                    WriteToLayerPatternWithOutGap(0, ProcessingPattern);
                    InternalBundleSemiCircleRotate();
                    WriteToLayerPatternWithOutGap(2, ProcessingPattern);

                    for (int i = 0; i < 32; i++)
                    {
                        ProcessingPattern[i] = SourcePatternWithOutGap[1, i];
                    }
                    WriteToLayerPatternWithOutGap(1, ProcessingPattern);

                    InternalBundleSemiCircleRotate();
                    WriteToLayerPatternWithOutGap(3, ProcessingPattern);
                    break;
            }
        }
        //===========================================================================
        public void ReOrderPatternSequence()
        //public void ReOrderPatternSequence(bool loc_MirrorFunction) // Modify function for mirrored layout ver3.0 2023-05-08
        {
            switch (PatternType)
            {
                case "C":
                    switch (BundlePerLayer)
                    {
                        case 2:
                            {
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "2,1");
                                    ReOrderLayerPatternSequenceByFix(1, "2,1");
                                }
                                break;
                            }
                        case 3:
                            {
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "3,2,1");
                                    ReOrderLayerPatternSequenceByFix(1, "3,2,1");
                                }
                                break;
                            }
                        case 4:
                            if (PatternOptionCode == "22")
                            {
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3,4");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3,4");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "2,4,1,3");
                                    ReOrderLayerPatternSequenceByFix(1, "2,4,1,3");
                                }
                                break;
                            }
                            else
                            {
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3,4");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3,4");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "4,3,2,1");
                                    ReOrderLayerPatternSequenceByFix(1, "4,3,2,1");
                                }
                            }
                            break;
                        case 6:
                            if (SwapSide != true)
                            {
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "2,4,6,1,3,5");
                                    ReOrderLayerPatternSequenceByFix(1, "2,4,6,1,3,5");
                                }
                            }
                            else
                            {
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "2,4,6,1,3,5");
                                    ReOrderLayerPatternSequenceByFix(1, "2,4,6,1,3,5");
                                }
                            }
                            break;
                        case 8:
                            if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                            {
                                ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8");
                                ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8");
                            }
                            else
                            {
                                ReOrderLayerPatternSequenceByFix(0, "2,4,6,8,1,3,5,7");
                                ReOrderLayerPatternSequenceByFix(1, "2,4,6,8,1,3,5,7");
                            }
                            break;
                        case 9:
                            if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                            {
                                ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8,9");
                                ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8,9");
                            }
                            else
                            {
                                ReOrderLayerPatternSequenceByFix(0, "3,6,9,2,5,8,1,4,7");
                                ReOrderLayerPatternSequenceByFix(1, "3,6,9,2,5,8,1,4,7");
                            }
                            break;
                        case 12:
                            if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                            {
                                ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8,9,10,11,12");
                                ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8,9,10,11,12");
                            }
                            else
                            {
                                ReOrderLayerPatternSequenceByFix(0, "4,8,12,3,7,11,2,6,10,1,5,9");
                                ReOrderLayerPatternSequenceByFix(1, "4,8,12,3,7,11,2,6,10,1,5,9");
                            }
                            break;
                        default:
                            ReOrderLayerPatternSequenceByCalculation(0);
                            ReOrderLayerPatternSequenceByCalculation(1);
                            break;
                    }
                    break;
                case "C_FixFace":
                    switch (BundlePerLayer)
                    {
                        case 4:
                            {
                                if (PatternOptionCode == "22")
                                {
                                    if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                    {
                                        ReOrderLayerPatternSequenceByFix(0, "1,2,3,4");
                                        ReOrderLayerPatternSequenceByFix(1, "1,2,3,4");
                                    }
                                    else
                                    {
                                        ReOrderLayerPatternSequenceByFix(0, "2,4,1,3");
                                        ReOrderLayerPatternSequenceByFix(1, "2,4,1,3");
                                    }
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByCalculation(0);
                                    ReOrderLayerPatternSequenceByCalculation(1);
                                }
                                break;
                            }
                        case 6:
                            {
                                if (PatternOptionCode == "32")
                                {
                                    
                                    if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                    {
                                        ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6");
                                        ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6");
                                    }
                                    else
                                    {
                                        ReOrderLayerPatternSequenceByFix(0, "2,4,6,1,3,5");
                                        ReOrderLayerPatternSequenceByFix(1, "2,4,6,1,3,5");
                                    }
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByCalculation(0);
                                    ReOrderLayerPatternSequenceByCalculation(1);
                                }
                                break;
                            }
                        case 8:
                            {
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "2,4,6,8,1,3,5,7");
                                    ReOrderLayerPatternSequenceByFix(1, "2,4,6,8,1,3,5,7");
                                }
                                break;
                            }
                        case 9:
                            {
                                if (PatternOptionCode == "33")
                                {
                                    if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                    {
                                        ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8,9");
                                        ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8,9");
                                    }
                                    else
                                    {
                                        ReOrderLayerPatternSequenceByFix(0, "3,6,9,2,5,8,1,4,7");
                                        ReOrderLayerPatternSequenceByFix(1, "3,6,9,2,5,8,1,4,7");
                                    }
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByCalculation(0);
                                    ReOrderLayerPatternSequenceByCalculation(1);
                                }
                                break;
                            }
                        default:
                            {
                                ReOrderLayerPatternSequenceByCalculation(0);
                                ReOrderLayerPatternSequenceByCalculation(1);
                                break;
                            }
                    }
                    break;
                case "I":
                    switch (BundlePerLayer)
                    {
                        case 3:
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3");
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,3,2");
                                    ReOrderLayerPatternSequenceByFix(1, "2,1,3");
                            }
                            break;
                        case 5:
                            
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5");
                            }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "2,1,5,4,3");
                                    ReOrderLayerPatternSequenceByFix(1, "3,2,1,5,4");
                                }
                            break;
                        case 6:
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "5,4,6,1,3,2");
                                    ReOrderLayerPatternSequenceByFix(1, "4,6,5,2,1,3");
                                }
                            break;
                        case 7:
                                if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7");
                                    ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7");
                                }
                                else
                                {
                                    ReOrderLayerPatternSequenceByFix(0, "2,4,1,3,7,6,5");
                                    ReOrderLayerPatternSequenceByFix(1, "3,2,1,5,7,4,6");
                                }
                            break;
                        case 10:
                            if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                            {
                                ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8,9,10");
                                ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8,9,10");
                            }
                            else
                            {
                                ReOrderLayerPatternSequenceByFix(0, "2,6,10,5,9,1,4,8,3,7");
                                ReOrderLayerPatternSequenceByFix(1, "4,8,3,7,2,6,1,5,10,9");
                            }
                            break;
                        case 11:
                            if ((RotatePattern && SuggestRotate) || (!RotatePattern && !SuggestRotate))
                            {
                                ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8,9,10,11");
                                ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8,9,10,11");
                            }
                            else
                            {
                                ReOrderLayerPatternSequenceByFix(0, "3,2,7,11,6,10,1,5,9,4,8");
                                ReOrderLayerPatternSequenceByFix(1, "4,8,3,7,11,2,6,1,5,10,9");
                            }
                            break;

                    }
                    break;
                case "S":
                    switch (BundlePerLayer)
                    {
                        case 4:
                            ReOrderLayerPatternSequenceByFix(0, "1,2,3,4");
                            ReOrderLayerPatternSequenceByFix(1, "1,2,3,4");
                            ReOrderLayerPatternSequenceByFix(2, "1,2,3,4");
                            ReOrderLayerPatternSequenceByFix(3, "1,2,3,4");
                            break;
                        case 8:
                            ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8");
                            ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8");
                            ReOrderLayerPatternSequenceByFix(2, "1,2,3,4,5,6,7,8");
                            ReOrderLayerPatternSequenceByFix(3, "1,2,3,4,5,6,7,8");
                            break;
                        case 12:
                            ReOrderLayerPatternSequenceByFix(0, "1,2,3,4,5,6,7,8,9,10,11,12");
                            ReOrderLayerPatternSequenceByFix(1, "1,2,3,4,5,6,7,8,9,10,11,12");
                            ReOrderLayerPatternSequenceByFix(2, "1,2,3,4,5,6,7,8,9,10,11,12");
                            ReOrderLayerPatternSequenceByFix(3, "1,2,3,4,5,6,7,8,9,10,11,12");
                            break;
                    }
                    break;
            }
        }
        //===========================================================================
        private void ReOrderLayerPatternSequenceByFix(int LayerIndex, string OrderText)
        {
            Pattern[] TempPattern = new Pattern[32];
            string strA = OrderText;
            string strB;
            int i;
            int n = 0;
            int m;    

            // Set Sequence for Pattern with Gap
            ReadLayerPatternToProcessingPattern(LayerIndex);
            Array.Clear(TempPattern, 0, 32);
            while (strA.Length > 0)
            {
                i = strA.IndexOf(","); // get the index of 1st "," : return -1 if not found
                if (i < 0)
                {
                    strB = strA; // assign the part of strA after the index of "," to strB
                    m = int.Parse(strB); // Convert strB value to int and store in 'm'
                    Array.Copy(ProcessingPattern, m - 1, TempPattern, n, 1); // Copy only 1 element from ProcessingPattern[m-1] to TempPattern[n]
                    strA = ""; // clear strA value
                }
                else
                {
                    strB = strA.Substring(0, i); // assign the part of strA until Index of "," to strB
                    m = int.Parse(strB); // Convert strB value to int and store in 'm'
                    Array.Copy(ProcessingPattern, m - 1, TempPattern, n, 1); // Copy only 1 element from ProcessingPattern[m-1] to TempPattern[n]
                    strA = strA.Substring(i + 1); // assign the part of strA after the index of "," to strA
                }
                n++; // increment n
            }
            Array.Copy(TempPattern, ProcessingPattern, 32); // Copy all element of TempPattern to ProcessingPattern
            WriteToLayerPattern(LayerIndex, ProcessingPattern); // Copy ProcessingPattern to LayerPattern


            // Set Sequence for Pattern without Gap
            Pattern[] _ptmTemp = new Pattern[32];
            TempPattern = new Pattern[32];
            strA = OrderText;
            n = 0;
            for (int x = 0; x < 32; x++)
            {
                _ptmTemp[x] = LayerPatternWithOutGap[LayerIndex, x];
            }
            while (strA.Length > 0)
            {
                i = strA.IndexOf(",");
                if (i < 0)
                {
                    strB = strA;
                    m = int.Parse(strB);
                    Array.Copy(_ptmTemp, m - 1, TempPattern, n, 1);
                    strA = "";
                }
                else
                {
                    strB = strA.Substring(0, i);
                    m = int.Parse(strB);
                    Array.Copy(_ptmTemp, m - 1, TempPattern, n, 1);
                    strA = strA.Substring(i + 1);
                }
                n++;
            }
            Array.Copy(TempPattern, _ptmTemp, 32); // Copy all element of TempPattern to _ptmTemp
            WriteToLayerPatternWithOutGap(LayerIndex, _ptmTemp); // Copy _ptmTemp to LayerPattern
        }
        //===========================================================================
        private void ReOrderLayerPatternSequenceByCalculation(int LayerIndex)
        {
            Pattern[] TempPattern = new Pattern[32];
            int[] MostLeft = new int[32];
            int[] MostTop = new int[32];
            int CurrentMostLeft = 0;
            int CurrentMostTop = 0;
            int MostIndex = 0;
            int iA;
            int i;

            ReadLayerPatternToProcessingPattern(LayerIndex);
            for (i = 0; i < BundlePerLayer; i++)
            {
                switch (ProcessingPattern[i].ori)
                {
                    case 0:
                        MostLeft[i] = ProcessingPattern[i].y;
                        MostTop[i] = ProcessingPattern[i].x;
                        break;
                    case 1:
                        MostLeft[i] = ProcessingPattern[i].y - BundleWidth;
                        MostTop[i] = ProcessingPattern[i].x;
                        break;
                    case 2:
                        MostLeft[i] = ProcessingPattern[i].y - BundleLength;
                        MostTop[i] = ProcessingPattern[i].x - BundleWidth;
                        break;
                    case 3:
                        MostLeft[i] = ProcessingPattern[i].y;
                        MostTop[i] = ProcessingPattern[i].x - BundleLength;
                        break;
                }
            }
            for (i = 0; i < BundlePerLayer; i++)
            {
                CurrentMostLeft = 9999;
                CurrentMostTop = 9999;
                MostIndex = 0;
                if ((PatternType == "x"))        //Special case apply only for I3 according to Ing+Pee recommend
                {
                    for (iA = 0; iA < BundlePerLayer; iA++)
                    {
                        if (MostTop[iA] < CurrentMostTop)
                        {
                            CurrentMostTop = MostTop[iA];
                            CurrentMostLeft = MostLeft[iA];
                            MostIndex = iA;
                        }
                        else if (MostTop[iA] == CurrentMostTop)
                        {
                            if (MostLeft[iA] < CurrentMostLeft)
                            {
                                CurrentMostLeft = MostLeft[iA];
                                MostIndex = iA;
                            }
                        }
                    }
                }
                else
                {
                    for (iA = 0; iA < BundlePerLayer; iA++)
                    {
                        if (MostLeft[iA] < CurrentMostLeft)
                        {
                            CurrentMostLeft = MostLeft[iA];
                            CurrentMostTop = MostTop[iA];
                            MostIndex = iA;
                        }
                        else if (MostLeft[iA] == CurrentMostLeft)
                        {
                            if (MostTop[iA] < CurrentMostTop)
                            {
                                CurrentMostTop = MostTop[iA];
                                MostIndex = iA;
                            }
                        }
                    }
                }
                Array.Copy(ProcessingPattern, MostIndex, TempPattern, i, 1);
                MostLeft[MostIndex] = 9999;
                MostTop[MostIndex] = 9999;
            }
            Array.Copy(TempPattern, ProcessingPattern, BundlePerLayer);
            WriteToLayerPattern(LayerIndex, ProcessingPattern);

            //[05-07-2020]Set Seq bundle for press Pattern
            Pattern[] _ptmTemp = new Pattern[32];
            TempPattern = new Pattern[32];
            for (int x = 0; x < 32; x++)
            {
                _ptmTemp[x] = LayerPatternWithOutGap[LayerIndex, x];
            }
            for (i = 0; i < BundlePerLayer; i++)
            {
                switch (_ptmTemp[i].ori)
                {
                    case 0:
                        MostLeft[i] = _ptmTemp[i].y;
                        MostTop[i] = _ptmTemp[i].x;
                        break;
                    case 1:
                        MostLeft[i] = _ptmTemp[i].y - BundleWidth;
                        MostTop[i] = _ptmTemp[i].x;
                        break;
                    case 2:
                        MostLeft[i] = _ptmTemp[i].y - BundleLength;
                        MostTop[i] = _ptmTemp[i].x - BundleWidth;
                        break;
                    case 3:
                        MostLeft[i] = _ptmTemp[i].y;
                        MostTop[i] = _ptmTemp[i].x - BundleLength;
                        break;
                }
            }
            for (i = 0; i < BundlePerLayer; i++)
            {
                CurrentMostLeft = 9999;
                CurrentMostTop = 9999;
                MostIndex = 0;
                if ((PatternType == "x"))        //Special case apply only for I3 according to Ing+Pee recommend
                {
                    for (iA = 0; iA < BundlePerLayer; iA++)
                    {
                        if (MostTop[iA] < CurrentMostTop)
                        {
                            CurrentMostTop = MostTop[iA];
                            CurrentMostLeft = MostLeft[iA];
                            MostIndex = iA;
                        }
                        else if (MostTop[iA] == CurrentMostTop)
                        {
                            if (MostLeft[iA] < CurrentMostLeft)
                            {
                                CurrentMostLeft = MostLeft[iA];
                                MostIndex = iA;
                            }
                        }
                    }
                }
                else
                {
                    for (iA = 0; iA < BundlePerLayer; iA++)
                    {
                        if (MostLeft[iA] < CurrentMostLeft)
                        {
                            CurrentMostLeft = MostLeft[iA];
                            CurrentMostTop = MostTop[iA];
                            MostIndex = iA;
                        }
                        else if (MostLeft[iA] == CurrentMostLeft)
                        {
                            if (MostTop[iA] < CurrentMostTop)
                            {
                                CurrentMostTop = MostTop[iA];
                                MostIndex = iA;
                            }
                        }
                    }
                }
                Array.Copy(_ptmTemp, MostIndex, TempPattern, i, 1);
                MostLeft[MostIndex] = 9999;
                MostTop[MostIndex] = 9999;
            }
            Array.Copy(TempPattern, _ptmTemp, BundlePerLayer);
            WriteToLayerPatternWithOutGap(LayerIndex, _ptmTemp);

        }
        // === Shift Y-Axis to 'Pallet's Bottom-Edge' instead of 'Pattern-Edge' ======================
        // 
        public void PatternOffsetAndOverHangCalculation()
        {
            int Xoffset;
            for (int n = 0; n < 4; n++)
            {
                ReadLayerPatternToProcessingPattern(n);
                Xoffset = (PalletLength - ProcessingPatternDimension.MaxX) / 2;
                XoffsetToProcessingPattern(Xoffset);
                WriteToLayerPattern(n, ProcessingPattern);

                ReadLayerPatternWithOutGapToProcessingPattern(n);
                Xoffset = (PalletLength - ProcessingPatternDimension.MaxX) / 2;
                XoffsetToProcessingPattern(Xoffset);
                WriteToLayerPatternWithOutGap(n, ProcessingPattern);

            }
        }
        //=== Invert Y-Axis Direction ================================================================
        //
        public void YaxisInverting()
        {
            int n;
            int i;
            for (n = 0; n < 4; n++)
            {
                ReadLayerPatternToProcessingPattern(n);
                for (i = 0; i < BundlePerLayer; i++)
                {
                    ProcessingPattern[i].y = -ProcessingPattern[i].y;
                }
                WriteToLayerPattern(n, ProcessingPattern);

                ReadLayerPatternWithOutGapToProcessingPattern(n);
                for (i = 0; i < BundlePerLayer; i++)
                {
                    ProcessingPattern[i].y = -ProcessingPattern[i].y;
                }
                WriteToLayerPatternWithOutGap(n, ProcessingPattern);
            }
        }
        // === Calculate Squaring Moving Distance on both X,Y-Axis ====================================
        //  -   Origin is at 800 x 900 from center of pallet (half of squaring max length (1600x1800)
        //  -   X-Axis point to bottom-sheet, Y-Axis point out of main robot base
        public void SquaringMoveCalculation()
        {
            // Example *1* Pattern S0422-Dimension: X-885, Y-885
            // Overhang: X: 1200-885 = 315, Y: 1000-885 = 115
            // Bundle Size: W-366, L-499
            // ---------------------------------------------
            // Example *2* Pattern C0933-Dimension: X-1536, Y-1138
            // Overhang: X: 1200-1536 = 336, Y: 1000-1138 = 118
            // Bundle Size: W-366, L-499
            //SQ_XaxisLeftValue = ((PalletLong - Layer1PatternDimension.MaxX) / 2) - SQ_XaxisLeftZeroBiasFromRobotOrigin - SQ_OpenSafeGap;
            //SQ_XaxisLeftValue = -(LayerOverHang.X / 2) - SQ_XaxisLeftZeroBiasFromRobotOrigin;
            // *1*: 457 = - (-315 / 2) - (-300) - 0
            // *2*: 132 = - (366 / 2) - (-300) - 0
            //* SQ_XaxisRightValue = SQ_Max_PeriX - SQ_XaxisLeftValue - Layer1PatternDimension.MaxX - (SQ_OpenSafeGap * 2);
            // *1*: 458 = 1800 - 457 - 885 - (0*2)
            // *2*: 132 = 1800 - 132 - 1536 - (0*2)
            SQ_XaxisRightValue = (SQ_Max_PeriX / 2) - (Layer1PatternDimension.MaxX / 2);
            SQ_YaxisValue = (SQ_Max_PeriY / 2) - (Layer1PatternDimension.MaxY / 2);
            // *1*: 358 = (1600 / 2) - (885 / 2) - 0
            // *2*: 231 = (1600 / 2) - (1138 / 2) - 0

            //SQ_XaxisLeftValueWithOutGap = -(LayerOverHangWithOutGap.X / 2) - SQ_XaxisLeftZeroBiasFromRobotOrigin;
            //* SQ_XaxisRightValueWithOutGap = SQ_Max_PeriX - SQ_XaxisLeftValueWithOutGap - Layer1PatternDimensionWithOutGap.MaxX;
            SQ_XaxisRightValueWithOutGap = (SQ_Max_PeriX / 2) - (Layer1PatternDimensionWithOutGap.MaxX / 2);
            SQ_YaxisValueWithOutGap = (SQ_Max_PeriY / 2) - (Layer1PatternDimensionWithOutGap.MaxY / 2);
        }
        //===========================================================================
        #endregion 
    }
}
