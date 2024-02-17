using System;
using System.Collections.Generic;
using System.Text;

namespace Max.BaseKit.Utils
{
    public static class CalculateUtil
    {
        public static double CompareCalculate(this DateTime time, DateTime timeToBeCompared)
        {
            var ts1 = new TimeSpan(time.Ticks);
            var ts2 = new TimeSpan(timeToBeCompared.Ticks);
            var ts = ts1.Subtract(ts2).Duration();

            return ts.TotalSeconds;
        }
        /// <summary>
        /// 计算时间差，返回整型秒差
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeToBeCompared"></param>
        /// <returns></returns>
        public static double ToolsCalculateDuration(DateTime time, DateTime timeToBeCompared)
        {
            var ts1 = new TimeSpan(time.Ticks);
            var ts2 = new TimeSpan(timeToBeCompared.Ticks);
            var ts = ts1.Subtract(ts2).Duration();

            return ts.TotalSeconds;
        }

        public static string FormatDt = "yyyy-MM-dd HH:mm:ss:SSS";
    }
}
