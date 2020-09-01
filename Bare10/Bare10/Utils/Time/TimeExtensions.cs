using System;

namespace Bare10.Utils.Time
{
    public static class TimeExtensions
    {
        public static DateTime DateTimeFromUnixTime( this long epochMillis)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(epochMillis).ToLocalTime();
        }

        public static long ToUnixTime(this DateTime time)
        {
            return (long)time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}