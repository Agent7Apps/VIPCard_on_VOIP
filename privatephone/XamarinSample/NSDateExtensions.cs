using System;
using Foundation;

namespace privatephone
{
    /// <summary>
    /// Should account for a call in progress that changes daylight savings mid-call
    /// https://forums.xamarin.com/discussion/27184/convert-nsdate-to-datetime
    /// </summary>
    public static class NSDateExtensions  
    {
        static DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this NSDate date)
        {
            var utcDateTime = reference.AddSeconds(date.SecondsSinceReferenceDate);
            var dateTime = utcDateTime.ToLocalTime();
            return dateTime;
        }

        public static NSDate ToNSDate(this DateTime datetime)
        {
            var utcDateTime = datetime.ToUniversalTime();
            var date = NSDate.FromTimeIntervalSinceReferenceDate((utcDateTime - reference).TotalSeconds);
            return date;
        }
    }

}