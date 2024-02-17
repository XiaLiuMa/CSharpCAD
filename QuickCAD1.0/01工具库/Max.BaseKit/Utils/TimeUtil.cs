﻿using System;
using System.Globalization;

namespace Max.BaseKit.Utils
{
    public static class TimeUtil
    {
        /// <summary>
        /// 时间字符串转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format1"></param>
        /// <param name="format2"></param>
        /// <returns></returns>
        public static string TimeStrConvert(this string str, string format1,string format2)
        {
            if(string.IsNullOrEmpty(str) || string.IsNullOrEmpty(format1) || string.IsNullOrEmpty(format2)) return string.Empty;
            if (DateTime.TryParseExact(str, format1, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                return dateTime.ToString(format2);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 总年数(向上延伸：年份相同的视为相差1年)
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static int GetYearTotal(DateTime time1, DateTime time2)
        {
            if (time2 < time1) return 0;
            var yeqrs = time2.Year - time1.Year;
            return yeqrs + 1;
        }

        /// <summary>
        /// 总月数(向上延伸：月份相同的视为相差月年)
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static int GetMonthTotal(DateTime time1, DateTime time2)
        {
            if (time2 < time1) return 0;
            var months = ((time2.Year - time1.Year) * 12) + (time2.Month - time1.Month);
            return months + 1;
        }

        /// <summary>
        /// 总天数(向上延伸：月份相同的视为相差月年)
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static int GetDayTotal(DateTime time1, DateTime time2)
        {
            if (time2 < time1) return 0;
            TimeSpan tspan = time2 - time1;
            int days = (int)Math.Ceiling(tspan.TotalDays);//该函数只要有小数点就往上+1
            return days;
        }

        /// <summary>
        /// 获取开始时间
        /// </summary>
        /// <param name="TimeType">"Y","S","M","W","D","H"对应"年季月周日时"</param>
        /// <param name="now">时间</param>
        /// <returns></returns>
        public static string GetStartTime(string TimeType, DateTime now)
        {
            switch (TimeType)
            {
                case "Y":
                    DateTime ytime = now.AddDays(-now.DayOfYear + 1);
                    return ytime.ToString("yyyyMMdd000000");
                case "S":
                    DateTime _stime = now.AddMonths(0 - ((now.Month - 1) % 3));
                    DateTime stime = _stime.AddDays(-_stime.Day + 1);
                    return stime.ToString("yyyyMMdd000000");
                case "M":
                    DateTime mtime = now.AddDays(-now.Day + 1);
                    return mtime.ToString("yyyyMMdd000000");
                case "W":
                    DateTime wtime = now.AddDays(-(int)now.DayOfWeek + 1);
                    return wtime.ToString("yyyyMMdd000000");
                case "D":
                    return now.ToString("yyyyMMdd000000");
                case "H":
                    return now.ToString("yyyyMMddHH0000");
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 获取结束时间
        /// </summary>
        /// <param name="TimeType">"Y","S","M","W","D","H"对应"年季月周日时"</param>
        /// <param name="now">时间</param>
        /// <returns></returns>
        public static string GetEndTime(string TimeType, DateTime now)
        {
            switch (TimeType)
            {
                case "Y":
                    DateTime _ytime = now.AddYears(1);
                    DateTime ytime = _ytime.AddDays(-_ytime.DayOfYear);
                    return ytime.ToString("yyyyMMdd235959");
                case "S":
                    DateTime _stime = now.AddMonths((3 - ((now.Month - 1) % 3) - 1));
                    DateTime stime = _stime.AddMonths(1).AddDays(-_stime.AddMonths(1).Day + 1).AddDays(-1);
                    return stime.ToString("yyyyMMdd235959");
                case "M":
                    DateTime mtime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                    return mtime.ToString("yyyyMMdd235959");
                case "W":
                    DateTime wtime = now.AddDays(7 - (int)now.DayOfWeek);
                    return wtime.ToString("yyyyMMdd235959");
                case "D":
                    return now.ToString("yyyyMMdd235959");
                case "H":
                    return now.ToString("yyyyMMddHH5959");
                default:
                    return string.Empty;
            }
        }



        /// <summary>
        /// 获取开始时间
        /// </summary>
        /// <param name="TimeType">Week、Month、Season、Year</param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime? GetTimeStartByType(string TimeType, DateTime now)
        {
            switch (TimeType)
            {
                case "Week":
                    return now.AddDays(-(int)now.DayOfWeek + 1);
                case "Month":
                    return now.AddDays(-now.Day + 1);
                case "Season":
                    var time = now.AddMonths(0 - ((now.Month - 1) % 3));
                    return time.AddDays(-time.Day + 1);
                case "Year":
                    return now.AddDays(-now.DayOfYear + 1);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取结束时间
        /// </summary>
        /// <param name="TimeType">Week、Month、Season、Year</param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime? GetTimeEndByType(string TimeType, DateTime now)
        {
            switch (TimeType)
            {
                case "Week":
                    return now.AddDays(7 - (int)now.DayOfWeek);
                case "Month":
                    return now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                case "Season":
                    var time = now.AddMonths((3 - ((now.Month - 1) % 3) - 1));
                    return time.AddMonths(1).AddDays(-time.AddMonths(1).Day + 1).AddDays(-1);
                case "Year":
                    var time2 = now.AddYears(1);
                    return time2.AddDays(-time2.DayOfYear);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取时间段切片数量
        /// </summary>
        /// <param name="sectionType">切片类型(Y/M/D/H:年/月/日/时),其它标识不切片</param>
        /// <param name="dt1">开始时间</param>
        /// <param name="dt2">结束时间</param>
        /// <returns></returns>
        public static int GetSectionTotal(string sectionType, DateTime dt1, DateTime dt2)
        {
            if (dt2 < dt1) return 0;
            int rTotal = 0;
            switch (sectionType)
            {
                case "Y": rTotal = (dt2.Year - dt1.Year) + 1; break;//按年切片(向上延伸：年份相同的视为相差1年)
                case "M": rTotal = ((dt2.Year - dt1.Year) * 12) + (dt2.Month - dt1.Month) + 1; break;//按月切片(向上延伸：月份相同的视为相差1月)
                case "D": //按天切片(向上延伸：日期相同的视为相差1天)
                    {
                        rTotal = (int)Math.Ceiling(((TimeSpan)(dt2 - dt1)).TotalDays);//相差天数(该函数只要有小数点就往上+1)
                        DateTime d_dt1 = DateTime.ParseExact(dt1.ToString("HHmmss"), "HHmmss", CultureInfo.CurrentCulture);
                        DateTime d_dt2 = DateTime.ParseExact(dt2.ToString("HHmmss"), "HHmmss", CultureInfo.CurrentCulture);
                        rTotal += (d_dt1 > d_dt2) ? 1 : 0;
                    }
                    break;
                case "H": //按小时切片(向上延伸：小时相同的视为相差1小时)
                    {
                        rTotal = (int)Math.Ceiling(((TimeSpan)(dt2 - dt1)).TotalHours);//相差小时数(该函数只要有小数点就往上+1)
                        DateTime h_dt1 = DateTime.ParseExact(dt1.ToString("mmss"), "mmss", CultureInfo.CurrentCulture);
                        DateTime h_dt2 = DateTime.ParseExact(dt2.ToString("mmss"), "mmss", CultureInfo.CurrentCulture);
                        rTotal += (h_dt1 > h_dt2) ? 1 : 0;
                    }
                    break;
                default: rTotal = 1; break;
            }
            return rTotal;
        }
    }
}
