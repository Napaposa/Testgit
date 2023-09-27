using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_ID4P.Class
{
    public class palletsCls
    {

        private int pallLeng = 1200;
        private int pallWide = 1000;
        private int pallHigh = 115;
        private string pallName;
        private bool pallTurn;
        private string pallDirection;

        [CategoryAttribute("Appearance"), DescriptionAttribute("Pallet name"), ReadOnly(false)]
        public string Name { get { return pallName; } set { pallName = value; } }//Name of pattern Exm S0422
        [CategoryAttribute("Appearance"), DescriptionAttribute("Degree of turning pattern 0,90"), ReadOnly(false)]
        public bool Turn { get { return pallTurn; } set { pallTurn = value; } }
        [CategoryAttribute("Appearance"), DescriptionAttribute("Direection to load pallet L=Leng, W=Wide"), ReadOnly(false)]
        public string Direction { get { return pallDirection; } set { pallDirection = value; } }

        [CategoryAttribute("Dimension"), DescriptionAttribute("Leng side (mm.)"), ReadOnly(false)]
        public int Length { get { return pallLeng; } set { pallLeng = value; } } //Total leng size
        [CategoryAttribute("Dimension"), DescriptionAttribute("Wide side (mm.)"), ReadOnly(false)]
        public int Width { get { return pallWide; } set { pallWide = value; } } //Total wide size
        [CategoryAttribute("Dimension"), DescriptionAttribute("High of pallet (mm.)"), ReadOnly(false)]
        public int Height { get { return pallHigh; } set { pallHigh = value; } }


    }
}