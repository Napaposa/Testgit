using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_ID4P.Model
{
    public class LotEndFeedDataModel
    {
        public double SheetHeight { get; set; }
        public int HeightRatio { get; set; }
        public int BottomSheet { get; set; }
        public bool TopSheet { get; set; }
        public bool ExtraPickDepth { get; set; }
        public bool SQExtra { get; set; }
        public bool STKAntiBounce { get; set; }
        public int PlacingMode { get; set; }
        public int DischargeMode { get; set; }
        public int AmountOfPlacedBundles { get; set; }

    }
}
