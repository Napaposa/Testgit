using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_ID4P.Class
{
    public class Pattern3DModel
    {
        public string Pattern { get; set; }
        public int YStep1 { get; set; }
        public int YStep2 { get; set; }
        public List<Bundle3DModel> BDS { get; set; }
        public int[] OrderStep { get; set; }
    }
}
