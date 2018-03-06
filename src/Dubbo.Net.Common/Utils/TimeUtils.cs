using System;

namespace Dubbo.Net.Common.Utils
{
    public static class TimeUtils
    {
        /// <summary>
        /// 时间转为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime time)
        {
            DateTime startTime =TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0),TimeZoneInfo.Local);
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
    }
}
