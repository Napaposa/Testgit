using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATD_ID4P.Class;

namespace ATD_ID4P.Model
{
    public class RBWorkingModel
    {

        private LogCls Log = new LogCls();

        public List<RBRunningModel> RunData { get; set; }

        public RBRunningModel FindRunningData_ByStartTime(DateTime RunTime)
        {
            RBRunningModel result = new RBRunningModel(); ;

            if (RunData != null && RunData.Count > 0)
            {
                var item = RunData.Where(element => element.StartTime.Value == RunTime).FirstOrDefault();
                if (item != null)
                    result = item;
            }

            return result;
        }

        public void SaveUsedTimeLastPallet(DateTime EndTime, string TotalPlaceBundle)
        {
            if (RunData != null && RunData.Count > 0)
            {
                var item = RunData.Where(element => element.StartTime.Value <= EndTime && element.EndTime == null).FirstOrDefault();
                if (item != null)
                {
                    //int LastPalletNo = 0;

                    foreach (var RBRunningModel in RunData)
                    {
                        if (RBRunningModel.MaterialNo == item.MaterialNo)
                        {
                            RBRunningModel.TotalPlaceBundle = Convert.ToInt32(TotalPlaceBundle);
                            RBRunningModel.EndTime = EndTime;
                            RBRunningModel.UsedTime = RBRunningModel.GetUsedTime();
                            break;
                        }
                    }
                    //item.PalletNo = LastPalletNo + 1;
                }
            }
        }

        public decimal GetAvgUsedTime()
        {
            decimal result = 0;

            try
            {
                if (RunData != null && RunData.Count > 0)
                {
                    //result = RunData.Average(x => x.UsedTime);
                    int InTargetPallet = 0;
                    decimal SumUsedTime = 0;
                    foreach (var item in RunData)
                    {
                        int TargetBundle = item.PalletNo * (item.BundlePerLayer * item.LayerPerPallet);
                        if (item.TotalPlaceBundle >= TargetBundle && item.UsedTime < 15)
                        {
                            SumUsedTime += item.UsedTime;
                            InTargetPallet++;
                        }
                    }
                    result = (SumUsedTime / InTargetPallet);
                    result = ConvertDecimalMinuteToRealMinute(result);
                }
            }
            catch (Exception)
            {

            }

            return result;
        }

        public decimal GetTotalPallet()
        {
            decimal result = 0;

            try
            {
                if (RunData != null && RunData.Count > 0)
                    result = RunData.Count();
            }
            catch (Exception)
            {

            }

            return result;
        }

        public int GetTotalSheet(int SheetPerBundle, double BundleAmount)
        {
            int result = SheetPerBundle * Convert.ToInt32(BundleAmount);

            return result;
        }

        public decimal GetActualSpeed(decimal UsedMinute, decimal SheetPerPallet)
        {
            decimal result = 0;

            try
            {
                if (UsedMinute > 0 && SheetPerPallet > 0)
                    result = SheetPerPallet / UsedMinute;
            }
            catch (Exception)
            {

            }

            return result;
        }

        public decimal GetActualSpeedinPalletNo(int PalletNo)
        {
            decimal result = 0;

            try
            {
                var item = RunData.Where(x => x.PalletNo == PalletNo).FirstOrDefault();
                if (item != null)
                {
                    decimal TotalSheetOnPallet = (item.TotalPlaceBundle * item.SheetPerBundle) - ((item.GetTotalSheetPerPallet() * (item.PalletNo - 1)));
                    result = GetActualSpeed(item.UsedTime, TotalSheetOnPallet);
                    result = Math.Round(result, 0);
                }
            }
            catch (Exception)
            {

            }

            return result;
        }

        public decimal GetTotalWorkingTime()
        {
            decimal result = 0;

            try
            {
                if (RunData != null && RunData.Count > 0)
                {
                    result = RunData.Sum(x => x.UsedTime);
                    result = ConvertDecimalMinuteToRealMinute(result);
                }
            }
            catch (Exception)
            {

            }

            return result;
        }

        public DateTime GetFirstPlaceBundleTime()
        {
            DateTime result = DateTime.Now;

            try
            {
                if (RunData != null && RunData.Count > 0)
                {
                    var item = RunData.OrderBy(x => x.StartTime).FirstOrDefault();
                    result = item.StartTime.Value;
                }
            }
            catch (Exception)
            {

            }

            return result;
        }

        public void RemoveStopTime()
        {
            // Pick RunData with the the latest StartTime
            // if UsedTime is 0 (Start but never finish)
            //  Delete it
            if (RunData != null && RunData.Count > 0)
            {
                var item = RunData.OrderByDescending(element => element.StartTime).FirstOrDefault();
                if (item != null)
                {
                    if (item.UsedTime == 0)
                        RunData.Remove(item);
                }
                
            }
        }

        public decimal ConvertDecimalMinuteToRealMinute(decimal Min)
        {
            decimal result = 0;

            try
            {
                decimal Integer = Math.Truncate(Min);
                decimal Fraction = Min - Integer;
                decimal RealSecond = (Fraction * 60) / 100;

                result = Integer + RealSecond;
            }
            catch (Exception)
            {

            }

            return result;
        }

        public void UpdatePalletTimeLog()
        {
            try
            {
                if (RunData.Count == 0 || RunData == null)
                    return;

                string _PTimeLog = "";
                foreach (RBRunningModel item in RunData)
                {
                    _PTimeLog += string.Format("[Material:{4}, Pallet:{0}, Start:{1:yyyy-MM-dd_HHmmss}, End:{2:yyyy-MM-dd_HHmmss}, UseTime:{3}], ", item.PalletNo, item.StartTime.Value, item.EndTime.Value, item.UsedTime, item.MaterialNo);
                }
                Log.WriteLog(_PTimeLog, LogType.Success);
            }
            catch (Exception)
            {
                Log.WriteLog("Can't write Pallet Time", LogType.Fail);
            }
        }

        public int GetBundlePerPallet()
        {
            int BDPP = 0;

            if (RunData != null && RunData.Count > 0)
            {
                BDPP = RunData[0].BundlePerLayer * RunData[0].LayerPerPallet;
            }

            return BDPP;
        }

        public int GetPlacedBundlePercentinPalletNo(int PalletNo)
        {
            int result = 0;

            var item = RunData.Where(x => x.PalletNo == PalletNo).FirstOrDefault();
            if (item != null)
            {
                result = ((item.TotalPlaceBundle * 100) / GetBundlePerPallet());
            }

            return result;
        }

        public int GetPlacedBundleinPalletNo(int PalletNo)
        {
            int result = 0;

            var item = RunData.Where(x => x.PalletNo == PalletNo).FirstOrDefault();
            if (item != null)
            {
                result = item.TotalPlaceBundle;
            }

            return result;
        }

        public int GetTotalPlacedBundle()
        {
            int result = 0;
            try
            {
                result = RunData.Sum(x => x.TotalPlaceBundle);
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }

        public int GetTotalPlacedBundlewithoutPalletNo(int PalletNo)
        {
            int result = 0;

            try
            {
                result = RunData.Where(x => x.PalletNo != PalletNo).Sum(x => x.TotalPlaceBundle);
            }
            catch (Exception)
            {
                result = 0;
            }

            return result;
        }

        public decimal GetPredicateSpeed(int PalletNo, decimal UsedTime, int PlacedBundle)
        {
            decimal result = 0;

            try
            {
                var item = RunData.Where(x => x.PalletNo == PalletNo).FirstOrDefault();
                if (item != null)
                {
                    decimal TotalSheetPerPallet = item.GetTotalSheetPerPallet();
                    PalletNo = (PalletNo == 0 && RunData.Count > 0 ? RunData.Count() : PalletNo);
                    decimal LastTotalSheet = (PalletNo <= 1 ? 0 : (TotalSheetPerPallet * (PalletNo - 1)));
                    decimal TotalSheetOnPallet = (PlacedBundle * item.SheetPerBundle) - LastTotalSheet;
                    decimal UseTimePerSheet = UsedTime / TotalSheetOnPallet;
                    decimal TotalUsedTime = UseTimePerSheet * TotalSheetPerPallet;
                    TotalUsedTime = ConvertDecimalMinuteToRealMinute(TotalUsedTime);
                    result = Math.Round(GetActualSpeed(TotalUsedTime, TotalSheetPerPallet), 2);
                }
            }
            catch (Exception)
            {

            }

            return result;
        }
    }


    public class RBRunningModel
    {
        public String MaterialNo { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal UsedTime { get; set; }
        public int PalletNo { get; set; }
        public int TotalPlaceBundle { get; set; }
        public int SheetPerBundle { get; set; }
        public int BundlePerLayer { get; set; }
        public int LayerPerPallet { get; set; }
        public bool LabelStamp { get; set; }

        public decimal GetUsedTime()
        {
            if (StartTime.HasValue && EndTime.HasValue)
            {
                UsedTime = Convert.ToDecimal((EndTime.Value - StartTime.Value).TotalMinutes);
            }

            return UsedTime;
        }

        public decimal GetTotalSheetPerPallet()
        {
            int SheetPerPallet = 0;

            SheetPerPallet = SheetPerBundle * BundlePerLayer * LayerPerPallet;

            return SheetPerPallet;
        }
    }
}
