using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATD_ID4P.Class
{
    public class Bundle3DCls
    {
        public List<Bundle3DModel> BDS = new List<Bundle3DModel>();
        public List<Bundle3DModel> Pallet3D = new List<Bundle3DModel>();
        List<Pattern3DModel> PTN3Ds = new List<Pattern3DModel>();

        public string Pattern;
        public int BWidth = 50;
        public int BLength = 70;
        public int BHeight = 20;
        private int OC = 35;
        public Point drawOrigin = new Point(80, 200);

        private void InitPattern3D()
        {
            if (PTN3Ds == null || PTN3Ds.Count == 0)
            {
                string _Pattrn3DFilePath = Application.StartupPath + Properties.Settings.Default.Pattern3DFilePath;
                if (System.IO.File.Exists(_Pattrn3DFilePath))
                {
                    string _P3DText = System.IO.File.ReadAllText(Application.StartupPath + Properties.Settings.Default.Pattern3DFilePath);
                    PTN3Ds = JsonConvert.DeserializeObject<List<Pattern3DModel>>(_P3DText);
                }
            }
        }

        public void AddBundlebyBundleNo(int BundleNo)
        {
            //Find Pattern
            InitPattern3D();
            Pattern3DModel _CURR_ptm = PTN3Ds.Where(x => x.Pattern == Pattern).FirstOrDefault();
            if (_CURR_ptm == null)
                return;

            int _Y = 0;
            Bundle3DModel SourceBD = new Bundle3DModel();
            if (BundleNo <= _CURR_ptm.BDS.Count)
            {
                SourceBD = _CURR_ptm.BDS.Where(c => c.No == BundleNo).FirstOrDefault();
                _Y = SourceBD.Y;
            }
            else
            {
                int PCode = Convert.ToInt32(Pattern.Substring(1, 2));
                int MaxPattern = _CURR_ptm.BDS.Count();
                int ModV = (BundleNo % MaxPattern);
                int PFloor = (BundleNo / MaxPattern);
                PFloor = (ModV == 0 ? PFloor - 1 : PFloor);
                int TargetBDNo = (ModV == 0 ? MaxPattern : ModV);
                SourceBD = _CURR_ptm.BDS.Where(c => c.No == TargetBDNo).FirstOrDefault();
                _Y = SourceBD.Y - ((PFloor * 2) * _CURR_ptm.YStep1);
            }

            int LastNo = (BDS == null ? 0 : BDS.Count);
            Bundle3DModel NewBundle = new Bundle3DModel
            {
                No = LastNo + 1,
                Width = _CURR_ptm.BDS[0].Width,
                Length = _CURR_ptm.BDS[0].Length,
                Height = _CURR_ptm.BDS[0].Height,
                Ori = SourceBD.Ori,
                X = SourceBD.X,
                Y = _Y
            };
            BDS.Add(NewBundle);
        }

        public void DrawBundleOld(PictureBox PB)
        {
            Image Temp = new Bitmap(PB.Width, PB.Height);
            if (PB.Image != null)
                PB.Image.Dispose();
            PB.Image = null;
            DrawPallet(PB);
            Temp = PB.Image;

            int[] DrawStep = GetDrawStep();

            int CPattern = Convert.ToInt32(Pattern.Substring(3, 1));
            int XOrigin = (PB.Width / 2) - ((CPattern - 1) * BWidth);
            int YOrigin = PB.Height - ((CPattern + 1) * BHeight);
            XOrigin = (XOrigin < 50 ? 50 : XOrigin);
            drawOrigin = new Point(XOrigin, YOrigin);

            using (Graphics g = Graphics.FromImage(Temp))
            {
                foreach (Bundle3DModel Bundle in BDS.OrderBy(x => Array.IndexOf(DrawStep, x.No)))
                {
                    //--Set Bundle Properties--
                    Math3D.Cube BC = new Math3D.Cube(Bundle.Width, Bundle.Height, Bundle.Length);
                    if (Bundle.Ori == 1 || Bundle.Ori == 3)
                    {
                        BC = new Math3D.Cube(BLength, BHeight, BWidth);
                    }
                    BC.RotateY = (float)OC;
                    BC.RotateX = (float)-OC;
                    BC.RotateZ = (float)0;
                    BC.FillBack = true;
                    BC.FillBottom = true;
                    BC.FillFront = true;
                    BC.FillLeft = true;
                    BC.FillRight = true;
                    BC.FillTop = true;

                    Image ImgBundle = BC.DrawCube(drawOrigin);
                    if (Temp == null)
                        Temp = ImgBundle;
                    else
                        g.DrawImage(ImgBundle, Bundle.X, Bundle.Y);

                    ////--Draw Bundle--
                    //Image ImgBundle = BC.DrawCube(drawOrigin);
                    //if (Temp == null)
                    //{
                    //    Temp = ImgBundle;
                    //}
                    //else
                    //{
                    //    Graphics g = Graphics.FromImage(Temp);
                    //    //--Draw Border--
                    //    //using (Graphics c = Graphics.FromImage(ImgBundle))
                    //    //{
                    //    //    c.DrawRectangle(new Pen(Brushes.Yellow, 5), new Rectangle(0, 0, ImgBundle.Width, ImgBundle.Height));
                    //    //}
                    //    g.DrawImage(ImgBundle, Bundle.X, Bundle.Y);
                    //    //--Draw Dot--
                    //    //g.DrawEllipse(new Pen(Brushes.Red, 5), new Rectangle(Bundle.X, Bundle.Y, 5, 5));
                    //    g.Dispose();
                    //}
                }
                g.Dispose();
            }

            PB.Image = Temp;
        }

        public void DrawBundle(PictureBox PB)
        {
            PB.Image = null;
            DrawPallet(PB);

            //Point palletOrigin = new Point(180, 200);
            int XOrigin = 150;
            int YOrigin = PB.Height - 160;
            YOrigin = (YOrigin < 50 ? 50 : YOrigin);
            Point palletOrigin = new Point(XOrigin, YOrigin);
            Image Temp = PB.Image;

            int[] IDX = new int[0];

            //Find Pattern
            InitPattern3D();
            Pattern3DModel _CURR_ptm = PTN3Ds.Where(x => x.Pattern == Pattern).FirstOrDefault();
            if (_CURR_ptm != null)
                //IDX = _CURR_ptm.OrderStep;
                IDX = GenarateFullDrawStep(_CURR_ptm.OrderStep, _CURR_ptm.OrderStep.Length);


            foreach (Bundle3DModel BM in BDS.OrderBy(x => Array.IndexOf(IDX, x.No)))
            {
                Math3D.Cube BC = new Math3D.Cube(BM.Width, BM.Height, BM.Length);
                if (BM.Ori == 1 || BM.Ori == 3)
                    BC = new Math3D.Cube(BM.Length, BM.Height, BM.Width);

                BC.RotateY = (float)OC;
                BC.RotateX = (float)-OC;
                BC.RotateZ = (float)0;
                BC.FillBack = true;
                BC.FillBottom = true;
                BC.FillFront = true;
                BC.FillLeft = true;
                BC.FillRight = true;
                BC.FillTop = true;

                Image ImgBundle = BC.DrawCube(palletOrigin);

                if (Temp == null)
                {
                    Temp = ImgBundle;
                }
                else
                {
                    Graphics g = Graphics.FromImage(Temp);

                    g.DrawImage(ImgBundle, BM.X, BM.Y);
                    g.Dispose();
                }
            }
            PB.Image = Temp;
        }

        private void DrawPallet(PictureBox PB)
        {
            Image Temp = new Bitmap(PB.Width, PB.Height);

            int XOrigin = 150;
            int YOrigin = PB.Height - 160;
            YOrigin = (YOrigin < 50 ? 50 : YOrigin);
            Point palletOrigin = new Point(XOrigin, YOrigin);

            Pallet3D = new List<Bundle3DModel>();
            if (Pallet3D.Count == 0)
            {
                Bundle3DModel P3D;
                P3D = new Bundle3DModel() { Width = 180, Height = 12, Length = 12, X = -60, Y = 130, Ori = 0, };
                Pallet3D.Add(P3D);
                P3D = new Bundle3DModel() { Width = 180, Height = 12, Length = 12, X = -25, Y = 101, Ori = 0 };
                Pallet3D.Add(P3D);
                P3D = new Bundle3DModel() { Width = 180, Height = 12, Length = 12, X = 12, Y = 70, Ori = 0 };
                Pallet3D.Add(P3D);

                P3D = new Bundle3DModel() { Width = 150, Height = 3, Length = 12, X = -87, Y = 65, Ori = 1 };
                Pallet3D.Add(P3D);
                P3D = new Bundle3DModel() { Width = 150, Height = 3, Length = 12, X = -62, Y = 75, Ori = 1 };
                Pallet3D.Add(P3D);
                P3D = new Bundle3DModel() { Width = 150, Height = 3, Length = 12, X = -33, Y = 86, Ori = 1 };
                Pallet3D.Add(P3D);
                P3D = new Bundle3DModel() { Width = 150, Height = 3, Length = 12, X = -4, Y = 98, Ori = 1 };
                Pallet3D.Add(P3D);
                P3D = new Bundle3DModel() { Width = 150, Height = 3, Length = 12, X = 24, Y = 109, Ori = 1 };
                Pallet3D.Add(P3D);
                P3D = new Bundle3DModel() { Width = 150, Height = 3, Length = 12, X = 50, Y = 120, Ori = 1 };
                Pallet3D.Add(P3D);
            }

            foreach (Bundle3DModel PL3D in Pallet3D)
            {
                Math3D.Cube BC = new Math3D.Cube(PL3D.Width, PL3D.Height, PL3D.Length);
                if (PL3D.Ori == 1 || PL3D.Ori == 3)
                {
                    BC = new Math3D.Cube(PL3D.Length, PL3D.Height, PL3D.Width);
                }
                BC.RotateY = (float)OC;
                BC.RotateX = (float)-OC;
                BC.RotateZ = (float)0;
                BC.FillBack = true;
                BC.FillBottom = true;
                BC.FillFront = true;
                BC.FillLeft = true;
                BC.FillRight = true;
                BC.FillTop = true;

                Image ImgBundle = BC.DrawCube(palletOrigin, Brushes.Crimson, Brushes.DarkRed);
                if (Temp == null)
                {
                    Temp = ImgBundle;
                }
                else
                {
                    Graphics g = Graphics.FromImage(Temp);

                    g.DrawImage(ImgBundle, PL3D.X, PL3D.Y);
                    g.Dispose();
                }

            }
            PB.Image = Temp;
        }

        private int[] GetDrawStep()
        {
            int[] DrawStep = { 0 };
            switch (Pattern)
            {
                case "C0422":
                case "S0422":
                    {
                        DrawStep = new int[] { 3, 4, 1, 2 };
                        DrawStep = GenarateFullDrawStep(DrawStep, 4);
                        break;
                    }
                case "I0321":
                    {
                        DrawStep = new int[] { 2, 1, 3, 4, 6, 5 };
                        DrawStep = GenarateFullDrawStep(DrawStep, 6);
                        break;
                    }

                case "I0532":
                    {
                        DrawStep = new int[] { 3, 2, 1, 5, 4, 7, 6, 10, 9, 8 };
                        DrawStep = GenarateFullDrawStep(DrawStep, 10);
                        break;
                    }
                case "C0632":
                    {
                        DrawStep = new int[] { 4, 5, 6, 1, 2, 3 };
                        DrawStep = GenarateFullDrawStep(DrawStep, 6);
                        break;
                    }
                case "C0933":
                    {
                        DrawStep = new int[] { 7, 8, 9, 4, 5, 6, 1, 2, 3 };
                        DrawStep = GenarateFullDrawStep(DrawStep, 9);
                        break;
                    }
                case "S0844":
                    {
                        //DrawStep = new int[] { 5, 6, 8, 7, 2, 1, 3, 4, 14, 13, 15, 16, 9, 10, 12, 11, 21, 22, 24, 23, 18, 17, 19, 20 };
                        DrawStep = new int[] { 5, 6, 8, 7, 2, 1, 3, 4, 14, 13, 15, 16, 9, 10, 12, 11 };
                        DrawStep = GenarateFullDrawStep(DrawStep, 16);
                        break;
                    }
            }
            return DrawStep;
        }

        private int[] GenarateFullDrawStep(int[] _DrawStep, int BDPerFloor)
        {
            int[] result = { };

            if (BDPerFloor == 0)
                return result;

            int OriginIndex = 0;

            if (BDS.Count > BDPerFloor)
            {
                int TotalFloor = (BDS.Count / BDPerFloor) + 1;
                int[] FullStep = new int[TotalFloor * BDPerFloor];
                for (int i = 0; i < FullStep.Length; i++)
                {
                    if (i < BDPerFloor)
                        FullStep[i] = _DrawStep[i];
                    else
                    {
                        int Floor = (i / BDPerFloor);
                        OriginIndex = (Floor > 1 ? (i - (BDPerFloor * Floor)) : (i - BDPerFloor));
                        FullStep[i] = _DrawStep[OriginIndex] + (BDPerFloor * Floor);
                    }
                }
                result = FullStep;
            }
            else
            {
                result = _DrawStep;
            }

            return result;
        }

        public void AddBundle(int _Floor, int _Ori, int _Column)
        {
            //if (Pattern == "I0532")
            //{
            //    BWidth = 50;
            //    BLength = 60;
            //    BHeight = 15;
            //}
            //else
            //{
            //    BWidth = 50;
            //    BLength = 70;
            //    BHeight = 20;
            //} 
            int LastNo = (BDS == null ? 0 : BDS.Count);
            Bundle3DModel NewBundle = new Bundle3DModel
            {
                No = LastNo + 1,
                Floor = _Floor,
                Width = BWidth,
                Length = BLength,
                Height = BHeight,
                Ori = _Ori,
                Column = _Column
            };
            BDS.Add(NewBundle);

            string PType = Pattern.Substring(0, 1);
            switch (PType)
            {
                case "C": { C_Pattern(Pattern); break; }
                case "S": { S_Pattern(Pattern); break; }
                case "I": { I_Pattern(Pattern); break; }
            }

        }

        public void AddBundlebyBundleNoOld(int BundleNo)
        {
            switch (Pattern)
            {
                case "C0111":
                    {
                        AddBundle(BundleNo, 0, 1);
                        break;
                    }
                case "C0221":
                    {
                        if (BDS.Count >= 2)
                        {
                            var CHK = (BDS.Count % 2);
                            var LastBundle = GetNearBundle(BundleNo - 1);
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            AddBundle(TargetFloor, 0, 1);
                        }
                        else
                        {
                            AddBundle(1, 0, 1);
                        }
                        break;
                    }
                case "C0331":
                    {
                        if (BDS.Count >= 3)
                        {
                            var CHK = (BDS.Count % 3);
                            var LastBundle = GetNearBundle(BundleNo - 1);
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            AddBundle(TargetFloor, 0, 1);
                        }
                        else
                        {
                            AddBundle(1, 0, 1);
                        }
                        break;
                    }
                case "C0441":
                    {
                        if (BDS.Count >= 4)
                        {
                            var CHK = (BDS.Count % 4);
                            var LastBundle = GetNearBundle(BundleNo - 1);
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            AddBundle(TargetFloor, 0, 1);
                        }
                        else
                        {
                            AddBundle(1, 0, 1);
                        }
                        break;
                    }
                case "S0422":
                    {
                        int[] OriStep = { 0, 1, 1, 0, 1, 0, 0, 1 };
                        int[] ColumnStep = { 1, 1, 2, 2 };
                        if (BDS.Count >= 4)
                        {
                            var CHK = (BDS.Count % 4);
                            var LastBundle = GetNearBundle(BundleNo - 1);
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            var IndexColumn = ((BundleNo % 4) - 1);
                            IndexColumn = (IndexColumn == -1 ? 3 : IndexColumn);
                            var TargetOri = ((TargetFloor % 2) == 1) ? OriStep[IndexColumn] : OriStep[IndexColumn + 4];
                            AddBundle(TargetFloor, TargetOri, ColumnStep[IndexColumn]);
                        }
                        else
                        {
                            AddBundle(1, OriStep[BundleNo - 1], ColumnStep[BundleNo - 1]);
                        }
                        break;
                    }
                case "I0321":
                    {
                        int[] OriStep = { 1, 1, 0, 0, 1, 1 };
                        int[] ColumnStep = { 1, 2, 1, 1, 1, 2 };
                        if (BDS.Count >= 3)
                        {
                            var CHK = (BDS.Count % 3);
                            var LastBundle = GetNearBundle(BundleNo - 1);
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            var IndexColumn = ((BundleNo % -3) - 1);
                            IndexColumn = (IndexColumn == -1 ? 2 : IndexColumn);
                            var TargetOri = ((TargetFloor % 2) == 1) ? OriStep[IndexColumn] : OriStep[IndexColumn + 3];
                            AddBundle(TargetFloor, TargetOri, ColumnStep[IndexColumn]);
                        }
                        else
                        {
                            AddBundle(1, OriStep[BundleNo - 1], ColumnStep[BundleNo - 1]);
                        }
                        break;
                    }
                case "I0532":
                    {
                        int[] OriStep = { 1, 1, 1, 0, 0, 0, 0, 1, 1, 1 };
                        int[] ColumnStep = { 1, 2, 3, 1, 2, 1, 2, 1, 2, 3 };
                        if (BDS.Count >= 5)
                        {
                            var CHK = (BDS.Count % 5);
                            var LastBundle = GetNearBundle(BundleNo - 1);
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            var IndexColumn = ((BundleNo % -5) - 1);
                            IndexColumn = (IndexColumn == -1 ? 4 : IndexColumn);
                            var TargetOri = ((TargetFloor % 2) == 1) ? OriStep[IndexColumn] : OriStep[IndexColumn + 5];
                            AddBundle(TargetFloor, TargetOri, ColumnStep[IndexColumn]);
                        }
                        else
                        {
                            AddBundle(1, OriStep[BundleNo - 1], ColumnStep[BundleNo - 1]);
                        }
                        break;
                    }
                case "S0844":
                    {
                        int[] OriStep = { 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0 };
                        int[] ColumnStep = { 1, 2, 1, 1, 3, 3, 2, 3, 1, 1, 1, 2, 2, 2, 3, 3 };
                        if (BDS.Count >= 16)
                        {
                            var CHK = (BDS.Count % 8);
                            var LastBundle = GetNearBundle(BundleNo - 1);
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            var IndexColumn = ((BundleNo % 8) - 1);
                            IndexColumn = (IndexColumn == -1 ? 7 : IndexColumn);
                            var TargetOri = ((TargetFloor % 2) == 1) ? OriStep[IndexColumn] : OriStep[IndexColumn + 8];
                            var TargetCol = ((TargetFloor % 2) == 1) ? ColumnStep[IndexColumn] : ColumnStep[IndexColumn + 8];
                            AddBundle(TargetFloor, TargetOri, TargetCol);
                        }
                        else
                        {
                            int _FL = (BDS.Count >= 8) ? 2 : 1;
                            AddBundle(_FL, OriStep[BundleNo - 1], ColumnStep[BundleNo - 1]);
                        }
                        break;
                    }
                case "C0422":
                    {
                        int[] ColumnStep = { 1, 1, 2, 2 };
                        if (BDS.Count >= 4)
                        {
                            var CHK = (BDS.Count % 4);
                            var LastBundle = BDS.Last();
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            var IndexColumn = ((BundleNo % 4) - 1);
                            IndexColumn = (IndexColumn == -1 ? 3 : IndexColumn);
                            AddBundle(TargetFloor, 0, ColumnStep[IndexColumn]);
                        }
                        else
                        {
                            AddBundle(1, 0, ColumnStep[BundleNo - 1]);
                        }
                        break;
                    }
                case "C0632":
                    {
                        int[] ColumnStep = { 1, 1, 1, 2, 2, 2 };
                        if (BDS.Count >= 6)
                        {
                            var CHK = (BDS.Count % 6);
                            var LastBundle = BDS.Last();
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            var IndexColumn = ((BundleNo % 6) - 1);
                            IndexColumn = (IndexColumn == -1 ? 5 : IndexColumn);
                            AddBundle(TargetFloor, 0, ColumnStep[IndexColumn]);
                        }
                        else
                        {
                            AddBundle(1, 0, ColumnStep[BundleNo - 1]);
                        }
                        break;
                    }
                case "C0933":
                    {
                        int[] ColumnStep = { 1, 1, 1, 2, 2, 2, 3, 3, 3 };
                        if (BDS.Count >= 9)
                        {
                            var CHK = (BDS.Count % 9);
                            var LastBundle = GetNearBundle(BundleNo - 1);
                            var TargetFloor = (CHK == 0) ? LastBundle.Floor + 1 : LastBundle.Floor;
                            var IndexColumn = ((BundleNo % 9) - 1);
                            IndexColumn = (IndexColumn == -1 ? 8 : IndexColumn);
                            AddBundle(TargetFloor, 0, ColumnStep[IndexColumn]);
                        }
                        else
                        {
                            AddBundle(1, 0, ColumnStep[BundleNo - 1]);
                        }
                        break;
                    }
            }
        }

        private void C_Pattern(string Pattern)
        {
            //--Get Floor List--
            var FL = (from ms in BDS
                      group ms by new { ms.Floor } into gp
                      select gp.FirstOrDefault()).ToList();

            int LastCol = 1;
            int LastY = 0;
            int LastX = 0;
            int XPos = 0;
            int YPos = 0;
            int TRow = 0;
            int TCol = 0;
            decimal OC_Factor = ((decimal)OC / 360);
            switch (Pattern)
            {
                case "C0111": { TRow = 1; TCol = 1; break; }
                case "C0221": { TRow = 2; TCol = 1; break; }
                case "C0331": { TRow = 3; TCol = 1; break; }
                case "C0441": { TRow = 4; TCol = 1; break; }
                case "C0422": { TRow = 2; TCol = 2; break; }
                case "C0632": { TRow = 3; TCol = 2; break; }
                case "C0933": { TRow = 3; TCol = 3; break; }

            }

            foreach (var F in FL)
            {
                int IRow = 1;
                int ICol = 1;
                var FLB = (from ms in BDS
                           where ms.Floor == F.Floor
                           select ms).ToList();

                int HeightPerFloor = ((F.Width * OC) / 100);
                //int HeightPerFloor = (F.Height - 2);
                int XIndent = (F.Width - (F.Length / 10));
                LastCol = 0;
                XPos = 0;
                foreach (var BD in FLB)
                {
                    //--Skip First Bundle Alway 0,0--
                    if (BD.No > 1)
                    {
                        if (TRow >= IRow && TRow > 1)
                        {
                            if (F.Floor == 1)
                            {
                                XPos += BD.Width - (BD.Length / 10);
                                YPos += ((BD.Width * OC) / 100);
                            }
                            else
                            {
                                if (TCol == 1)
                                {
                                    var _BT = GetNearBundle(BD.No - (TRow));
                                    YPos = _BT.Y;
                                    XPos = _BT.X;
                                    if (TRow > 2)
                                        YPos = LastY + HeightPerFloor;
                                }
                                else
                                {
                                    XPos = XIndent * (IRow - 1);
                                    YPos = LastY + HeightPerFloor;
                                }

                                if (TRow == IRow)
                                {
                                    YPos = LastY + HeightPerFloor;
                                    if (TCol > 1 && ICol > 1)
                                        XPos = XIndent * (TCol - 1);
                                }


                                if (IRow == 1 && ICol == 1)
                                    XPos = 0;
                            }
                            BD.X = XPos;
                            BD.Y = YPos;
                        }
                        else
                        {
                            BD.Y = -YPos;
                        }


                        if (BD.Column > 1)
                        {
                            if (ICol == 1)
                                XPos = 0;

                            XPos += BD.Width - (BD.Length / 10);
                            if (LastCol == BD.Column)
                            {
                                XPos = XIndent * ICol;
                                if (LastCol == 3)
                                    XPos = XIndent * (ICol - 2);
                                YPos += HeightPerFloor;
                            }
                            else
                            {
                                if (TRow > 2)
                                    YPos = -LastY;
                                else
                                    YPos = -LastY * 2;

                                if (BD.Floor > 1)
                                    YPos = -(HeightPerFloor * (BD.Floor + 1));

                                //--C0933--
                                if (TCol == 3 && ICol == 4)
                                {
                                    XPos = XIndent * (TCol - 1);
                                    YPos = -(HeightPerFloor * (TCol + BD.Floor));
                                }
                            }
                            BD.X = XPos;
                            BD.Y = YPos;
                            ICol++;
                        }

                        if (IRow == 1 && TRow > 1)
                        {
                            BD.Y = -(HeightPerFloor * (F.Floor - 1));
                        }

                        LastY = BD.Y;
                        LastX = BD.X;
                        LastCol = BD.Column;
                    }
                    IRow++;

                    string _Temp = string.Format("[{6}]Width:{0} Height:{1} Lendth:{4} X:{2} Y:{3}{5}", BD.Width, BD.Height, BD.X, BD.Y, BD.Length, Environment.NewLine, BD.No);
                    //txbRawData.Text += _Temp;
                }
                YPos += HeightPerFloor;
            }
        }

        private void S_Pattern(string Pattern)
        {
            //--Update Placement--
            var FL = (from ms in BDS
                      group ms by new { ms.Floor } into gp
                      select gp.FirstOrDefault()).ToList();

            int YFirstFloor = 0;
            int LastY = 0;
            int SpaceX = 3;
            int SpaceY = 2;
            //int GapY = 2;
            decimal OC_Factor = ((decimal)OC / 360) * 100;

            if (Pattern == "S0422")
            {
                foreach (var item in FL)
                {
                    int XPos = 0;
                    int YPos = 0;
                    var FLB = (from ms in BDS
                               where ms.Floor == item.Floor
                               select ms).ToList();

                    if (item.Floor > 1)
                    {
                        //YPos = LastY;
                        var Near = GetNearBundle(item.No - 2);
                        //YPos = -(item.Height * (36/100));
                        YPos = -((Near.Height / 2) + 1);
                        XPos = 2;
                    }

                    if (item.Floor == 2)
                        YFirstFloor = LastY;

                    int Index = 0;
                    int LastX = 0;
                    LastY = 0;
                    int LastColumn = 0;
                    int LastOri = 0;
                    int Factor = -38;
                    foreach (var BD in FLB)
                    {
                        if (BD.Floor > 2)
                        {
                            XPos = (GetNearBundle(BD.No - 8).X);
                            YPos = (GetNearBundle(BD.No - 8).Y) + Factor;
                        }
                        else
                        {
                            if (Index > 0)
                            {
                                if (BD.Ori == 0 || BD.Ori == 2)
                                {
                                    XPos += BD.Width - (BD.Length / 10);
                                    YPos += ((BD.Width * OC) / 100);
                                }
                                else
                                {
                                    XPos += BD.Width - (BD.Length / 10);
                                    YPos += ((BD.Length * OC) / 100);
                                }

                                if (BD.Floor % 2 == 0 && BD.Floor > 1)
                                {
                                    XPos += 12;
                                    YPos += -2;
                                }

                                YPos += SpaceY;
                                XPos += SpaceX;
                            }

                            if (BD.Column > 1)
                            {
                                XPos = LastX;
                                if (LastColumn == BD.Column)
                                {
                                    XPos = LastX * 2;
                                    YPos = (GetNearBundle(BD.No - 3).Y);
                                    if (BD.Floor % 2 == 1)
                                    {
                                        YPos += -2;
                                    }

                                    if (BD.Floor == 2)
                                    {
                                        XPos = 88;
                                        YPos = -28;
                                    }
                                }
                                else
                                {
                                    if (BD.Ori == 0 || BD.Ori == 2)
                                        YPos = -(LastY * 2);
                                    else
                                        YPos = -LastY;

                                    if (BD.Floor == 2)
                                    {
                                        XPos = LastX / 2;
                                        YPos = -(BD.Width - (BD.Length / 10));
                                    }
                                }
                            }
                        }

                        BD.X = XPos;
                        BD.Y = YPos;
                        LastX = XPos;
                        LastY = YPos;
                        LastColumn = BD.Column;
                        LastOri = BD.Ori;
                        Index++;
                        string _Temp = string.Format("[{6}]Width:{0} Height:{1} Lendth:{4} X:{2} Y:{3}{5}", BD.Width, BD.Height, BD.X, BD.Y, BD.Length, Environment.NewLine, BD.No);
                        //txbRawData.Text += _Temp;
                    }
                }
            }
            else
            {
                int LastX = 0;
                int LastOri = -1;
                int LastCol = -1;
                int[] XIdents = { (BWidth - BHeight), (BLength - (BWidth - BHeight)) };
                int[] XGap = { 2, 8, 4 };
                int[] YGap = { -26, 18 };
                int[] HFloor = { 37, BHeight };

                foreach (var item in FL)
                {
                    int XPos = 0;
                    int YPos = 0;
                    var FLB = (from ms in BDS
                               where ms.Floor == item.Floor
                               select ms).ToList();

                    LastOri = -1;
                    LastCol = -1;
                    foreach (var BD in FLB)
                    {
                        if (BD.No > 1)
                        {
                            if (BD.Floor <= 2)
                            {
                                if (BD.Floor == 1)
                                {
                                    switch (BD.No)
                                    {
                                        case 2:
                                            {
                                                XPos = XIdents[0] + XGap[0];
                                                YPos = YGap[0];
                                                break;
                                            }
                                        case 3:
                                            {
                                                XPos = BWidth + XGap[1];
                                                YPos = YGap[1];
                                                break;
                                            }
                                        case 4:
                                            {
                                                XPos = (BWidth * 2) + XGap[0];
                                                YPos = YGap[1] * 2;
                                                break;
                                            }
                                        case 5:
                                            {
                                                var BDNear = GetNearBundle(BD.No - 2);
                                                XPos = BDNear.X + XGap[2];
                                                YPos = -(XIdents[0] * 2);
                                                break;
                                            }
                                        case 6:
                                            {
                                                var BDNear = GetNearBundle(BD.No - 2);
                                                XPos = BDNear.X + XGap[2];
                                                YPos = LastY + YGap[1];
                                                break;
                                            }
                                        case 7:
                                            {
                                                var BDNear = GetNearBundle(BD.No - 2);
                                                XPos = (BWidth * BD.Column) + XIdents[0] + XGap[0];
                                                YPos = 0;
                                                break;
                                            }
                                        case 8:
                                            {
                                                var BDNear = GetNearBundle(BD.No - 2);
                                                XPos = LastX + XIdents[0] + XGap[0];
                                                YPos = YGap[0];
                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    switch (BD.No)
                                    {
                                        case 9:
                                            {
                                                XPos = -(XGap[0]);
                                                YPos = YGap[0];
                                                break;
                                            }
                                        case 10:
                                            {
                                                XPos = XIdents[1] + XGap[0];
                                                YPos = LastY + YGap[1];
                                                break;
                                            }
                                        case 11:
                                            {
                                                XPos = (BD.Width * 2) - (XGap[1] + XGap[2]);
                                                YPos = YGap[1];
                                                break;
                                            }
                                        case 12:
                                            {
                                                XPos = LastX + XIdents[0];
                                                YPos = LastY + YGap[0];
                                                break;
                                            }
                                        case 13:
                                            {
                                                XPos = XIdents[1] + XGap[0];
                                                YPos = YGap[0] * 2;
                                                break;
                                            }
                                        case 14:
                                            {
                                                XPos = LastX + XIdents[0];
                                                YPos = LastY + YGap[0];
                                                break;
                                            }
                                        case 15:
                                            {
                                                XPos = XIdents[1] * BD.Column;
                                                YPos = -(YGap[1] * BD.Column);
                                                break;
                                            }
                                        case 16:
                                            {
                                                XPos = LastX + XIdents[1] + XGap[2];
                                                YPos = LastY + YGap[1];
                                                break;
                                            }
                                    }
                                }
                                BD.X = XPos;
                                BD.Y = YPos;
                            }
                            else
                            {
                                if (BD.Floor % 2 == 1)
                                {
                                    var BDFirst = GetNearBundle(BD.No - 16);
                                    XPos = BDFirst.X;
                                    if (BD.Ori != LastOri || BD.Column != LastCol)
                                        //YPos = BDFirst.Y - (HFloor[0] * (ODDFloor));
                                        YPos = BDFirst.Y - HFloor[0];
                                    else
                                    {
                                        var YGapIndex = (BD.Ori == 0 || BD.Ori == 2) ? 1 : 0;
                                        YPos = LastY + YGap[YGapIndex];
                                    }
                                }
                                else
                                {
                                    var BDFirst = GetNearBundle(BD.No - 16);
                                    XPos = BDFirst.X;
                                    if (BD.Ori != LastOri || BD.Column != LastCol)
                                        //YPos = BDFirst.Y - (HFloor[0] * (EVEFloor));
                                        YPos = BDFirst.Y - HFloor[0];
                                    else
                                    {
                                        var YGapIndex = (BD.Ori == 0 || BD.Ori == 2) ? 1 : 0;
                                        YPos = LastY + YGap[YGapIndex];
                                    }

                                    //--Special--
                                    if ((BD.Floor * 8) - BD.No == 3)
                                    {
                                        YPos = BDFirst.Y - HFloor[0];
                                    }
                                }
                                BD.X = XPos;
                                BD.Y = YPos;
                            }
                        }
                        else
                        {
                            BD.X = 0;
                            BD.Y = 0;
                        }
                        LastX = BD.X;
                        LastY = BD.Y;
                        LastOri = BD.Ori;
                        LastCol = BD.Column;
                    }

                }
            }
        }

        private void I_Pattern(string Pattern)
        {

            switch (Pattern)
            {
                case "I0321": { PatternI0321(); break; }
                case "I0532": { PatternI0532(); break; }
            }


            void PatternI0321()
            {
                var FL = (from ms in BDS
                          group ms by new { ms.Floor } into gp
                          select gp.FirstOrDefault()).ToList();
                int[] LastY1 = { 0, 0, 0 };
                int[] LastY2 = { 0, 0, 0 };
                int Index = 0;

                foreach (var item in FL)
                {
                    int XPos = 0;
                    int YPos = 0;
                    int[] XPosFix1 = { 0, 30, 65 };
                    int[] XPosFix2 = { 10, 45, 75 };
                    int[] YPosFix1 = { 0, -25, 8 };
                    int[] YPosFix2 = { -32, 0, -25 };

                    var FLB = (from ms in BDS
                               where ms.Floor == item.Floor
                               select ms).ToList();

                    //LastY = 0;
                    int LastColumn = 0;
                    int LastOri = 0;

                    foreach (var BD in FLB)
                    {

                        if (item.Floor % 2 == 1)
                        {
                            if (Index > 2)
                            {
                                XPos = XPosFix1[(Index) % 3];
                                YPos = LastY1[(Index) % 3] - 37;

                                switch ((Index) % 3)
                                {
                                    case 0: { LastY1[0] = YPos; break; }
                                    case 1: { LastY1[1] = YPos; break; }
                                    case 2: { LastY1[2] = YPos; break; }
                                }
                            }

                            else
                            {
                                XPos = XPosFix1[(Index) % 3];
                                YPos = YPosFix1[(Index) % 3];

                                switch ((Index) % 3)
                                {
                                    case 0: { LastY1[0] = YPosFix1[(Index) % 3]; break; }
                                    case 1: { LastY1[1] = YPosFix1[(Index) % 3]; break; }
                                    case 2: { LastY1[2] = YPosFix1[(Index) % 3]; break; }
                                }
                            }

                        }
                        else
                        {
                            if (Index > 5)
                            {
                                XPos = XPosFix2[(Index) % 3];
                                YPos = LastY2[(Index) % 3] - 37;

                                switch ((Index) % 3)
                                {
                                    case 0: { LastY2[0] = YPos; break; }
                                    case 1: { LastY2[1] = YPos; break; }
                                    case 2: { LastY2[2] = YPos; break; }
                                }
                            }

                            else
                            {
                                XPos = XPosFix2[(Index) % 3];
                                YPos = YPosFix2[(Index) % 3];

                                switch ((Index) % 3)
                                {
                                    case 0: { LastY2[0] = YPosFix2[(Index) % 3]; break; }
                                    case 1: { LastY2[1] = YPosFix2[(Index) % 3]; break; }
                                    case 2: { LastY2[2] = YPosFix2[(Index) % 3]; break; }
                                }
                            }
                        }


                        BD.X = XPos;
                        BD.Y = YPos;
                        //LastX = XPos;
                        //LastY = YPos;
                        LastColumn = BD.Column;
                        LastOri = BD.Ori;
                        Index++;
                        string _Temp = string.Format("[{6}]Width:{0} Height:{1} Lendth:{4} X:{2} Y:{3}{5}", BD.Width, BD.Height, BD.X, BD.Y, BD.Length, Environment.NewLine, BD.No);
                        //txbRawData.Text += _Temp;
                    }
                }
            }


            void PatternI0532()
            {
                var FL = (from ms in BDS
                          group ms by new { ms.Floor } into gp
                          select gp.FirstOrDefault()).ToList();
                int[] LastY1 = { 0, 0, 0, 0, 0 };
                int[] LastY2 = { 0, 0, 0, 0, 0 };
                int Index = 0;

                foreach (var item in FL)
                {
                    int XPos = 0;
                    int YPos = 0;
                    int[] XPosFix1 = { 0, 30, 60, 50, 100 };
                    int[] XPosFix2 = { 0, 50, 40, 70, 100 };
                    int[] YPosFix1 = { 0, -25, -50, 15, -25 };
                    int[] YPosFix2 = { -20, -60, 5, -20, -45 };

                    var FLB = (from ms in BDS
                               where ms.Floor == item.Floor
                               select ms).ToList();

                    //LastY = 0;
                    int LastColumn = 0;
                    int LastOri = 0;

                    foreach (var BD in FLB)
                    {

                        if (item.Floor % 2 == 1)
                        {
                            if (Index > 4)
                            {
                                XPos = XPosFix1[(Index) % 5];
                                YPos = LastY1[(Index) % 5] - 25;

                                switch ((Index) % 5)
                                {
                                    case 0: { LastY1[0] = YPos; break; }
                                    case 1: { LastY1[1] = YPos; break; }
                                    case 2: { LastY1[2] = YPos; break; }
                                    case 3: { LastY1[3] = YPos; break; }
                                    case 4: { LastY1[4] = YPos; break; }
                                }
                            }

                            else
                            {
                                XPos = XPosFix1[(Index) % 5];
                                YPos = YPosFix1[(Index) % 5];

                                switch ((Index) % 5)
                                {
                                    case 0: { LastY1[0] = YPosFix1[(Index) % 5]; break; }
                                    case 1: { LastY1[1] = YPosFix1[(Index) % 5]; break; }
                                    case 2: { LastY1[2] = YPosFix1[(Index) % 5]; break; }
                                    case 3: { LastY1[3] = YPosFix1[(Index) % 5]; break; }
                                    case 4: { LastY1[4] = YPosFix1[(Index) % 5]; break; }

                                }
                            }

                        }
                        else
                        {
                            if (Index > 10)
                            {
                                XPos = XPosFix2[(Index) % 5];
                                YPos = LastY2[(Index) % 5] - 25;

                                switch ((Index) % 5)
                                {
                                    case 0: { LastY2[0] = YPos; break; }
                                    case 1: { LastY2[1] = YPos; break; }
                                    case 2: { LastY2[2] = YPos; break; }
                                    case 3: { LastY2[3] = YPos; break; }
                                    case 4: { LastY2[4] = YPos; break; }
                                }
                            }

                            else
                            {
                                XPos = XPosFix2[(Index) % 5];
                                YPos = YPosFix2[(Index) % 5];

                                switch ((Index) % 5)
                                {
                                    case 0: { LastY2[0] = YPosFix2[(Index) % 5]; break; }
                                    case 1: { LastY2[1] = YPosFix2[(Index) % 5]; break; }
                                    case 2: { LastY2[2] = YPosFix2[(Index) % 5]; break; }
                                    case 3: { LastY2[3] = YPosFix2[(Index) % 5]; break; }
                                    case 4: { LastY2[4] = YPosFix2[(Index) % 5]; break; }
                                }
                            }
                        }


                        BD.X = XPos;
                        BD.Y = YPos;
                        //LastX = XPos;
                        //LastY = YPos;
                        LastColumn = BD.Column;
                        LastOri = BD.Ori;
                        Index++;
                        string _Temp = string.Format("[{6}]Width:{0} Height:{1} Lendth:{4} X:{2} Y:{3}{5}", BD.Width, BD.Height, BD.X, BD.Y, BD.Length, Environment.NewLine, BD.No);
                        //txbRawData.Text += _Temp;
                    }
                }
            }
        }

        private Bundle3DModel GetNearBundle(int No)
        {
            var BN = (from ms in BDS
                      where ms.No == No
                      select ms).FirstOrDefault();
            return BN;
        }

        public void ClearPallet()
        {
            BDS = new List<Bundle3DModel>();
        }
    }

    public class Bundle3DModel
    {
        public int No { get; set; }
        public int Floor { get; set; }
        public int Width { get; set; }
        public int Length { get; set; } //Depth
        public int Height { get; set; }
        public bool Rotate { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Ori { get; set; }
        public int Column { get; set; }
    }
}
