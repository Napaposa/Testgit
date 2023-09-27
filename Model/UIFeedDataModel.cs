using ATD_ID4P.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATD_ID4P.Model
{
    public class UIFeedDataModel
    {
        public string MaterialNo { get; set; }
        public int OrderBundle_Count { get; set; }
        public int Pallet_Count { get; set; }
        public int SO_Count { get; set; }
        public int Step_Count { get; set; }
        public int Layer_Count { get; set; }
        public int Pattern_Count { get; set; }
        public int LastBD_PrvPallet { get; set; }
        public int LastBD_PrvSO { get; set; }
        public int PrvPallet_Count { get; set; }
        public int LastBD_B4PrvPallet { get; set; }
        public List<int> PalletBD_Count { get; set; } = new List<int>();
        public int SOBD_Count { get; set; }
        public int TieSheet_Width { get; set; }
        public int TieSheet_Length { get; set; }

        public void Reset_FeedData()
        {
            MaterialNo = "";
            OrderBundle_Count = 0;
            Pallet_Count = 1;
            SO_Count = 1;
            Step_Count = 0;
            Layer_Count = 0;
            Pattern_Count = 0;

            LastBD_PrvPallet = 0;
            LastBD_PrvSO = 0;

            PrvPallet_Count = 1;
            LastBD_B4PrvPallet = 0;
            if (PalletBD_Count.Count > 0)
            {
                // Remove all the element from the list except the first one
                PalletBD_Count.RemoveRange(1, PalletBD_Count.Count - 1);
                PalletBD_Count[0] = 0;
            }
            else
            {
                // If PalletBD_Count don't have element -> initial with element 0
                PalletBD_Count.Add(0);
            }
            SOBD_Count = 0;
            TieSheet_Width = 0;
            TieSheet_Length = 0;
        }

        public void Check_PalletComplete()
        {
            if (Pallet_Count != PrvPallet_Count)
            {
                int buf_PalletBD = LastBD_PrvPallet - LastBD_B4PrvPallet;
                if (PalletBD_Count.Count < PrvPallet_Count)
                {
                    PalletBD_Count.Add(buf_PalletBD);
                }
                PalletBD_Count[PrvPallet_Count - 1] = buf_PalletBD;
                PrvPallet_Count = Pallet_Count;
                LastBD_B4PrvPallet = LastBD_PrvPallet;
            }
            if (PalletBD_Count.Count < Pallet_Count)
                PalletBD_Count.Add(0);
        }

        public UIFeedDataModel TestValue(UIFeedDataModel FeedValue, OrderModel test_odm, PatternModel test_ptm)
        {
            int test_LYPerPallet = test_odm.LayerPerPallet;
            int test_BDPerLY = test_odm.BundlePerLayer;
            int test_SWPat1 = test_ptm.SwitchPattern[2] - '0';
            int test_SWPat2 = test_ptm.SwitchPattern[4] - '0';
            int test_SWPat3 = test_ptm.SwitchPattern[6] - '0';
            int test_SWPat4 = test_ptm.SwitchPattern[8] - '0';
            int test_PatAmount = test_SWPat1 + test_SWPat2 + test_SWPat3 + test_SWPat4;
            UIFeedDataModel result = new UIFeedDataModel();
            result = FeedValue;
            if (result.Pallet_Count < 1)
            {
                result.Pallet_Count = 1;
                result.PrvPallet_Count = 1;
            }

            try
            {
                result.MaterialNo = result.MaterialNo;
                result.OrderBundle_Count = result.OrderBundle_Count + 1;
                if (result.Step_Count == test_BDPerLY)
                {
                    result.Step_Count = 1;
                    if (result.Layer_Count == test_LYPerPallet)
                    {
                        result.Layer_Count = 1;
                        result.Pattern_Count = result.Pattern_Count + 1;
                        result.Pallet_Count = result.Pallet_Count + 1;
                        result.LastBD_PrvPallet = result.OrderBundle_Count;
                    }
                    else
                    {
                        result.Layer_Count = result.Layer_Count + 1;
                        if (result.Pattern_Count == test_PatAmount)
                        {
                            result.Pattern_Count = 1;
                        }
                        else
                        {
                            result.Pattern_Count = result.Pattern_Count + 1;
                        }
                    }
                }
                else
                {
                    result.Step_Count = result.Step_Count + 1;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Test Value Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }
    }
}
