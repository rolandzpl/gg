using System;

namespace MTGG
{
    internal class UTC
    {
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0,
            DateTimeKind.Utc);
 
        public static DateTime UnixTimestampToDate(uint seconds)
        {
            return UTC.UnixEpoch.AddSeconds(seconds).ToLocalTime();
        }
 
        public static uint DateToUnixTimestamp(DateTime date)
        {
            return (uint)date.Subtract(UTC.UnixEpoch).TotalSeconds;
        }
    }
}
