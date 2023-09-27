using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATD_ID4P.Model;

namespace ATD_ID4P.Class
{

    class patternCls
    {
        #region SPattern
        public string SpiralPattern(double FoldLeng, double FoldWidth, int PalletL, int PalletW, Int16 MaxBunduleLayer)
        {
            double _fLenWid, _2fWid1Leng, _2fLeng, _3fWid1Leng, _4fWid1Leng, _4fWid2Leng;
            string sPattern = "";

            try
            {
                _fLenWid = Math.Floor(FoldLeng + FoldWidth);    //Width + Leng Size
                _2fLeng = Math.Floor(FoldLeng * 2);             //Leng x 2
                _2fWid1Leng = Math.Floor(FoldWidth * 2) + FoldLeng;  //Width x 2 + Leng
                _3fWid1Leng = Math.Floor(FoldWidth * 3) + FoldLeng;             //Width x 3
                _4fWid1Leng = Math.Floor(FoldWidth * 4) + FoldLeng;             //Width x 4
                _4fWid2Leng = Math.Floor(FoldWidth * 4) + _2fLeng;             //Width x 4

                sPattern = S3216(PalletL, PalletW, _4fWid2Leng, MaxBunduleLayer);
                if (sPattern != "")
                {
                    return sPattern;
                }
                sPattern = S1688(PalletL, PalletW, _4fWid1Leng, MaxBunduleLayer);
                if (sPattern != "")
                {
                    return sPattern;
                }
                sPattern = S1266(PalletL, PalletW, _3fWid1Leng, MaxBunduleLayer);
                if (sPattern != "")
                {
                    return sPattern;
                }
                sPattern = S0844(PalletL, PalletW, _2fWid1Leng, MaxBunduleLayer);
                if (sPattern != "")
                {
                    return sPattern;
                }
                sPattern = S0422(PalletL, PalletW, _fLenWid, MaxBunduleLayer);

                return sPattern;
            }
            catch (Exception)
            {

                return "";
            }
        }
        static string S0422(double PalletL, double PalletW, double _fLenWid, Int16 MaxBunduleLayer)
        {
            //S0422 Compare avaliable to place on pallet
            string S0422 = ""; bool i_status = false; double PL_LW, PW_LW, QB21, QB22;
            PL_LW = Math.Floor(PalletL / _fLenWid);          //Pallet leng / (width + leng)
            PW_LW = Math.Floor(PalletW / _fLenWid);          //Pallet width / Max size with
            QB21 = Math.Floor(PalletL / _fLenWid);          //Pallet Leng / Max size with
            QB22 = Math.Floor(PalletW / _fLenWid);          //Pallet width / (width + leng)

            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_fLenWid - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_fLenWid - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.S0422 = true;
            else
                PicPtn.S0422 = false;
            //---------------------------------------------------------------
            */
            if (PL_LW == 1 && PW_LW == 1)
            {
                i_status = true;
            }
            else if (QB21 == 1 && QB22 == 1)
            {
                i_status = true;
            }
            else
            {
                i_status = false;
            }

            if (i_status == true && MaxBunduleLayer >= 4)
            {
                S0422 = "S0422:WW:0:" + _fLenWid + ":" + _fLenWid;
            }
            else
            {
                S0422 = "";
            }
            return S0422;
        }

        static string S0844(double PalletL, double PalletW, double _2fWid1Leng, Int16 MaxBunduleLayer)
        {
            //S0844 Compare avaliable to place on pallet
            string S0844 = ""; bool i_status = false; double PL_LW, PW_LW, QB21, QB22;
            PL_LW = Math.Floor(PalletL / _2fWid1Leng);          //Pallet leng / (width + leng)
            PW_LW = Math.Floor(PalletW / _2fWid1Leng);          //Pallet width / Max size with
            QB21 = Math.Floor(PalletL / _2fWid1Leng);          //Pallet Leng / Max size with
            QB22 = Math.Floor(PalletW / _2fWid1Leng);          //Pallet width / (width + leng)
            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_2fWid1Leng - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_2fWid1Leng - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.S0844 = true;
            else
                PicPtn.S0844 = false;
            //---------------------------------------------------------------
            */
            if (PL_LW == 1 && PW_LW == 1)
            {
                i_status = true;
            }
            else if (QB21 == 1 && QB22 == 1)
            {
                i_status = true;
            }
            else
            {
                i_status = false;
            }

            if (i_status == true && MaxBunduleLayer >= 8)
            {
                S0844 = "S0844:WW:0:" + _2fWid1Leng + ":" + _2fWid1Leng;
            }

            return S0844;
        }

        static string S1266(double PalletL, double PalletW, double _3fWid1Leng, Int16 MaxBunduleLayer)
        {
            //S1266 Compare avaliable to place on pallet
            string S1266 = ""; bool i_status = false; double PL_LW, PW_LW, QB21, QB22;

            PL_LW = Math.Floor(PalletL / _3fWid1Leng);          //Pallet leng / (width + leng)
            PW_LW = Math.Floor(PalletW / _3fWid1Leng);          //Pallet width / Max size with
            QB21 = Math.Floor(PalletL / _3fWid1Leng);          //Pallet Leng / Max size with
            QB22 = Math.Floor(PalletW / _3fWid1Leng);          //Pallet width / (width + leng)
            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_3fWid1Leng - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_3fWid1Leng - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.S1266 = true;
            else
                PicPtn.S1266 = false;
            //---------------------------------------------------------------
            */

            if (PL_LW == 1 && PW_LW == 1)
            {
                i_status = true;
            }
            else if (QB21 == 1 && QB22 == 1)
            {
                i_status = true;
            }
            else
            {
                i_status = false;
            }

            if (i_status == true && MaxBunduleLayer >= 12)
            {
                S1266 = "S1266:WW:0:" + _3fWid1Leng + ":" + _3fWid1Leng;
            }
            return S1266;
        }

        static string S1688(double PalletL, double PalletW, double _4fWid1Leng, Int16 MaxBunduleLayer)
        {
            //S1688 Compare avaliable to place on pallet
            string S1688 = ""; bool i_status = false; double PL_LW, PW_LW, QB21, QB22;

            PL_LW = Math.Floor(PalletL / _4fWid1Leng);          //Pallet leng / (width + leng)
            PW_LW = Math.Floor(PalletW / _4fWid1Leng);          //Pallet width / Max size with
            QB21 = Math.Floor(PalletL / _4fWid1Leng);          //Pallet Leng / Max size with
            QB22 = Math.Floor(PalletW / _4fWid1Leng);          //Pallet width / (width + leng)
            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_4fWid1Leng - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_4fWid1Leng - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.S1688 = true;
            else
                PicPtn.S1688 = false;
            //----------------------------------------------------------------
            */

            if (PL_LW == 1 && PW_LW == 1)
            {
                i_status = true;
            }
            else if (QB21 == 1 && QB22 == 1)
            {
                i_status = true;
            }
            else
            {
                i_status = false;
            }

            if (i_status == true && MaxBunduleLayer >= 16)
            {
                S1688 = "S1688:WW:0:" + _4fWid1Leng + ":" + _4fWid1Leng;
            }
            return S1688;
        }

        static string S3216(double PalletL, double PalletW, double _4fWid2Leng, Int16 MaxBunduleLayer)
        {
            //S3216 Compare avaliable to place on pallet
            string S3216 = ""; bool i_status = false; double PL_LW, PW_LW, QB21, QB22;
            PL_LW = Math.Floor(PalletL / _4fWid2Leng);          //Pallet leng / (width + leng)
            PW_LW = Math.Floor(PalletW / _4fWid2Leng);          //Pallet width / Max size with
            QB21 = Math.Floor(PalletL / _4fWid2Leng);          //Pallet Leng / Max size with
            QB22 = Math.Floor(PalletW / _4fWid2Leng);          //Pallet width / (width + leng)
            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_4fWid2Leng - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_4fWid2Leng - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.S3216 = true;
            else
                PicPtn.S3216 = false;
            //----------------------------------------------------------------
            */
            if (PL_LW == 1 && PW_LW == 1)
            {
                i_status = true;
            }
            else if (QB21 == 1 && QB22 == 1)
            {
                i_status = true;
            }
            else
            {
                i_status = false;
            }

            if (i_status == true && MaxBunduleLayer >= 32)
            {
                S3216 = "S3216:WW:0:" + _4fWid2Leng + ":" + _4fWid2Leng;
            }
            return S3216;
        }
        #endregion

        //Calculate for Interlock patten and return maximum bundule per pallte.
        #region IPattern
        public string InterlockPattern(double BLeng, double BWide, int PalletL, int PalletW, Int16 MaxBundleLayer)
        {
            double _2BWide, _1BL1BW, _2BWide1BLeng, _1BLeng, _2BLeng, _3BWide, _4BWide, _5BWide; double TotalLSize = 0, TotalWSize = 0;
            double Max1BL2BW, Max2BL3BW, Max4BW2BW1BL, Max2BL5BW;
            string iPattern = "";
            string[] PatternX = new string[5];
            try
            {
                _1BL1BW = Math.Floor(BLeng + BWide);    //Width + Leng Size
                _2BWide = Math.Floor(BWide * 2);             //Width x 2
                _2BWide1BLeng = Math.Floor(BWide * 2) + BLeng;  //Width x 2 + Leng
                _3BWide = Math.Floor(BWide * 3);             //Width x 3
                _4BWide = Math.Floor(BWide * 4);             //Width x 4
                _5BWide = Math.Floor(BWide * 5);             //Width x 5
                _1BLeng = BLeng;                             //Leng size
                _2BLeng = Math.Floor(BLeng * 2);             //Leng x 2
                Max1BL2BW = Math.Max(BLeng, _2BWide);          //Select max size for width size
                Max2BL3BW = Math.Max(_2BLeng, _3BWide);
                Max4BW2BW1BL = Math.Max(_2BLeng, Math.Max(_4BWide, _2BWide1BLeng));
                Max2BL5BW = Math.Max(_2BLeng, _5BWide);

                PatternX[0] = I0321(PalletL, PalletW, TotalLSize, _1BL1BW, Max1BL2BW, TotalWSize, MaxBundleLayer);
                PatternX[1] = I0532(PalletL, PalletW, TotalLSize, _1BL1BW, Max2BL3BW, TotalWSize, MaxBundleLayer, _2BLeng, _3BWide);
                PatternX[2] = I0642(PalletL, PalletW, TotalLSize, _1BL1BW, Max4BW2BW1BL, TotalWSize, MaxBundleLayer);
                PatternX[3] = I0734(PalletL, PalletW, TotalLSize, _1BL1BW, Max2BL5BW, TotalWSize, MaxBundleLayer);
                PatternX[4] = I0752(PalletL, PalletW, TotalLSize, _1BL1BW, _2BWide1BLeng, Max2BL3BW, TotalWSize, MaxBundleLayer);

                for (int i = 0; i < PatternX.GetLength(0); i++)
                {
                    if (PatternX[i] != "")
                    {
                        iPattern = PatternX[i];
                    }
                }

                return iPattern;

            }

            catch (Exception)
            {
                return "";
            }
        }
        static string I0321(double PalletL, double PalletW, double TotalLSize, double _1BL1BW, double Max1BL2BW,
                    double TotalWSize, Int16 MaxBundleLayer)
        {
            string I0321, PSide = ""; bool i_status;
            double PLTotal, PW_Max, PL_Max, PWTotal;
            //I0321 Compare avaliable to place on pallet
            PLTotal = Math.Floor(PalletL / _1BL1BW);          //Pallet leng / (width + leng) L
            PWTotal = Math.Floor(PalletW / _1BL1BW);          //Pallet width / (width + leng)W
            PW_Max = Math.Floor(PalletW / Max1BL2BW);          //Pallet width / Max size with  L
            PL_Max = Math.Floor(PalletL / Max1BL2BW);          //Pallet Leng / Max size with   W
            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_1BL1BW - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_1BL1BW - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((Max1BL2BW - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((Max1BL2BW - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.I0321 = true;
            else
                PicPtn.I0321 = false;
            //----------------------------------------------------------------
            */
            try
            {
                if (PL_Max == 1 && PWTotal == 1)
                {
                    i_status = true; TotalWSize = Math.Min(_1BL1BW, Max1BL2BW); TotalLSize = Math.Max(_1BL1BW, Max1BL2BW);
                    PSide = ":LW:0:";
                }
                else if (PLTotal == 1 && PW_Max == 1)
                {
                    i_status = true; TotalLSize = Math.Max(_1BL1BW, Max1BL2BW); TotalWSize = Math.Min(_1BL1BW, Max1BL2BW);
                    PSide = ":WL:1:";
                }
                else
                {
                    i_status = false;
                }

                if (i_status == true && MaxBundleLayer >= 3)
                {
                    I0321 = "I0321" + PSide + TotalLSize + ":" + TotalWSize;
                    //L,W : patterns.Direction 0/1: Rotation pallet, Total size long, Total size wide
                }
                else
                {
                    I0321 = "";
                }
            }
            catch (Exception)
            {
                I0321 = "";
            }
            return I0321;
        }

        static string I0532(double PalletL, double PalletW, double TotalLSize, double _1BL1BW, double Max2BL3BW,
            double TotalWSize, Int16 MaxBundleLayer, double _2BLeng, double _3BWide)
        {
            string I0532, PSide = ""; bool i_status = false;
            double PLTotal, PW_Max, PL_Max, PWTotal; double _3BW2BL;

            //I0532 Compare avaliable to place on pallet
            PLTotal = Math.Floor(PalletL / _1BL1BW);         //Pallet leng / (width + leng)
            PW_Max = Math.Floor(PalletW / Max2BL3BW);          //Pallet width / Max size with
            PL_Max = Math.Floor(PalletL / Max2BL3BW);          //Pallet Leng / Max size with
            PWTotal = Math.Floor(PalletW / _1BL1BW);         //Pallet width / (width + leng)
            _3BW2BL = _3BWide - _2BLeng;                     //3BW - 2BL >= 0
            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_1BL1BW - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_1BL1BW - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((Max2BL3BW - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((Max2BL3BW - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.I0532 = true;
            else
                PicPtn.I0532 = false;
            //---------------------------------------------------------------
            */
            try
            {
                if (PL_Max == 1 && PWTotal == 1)
                {
                    //i_status = true; TotalLSize = Math.Min(_1BL1BW, Max2BL3BW); TotalWSize = Math.Max(_1BL1BW, Max2BL3BW);
                    //PSide = ":LW:0:";//Pattern Name
                    //PSide = patterns.Direction side L,W/Rotation pallet 1,0/Total long size/Total wide size

                    if (_3BW2BL >= 0)
                    {
                        i_status = true; TotalWSize = Math.Min(_1BL1BW, Max2BL3BW); TotalLSize = Math.Max(_1BL1BW, Max2BL3BW);
                        PSide = ":L1:1:";
                    }
                    else
                    {
                        i_status = true; TotalWSize = Math.Min(_1BL1BW, Max2BL3BW); TotalLSize = Math.Max(_1BL1BW, Max2BL3BW);
                        PSide = ":L2:1:";
                    }

                }
                else if (PLTotal == 1 && PW_Max == 1)
                {
                    if (_3BW2BL >= 0)
                    {
                        i_status = true; TotalLSize = Math.Max(_1BL1BW, Max2BL3BW); TotalWSize = Math.Min(_1BL1BW, Max2BL3BW);
                        PSide = ":W1:0:";
                    }
                    else
                    {
                        i_status = true; TotalLSize = Math.Max(_1BL1BW, Max2BL3BW); TotalWSize = Math.Min(_1BL1BW, Max2BL3BW);
                        PSide = ":W2:0:";
                    }
                }
                else
                {
                    i_status = false;
                }

                if (i_status == true && MaxBundleLayer >= 5)
                {
                    I0532 = "I0532" + PSide + TotalLSize + ":" + TotalWSize;
                }
                else
                {
                    I0532 = "";
                }
            }
            catch (Exception)
            {
                I0532 = "";
            }
            return I0532;
        }

        static string I0642(double PalletL, double PalletW, double TotalLSize, double _1BL1BW, double Max4BW2BW1BL,
            double TotalWSize, Int16 MaxBundleLayer)
        {
            string I0642, PSide = ""; bool i_status;
            double PLTotal, PW_Max, PL_Max, PWTotal;
            //I0642 Compare avaliable to place on pallet
            PLTotal = Math.Floor(PalletL / _1BL1BW);          //Pallet leng / (width + leng)
            PW_Max = Math.Floor(PalletW / Max4BW2BW1BL);          //Pallet width / Max size with
            PL_Max = Math.Floor(PalletL / Max4BW2BW1BL);          //Pallet Leng / Max size with
            PWTotal = Math.Floor(PalletW / _1BL1BW);          //Pallet width / (width + leng)
            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_1BL1BW - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_1BL1BW - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((Max4BW2BW1BL - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((Max4BW2BW1BL - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.I0642 = true;
            else
                PicPtn.I0642 = false;
            //---------------------------------------------------------------
            */
            try
            {
                if (PL_Max == 1 && PWTotal == 1)
                {
                    i_status = true; TotalWSize = Math.Min(_1BL1BW, Max4BW2BW1BL); TotalLSize = Math.Max(_1BL1BW, Max4BW2BW1BL);
                    PSide = ":LW:0:";
                }
                else if (PLTotal == 1 && PW_Max == 1)
                {
                    i_status = true; TotalLSize = Math.Max(_1BL1BW, Max4BW2BW1BL); TotalWSize = Math.Min(_1BL1BW, Max4BW2BW1BL);
                    PSide = ":WL:1:";
                }
                else
                {
                    i_status = false;
                }

                if (i_status == true && MaxBundleLayer >= 6)
                {
                    I0642 = "I0642" + PSide + TotalLSize + ":" + TotalWSize;
                }
                else
                {
                    I0642 = "";
                }
            }
            catch (Exception)
            {
                I0642 = "";
            }
            return I0642;
        }

        static string I0734(double PalletL, double PalletW, double TotalLSize, double _1BL1BW, double Max2BL5BW,
           double TotalWSize, Int16 MaxBundleLayer)
        {
            string I0734, PSide = ""; bool i_status;
            double PLTotal, PW_Max, PL_Max, PWTotal;
            //I0734 Compare avaliable to place on pallet
            PLTotal = Math.Floor(PalletL / _1BL1BW);          //Pallet leng / (width + leng)
            PW_Max = Math.Floor(PalletW / Max2BL5BW);          //Pallet width / Max size with
            PL_Max = Math.Floor(PalletL / Max2BL5BW);          //Pallet Leng / Max size with
            PWTotal = Math.Floor(PalletW / _1BL1BW);          //Pallet width / (width + leng)
            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_1BL1BW - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_1BL1BW - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((Max2BL5BW - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((Max2BL5BW - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.I0734 = true;
            else
                PicPtn.I0734 = false;
            //---------------------------------------------------------------
            */
            try
            {
                if (PL_Max == 1 && PWTotal == 1)
                {
                    i_status = true; TotalWSize = Math.Min(_1BL1BW, Max2BL5BW); TotalLSize = Math.Max(_1BL1BW, Max2BL5BW);
                    PSide = ":LW:0:";
                }
                else if (PLTotal == 1 && PW_Max == 1)
                {
                    i_status = true; TotalLSize = Math.Max(_1BL1BW, Max2BL5BW); TotalWSize = Math.Min(_1BL1BW, Max2BL5BW);
                    PSide = ":WL:1:";
                }
                else
                {
                    i_status = false;
                }

                if (i_status == true && MaxBundleLayer >= 7)
                {
                    I0734 = "I0734" + PSide + TotalLSize + ":" + TotalWSize;
                }
                else
                {
                    I0734 = "";
                }
            }
            catch (Exception)
            {
                I0734 = "";
            }
            return I0734;
        }

        static string I0752(double PalletL, double PalletW, double TotalLSize, double _1BL1BW, double _2BWide1BLeng,
           double Max2BL3BW, double TotalWSize, Int16 MaxBundleLayer)
        {
            string I0752, PSide = ""; bool i_status;
            double PLTotal, PW_Max, PL_Max, PWTotal;

            //I0752 Compare avaliable to place on pallet
            PLTotal = Math.Floor(PalletL / _2BWide1BLeng);          //Pallet leng / (width + leng)
            PW_Max = Math.Floor(PalletW / Max2BL3BW);          //Pallet width / Max size with
            PL_Max = Math.Floor(PalletL / Max2BL3BW);          //Pallet Leng / Max size with
            PWTotal = Math.Floor(PalletW / _2BWide1BLeng);          //Pallet width / (width + leng)

            /* *checkagain*
            //------------Check pattern over for enable picture--------------
            if (((_1BL1BW - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_1BL1BW - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_2BWide1BLeng - PalletW) / 2) < Properties.Settings.Default.MaxSlideGuid &&
                ((_2BWide1BLeng - PalletL) / 2) < Properties.Settings.Default.MaxSlideGuid)
                PicPtn.I0752 = true;
            else
                PicPtn.I0752 = false;
            //---------------------------------------------------------------
            */
            try
            {
                if (PL_Max == 1 && PWTotal == 1)
                {
                    i_status = true; TotalWSize = Math.Min(_2BWide1BLeng, Max2BL3BW); TotalLSize = Math.Max(_2BWide1BLeng, Max2BL3BW);
                    PSide = ":LW:0:";
                }
                else if (PLTotal == 1 && PW_Max == 1)
                {
                    i_status = true; TotalLSize = Math.Max(_2BWide1BLeng, Max2BL3BW); TotalWSize = Math.Min(_2BWide1BLeng, Max2BL3BW);
                    PSide = ":WL:1:";
                }
                else
                {
                    i_status = false;
                }

                if (i_status == true && MaxBundleLayer >= 7)
                {
                    I0752 = "I0752" + PSide + TotalLSize + ":" + TotalWSize;
                }
                else
                {
                    I0752 = "";
                }
            }
            catch (Exception)
            {
                I0752 = "";
            }
            return I0752;
        }
        #endregion

        //Calculate for column patten and retuen maximum bundule per pallet.
        public string ColumnPattern(double FoldLeng, double FoldWidth, int PalletL, int PalletW, string[,] ColPattern, Int16 MaxBundleLayer)
        {


            double LogPlaceLong, WidePlacLong, WidePlaceWide, LongPlaceWide; string c_name;
            Int16 LongNo, WideNo, LongPlace1, WidePlace1, WidePlace2, LongPlace2, maxQty, c_bundule;
            bool LongPLongPalletLong, WidePWidePalletWide, LongPLongPalletWide, WidePWidePalletLong, WidePLongPalletLong, LongPWidePalletWide, LongPWidePalletLong, WidePLongPalletWide;
            c_bundule = 0; c_name = "";

            for (int i = 0; i < ColPattern.GetLength(0); i++)
            {
                LongPLongPalletLong = false; WidePWidePalletWide = false; LongPLongPalletWide = false; WidePWidePalletLong = false; WidePLongPalletLong = false;
                LongPWidePalletWide = false; LongPWidePalletLong = false; WidePLongPalletWide = false;
                LongNo = 0; WideNo = 0; LongPlace1 = 0; WidePlace1 = 0; WidePlace2 = 0; LongPlace2 = 0; maxQty = 0;
                if (ColPattern[i, 1].Substring(0, 1) == "C")
                {
                    LongNo = Convert.ToInt16(ColPattern[i, 2]);
                    WideNo = Convert.ToInt16(ColPattern[i, 3]);
                    LogPlaceLong = FoldLeng * LongNo;
                    WidePlacLong = FoldWidth * LongNo;
                    WidePlaceWide = FoldWidth * WideNo;
                    LongPlaceWide = FoldLeng * WideNo;
                    if (LogPlaceLong <= PalletL) { LongPLongPalletLong = true; }
                    if (LogPlaceLong <= PalletW) { LongPLongPalletWide = true; }
                    if (WidePlaceWide <= PalletW) { WidePWidePalletWide = true; }
                    if (WidePlaceWide <= PalletL) { WidePWidePalletLong = true; }
                    if (WidePlacLong <= PalletL) { WidePLongPalletLong = true; }
                    if (WidePlacLong <= PalletW) { WidePLongPalletWide = true; }
                    if (LongPlaceWide <= PalletW) { LongPWidePalletWide = true; }
                    if (LongPlaceWide <= PalletL) { LongPWidePalletLong = true; }

                    //Compare side to avaliable
                    if (LongPLongPalletLong && WidePWidePalletWide) { LongPlace1 = Convert.ToInt16(ColPattern[i, 4]); } //Pallet 90 Bundle O
                    if (LongPLongPalletWide && WidePWidePalletLong) { WidePlace1 = Convert.ToInt16(ColPattern[i, 4]); } //Pallet 0 Bundle O
                    if (WidePLongPalletLong && LongPWidePalletWide) { WidePlace2 = Convert.ToInt16(ColPattern[i, 4]); } //Pallet 90 Bundle 9O
                    if (LongPWidePalletLong && WidePLongPalletWide) { LongPlace2 = Convert.ToInt16(ColPattern[i, 4]); } //Pallet 0 Bundle 9O
                    //Maxinum quantity per bundle
                    maxQty = Math.Max(LongPlace1, Math.Max(WidePlace1, Math.Max(WidePlace2, LongPlace2)));

                    if (c_bundule < maxQty && MaxBundleLayer >= maxQty)
                    {
                        string c_direction = "WL"; double TotalLSize, TotalWSize;

                        if (WidePlace1 == maxQty)  //Pallet 0 Bundle O
                        {
                            TotalLSize = LongPlaceWide;
                            TotalWSize = WidePlacLong;
                            //c_direction = ":WL:0:" + TotalLSize.ToString() + ":" + TotalWSize.ToString();
                            c_direction = ":WL:0:" + TotalLSize.ToString() + ":" + TotalWSize.ToString();
                        }
                        else if (WidePlace2 == maxQty)  //Pallet 90 Bundle 9O
                        {
                            TotalLSize = WidePlacLong;
                            TotalWSize = LongPlaceWide;
                            //c_direction = ":LW:0:" + TotalLSize.ToString() + ":" + TotalWSize.ToString();
                            c_direction = ":LW:0:" + TotalLSize.ToString() + ":" + TotalWSize.ToString();
                        }
                        else if (LongPlace1 == maxQty) //Pallet 90 Bundle O
                        {
                            TotalLSize = LogPlaceLong;
                            TotalWSize = WidePlaceWide;
                            //c_direction = ":LL:1:" + TotalLSize.ToString() + ":" + TotalWSize.ToString();
                            c_direction = ":LW:1:" + TotalLSize.ToString() + ":" + TotalWSize.ToString();
                            //L,W : patterns.Direction 0/1: Rotation pallet, Total size long, Total size wide
                        }
                        else if (LongPlace2 == maxQty)  //Pallet 0 Bundle 9O
                        {
                            TotalLSize = LogPlaceLong;
                            TotalWSize = WidePlaceWide;
                            //c_direction = ":WW:1:" + TotalLSize.ToString() + ":" + TotalWSize.ToString();
                            c_direction = ":WW:1:" + TotalLSize.ToString() + ":" + TotalWSize.ToString();
                        }

                        c_bundule = maxQty;
                        c_name = ColPattern[i, 1] + c_direction;
                        ColumnPtnChk_(ColPattern[i, 1]);

                    }

                }
            }

            void ColumnPtnChk_(string columnnamePtn)
            {

                if (columnnamePtn == "C0111")
                    PicPtn.C0111 = true;
                if (columnnamePtn == "C0221")
                    PicPtn.C0221 = true;
                if (columnnamePtn == "C0331")
                    PicPtn.C0331 = true;
                if (columnnamePtn == "C0441")
                    PicPtn.C0441 = true;
                if (columnnamePtn == "C0422")
                    PicPtn.C0422 = true;
                if (columnnamePtn == "C0661")
                    PicPtn.C0661 = true;
                if (columnnamePtn == "C0632")
                    PicPtn.C0632 = true;
                if (columnnamePtn == "C0818")
                    PicPtn.C0818 = true;
                if (columnnamePtn == "C0933")
                    PicPtn.C0933 = true;
                if (columnnamePtn == "C0919")
                    PicPtn.C0919 = true;
                if (columnnamePtn == "C1243")
                    PicPtn.C1243 = true;
                if (columnnamePtn == "C1427")
                    PicPtn.C1427 = true;
                if (columnnamePtn == "C2054")
                    PicPtn.C2054 = true;
                if (columnnamePtn == "C2446")
                    PicPtn.C2446 = true;
            }

            return c_name;
        }
    }

    public class patternsProp
    {
        private string pattName;
        //*rename* private int pattLeng;
        private int pattLength;
        //*rename* private int pattWide;
        private int pattWidth;
        //*private int pattTall;
        //*rename* private int pattAmount;
        private int pattBPL;
        //*private int pattTie;
        private bool pattTurn;
        //*private bool pattFlip;
        //*private string pattDirection;
        //*private string pattID;
        //*private string pattDesc;
        //*private string pattImg;
        //*private string pattStack;
        //*private int pattSG;

        [CategoryAttribute("Appearance"), DescriptionAttribute("Pattern name"), ReadOnly(true)]
        public string Name { get { return pattName; } set { pattName = value; } }//Name of pattern Exm S0422
        [CategoryAttribute("Appearance"), DescriptionAttribute("Degree of turning pattern 90, 180, 270"), ReadOnly(false)]
        public bool Turn { get { return pattTurn; } set { pattTurn = value; } }
        //*[CategoryAttribute("Appearance"), DescriptionAttribute("Flip pattern (mirror solution)"), ReadOnly(false)]
        //*public bool Flip { get { return pattFlip; } set { pattFlip = value; } }
        //*[CategoryAttribute("Appearance"), DescriptionAttribute("Direection to load pattern L=Leng, W=Wide"), ReadOnly(false)]
        //*public string Direction { get { return pattDirection; } set { pattDirection = value; } }
        [CategoryAttribute("Appearance"), DescriptionAttribute("Amount of pattern"), ReadOnly(false)]
        public int BundlePerLayer { get { return pattBPL; } set { pattBPL = value; } }
        //*[CategoryAttribute("Appearance"), DescriptionAttribute("Tie sheet every layer"), ReadOnly(false)]
        //*public int TieSheet { get { return pattTie; } set { pattTie = value; } }
        //*[CategoryAttribute("Appearance"), DescriptionAttribute("Identity number."), ReadOnly(true)]
        //*public string ID { get { return pattID; } set { pattID = value; } }

        [CategoryAttribute("Dimension"), DescriptionAttribute("Leng side (mm.)"), ReadOnly(true)]
        public int Length { get { return pattLength; } set { pattLength = value; } } //Total leng size
        [CategoryAttribute("Dimension"), DescriptionAttribute("Wide side (mm.)"), ReadOnly(true)]
        public int Width { get { return pattWidth; } set { pattWidth = value; } } //Total wide size
        //*[CategoryAttribute("Dimension"), DescriptionAttribute("Tallness of stacking on pattern (mm.)"), ReadOnly(false)]
        //*public int Tallness { get { return pattTall; } set { pattTall = value; } }
        //*[CategoryAttribute("Other"), DescriptionAttribute("Information of pattern"), ReadOnly(false)]
        //*public string Description { get { return pattDesc; } set { pattDesc = value; } }
        //*[CategoryAttribute("Other"), DescriptionAttribute("Image of pattern"), ReadOnly(false)]
        //*public string ImgPattern { get { return pattImg; } set { pattImg = value; } }
        //*[CategoryAttribute("Other"), DescriptionAttribute("Image of stacking"), ReadOnly(false)]
        //*public string ImgStacking { get { return pattStack; } set { pattStack = value; } }
        //*[CategoryAttribute("Other"), DescriptionAttribute("Slid guid position -Overhang +Underhang"), ReadOnly(true)]
        //*public int SlidGuid { get { return pattSG; } set { pattSG = value; } }
    }

}
