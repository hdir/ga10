using Foundation;
using System;

namespace Bare10.iOS.Utils
{
    public static class DateTimeExtensions
    {
        static DateTime iOSReference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this NSDate date)
        {
            return iOSReference.AddSeconds(date.SecondsSinceReferenceDate).ToLocalTime();
        }

        public static NSDate ToNSDate(this DateTime date)
        {
            return NSDate.FromTimeIntervalSinceReferenceDate((date.ToUniversalTime() - iOSReference).TotalSeconds);
        }

        public static string ToAccurateString(this DateTime date)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D4}", date.Hour, date.Minute, date.Second, date.Millisecond);
        }
    }
}