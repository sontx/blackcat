using System;

namespace Blackcat.Types
{
    public static class DateTimeUtils
    {
        private static readonly DateTime _unixZeroPoint = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int ToUnixEpoch(this DateTime dateTime)
        {
            return (int)(dateTime.ToUniversalTime() - _unixZeroPoint).TotalSeconds;
        }

        public static DateTime ToDateTime(this int unixEpoch)
        {
            return (_unixZeroPoint + TimeSpan.FromSeconds(unixEpoch)).ToLocalTime();
        }
    }
}