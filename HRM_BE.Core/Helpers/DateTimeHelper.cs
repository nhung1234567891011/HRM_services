namespace HRM_BE.Core.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly string[] VietnamTimeZoneIds =
        {
            "Asia/Ho_Chi_Minh",
            "SE Asia Standard Time"
        };

        public static DateTime UtcNow => DateTime.UtcNow;

        public static DateTime BusinessNow => ConvertUtcToBusiness(UtcNow);

        public static DateTime ConvertUtcToBusiness(DateTime utcDateTime)
        {
            var utc = utcDateTime.Kind == DateTimeKind.Utc
                ? utcDateTime
                : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            var utcOffset = new DateTimeOffset(utc);

            foreach (var timeZoneId in VietnamTimeZoneIds)
            {
                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                    return TimeZoneInfo.ConvertTime(utcOffset, tz).DateTime;
                }
                catch (TimeZoneNotFoundException)
                {
                    // Try next timezone id.
                }
                catch (InvalidTimeZoneException)
                {
                    // Try next timezone id.
                }
            }

            return utc.AddHours(7);
        }
    }
}
