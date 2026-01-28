namespace Common.Toolkit.Extensions;

public static class DateExtensions
{
    private static readonly TimeZoneInfo PolandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    /// <summary>
    /// Converts UTC DateTime to Poland timezone (Central European Time)
    /// </summary>
    /// <param name="utcDateTime">DateTime in UTC</param>
    /// <returns>DateTime converted to Poland timezone</returns>
    public static DateTime ToPolandTimeZone(this DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            // If the DateTime is not explicitly UTC, treat it as UTC for conversion
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, PolandTimeZone);
    }

    /// <summary>
    /// Converts any DateTime to Poland timezone
    /// Assumes the input is UTC if Kind is Unspecified
    /// </summary>
    /// <param name="dateTime">DateTime to convert</param>
    /// <returns>DateTime converted to Poland timezone</returns>
    public static DateTime ToPolandTimeZoneFromUtc(this DateTime dateTime)
    {
        DateTime utcDateTime = dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
        };

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, PolandTimeZone);
    }
}
