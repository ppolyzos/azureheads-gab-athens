using System;

namespace gab_athens.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        // https://stackoverflow.com/questions/7908343/list-of-timezone-ids-for-use-with-findtimezonebyid-in-c
        private const string TimeZoneAthens = "GTB Standard Time";

        public static string ToTimeZone(this DateTime dateTime, string format, string nameOfTimeZone = TimeZoneAthens)
        {
            var tst = TimeZoneInfo.FindSystemTimeZoneById(nameOfTimeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, tst).ToString(format);
        }
    }
}