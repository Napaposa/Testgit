using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_ID4P.Model
{
    public class OrdersBundleModel
    {
        private int MaximumOrder = 20;

        public List<OrderBundleModel> OBs { get; set; }

        public int[] GetOrderBundleArray()
        {
            int[] result = new int[MaximumOrder];

            int i = 0;
            foreach (OrderBundleModel OB in OBs)
            {
                result[i] = OB.AmountBundle;
                i++;
            }

            return result;
        }

        public string[] GetOrderSOArray()
        {
            string[] result = new string[MaximumOrder];

            int i = 0;
            foreach (OrderBundleModel OB in OBs)
            {
                result[i] = OB.SONo;
                i++;
            }

            return result;
        }

        public string GetOrderBundleArrayText()
        {
            string result = "";

            result = string.Join(",", GetOrderBundleArray());

            return result;
        }

        public string GetOrderSheetText()
        {
            string result = "";

            result = string.Join(",", OBs.Select(x => x.AmountSheet).ToArray());

            return result;
        }

        public string GetSOText()
        {
            string result = "";

            result = string.Join(",", OBs.Select(x => x.SONo).ToArray());

            return result;
        }

        public List<OrderBundleModel> GetOrderBundlefromString(string SheetPerSO, int SheetPerBundle, string SOID)
        {
            OBs = new List<OrderBundleModel>();

            // Check SheetPerSO Text and Split into an Array of String
            SheetPerSO = string.IsNullOrEmpty(SheetPerSO) ? "0" : SheetPerSO;
            string[] Arr_SheetPerSO = SheetPerSO.Split(',');

            // Check SOID Text and Split into an Array of String
            SOID = string.IsNullOrEmpty(SOID) ? "0" : SOID;
            string[] Arr_SOID = SOID.Split(',');

            for (int i = 0; i < Arr_SheetPerSO.Length; i++)
            {
                string buf_SOID = "";
                if (Arr_SOID.Length > i)
                    buf_SOID = Arr_SOID[i];

                OrderBundleModel OB = new OrderBundleModel()
                {
                    No = (i + 1),
                    AmountSheet = Convert.ToInt32(Arr_SheetPerSO[i]),
                    SheetPerBundle = SheetPerBundle,
                    SONo = buf_SOID
                };
                OB.GetAmountBundle();
                OBs.Add(OB);
            }
            return OBs;
        }
    }

    public class OrderBundleModel
    {
        [ReadOnly(true)]
        public int No { get; set; }

        [TypeConverter(typeof(CustomNumberTypeConverter))]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0}")]
        public int AmountSheet { get; set; }
        [DisplayName("Bundles")]
        [ReadOnly(true)]
        [TypeConverter(typeof(CustomNumberTypeConverter))]
        public int AmountBundle { get; set; }
        [Browsable(false)]
        public int SheetPerBundle { get; set; }
        public string SONo { get; set; }

        public int GetAmountBundle()
        {
            AmountBundle = (AmountSheet / SheetPerBundle);
            return AmountBundle;
        }
    }

    //public class DisplayFormatAttribute : Attribute
    //{
    //    public DisplayFormatAttribute(string format)
    //    {
    //        Format = format;
    //    }
    //    public string Format { get; set; }
    //}

    public class CustomNumberTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                            Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string)
            {
                string s = (string)value;
                return Int32.Parse(s, NumberStyles.AllowThousands, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((int)value).ToString("N0", culture);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

}
