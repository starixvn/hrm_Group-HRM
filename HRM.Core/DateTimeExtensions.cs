namespace HRM.Core;

using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
public static class DateTimeExtensions
{
    public static DateTime FirstDayOfWeek(this DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public static DateTime LastDayOfWeek(this DateTime dt)
    {
        return dt.FirstDayOfWeek().AddDays(6);
    }

    public static DateTime FirstDayOfMonth(this DateTime dt)
    {
        return new DateTimeOffset(dt.Year, dt.Month, 1, 0, 0, 0, TimeSpan.Zero).Date;
    }

    public static DateTime LastDayOfMonth(this DateTime dt)
    {
        return dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);
    }

    public static string ToFormatedDate(this DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToFormatedDate();
        }

        return string.Empty;
    }

    public static string ToFormatedDate(this DateTime date)
    {
        return date.ToString("dd MMM, yyyy hh:mm tt");
    }

    public static string ToShortFormatedDate(this DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToShortFormatedDate();
        }

        return string.Empty;
    }

    public static string ToShortFormatedDate(this DateTime date)
    {
        string dateStr = date.ToString(@"dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");
        return dateStr;
    }

    public static string ToDetailFormatedDate(this DateTime date)
    {
        string dateStr = date.ToString(@"ddd, dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");
        return dateStr;
    }

    public static string ToMonthAndDayFormatedDate(this DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToMonthAndDayFormatedDate();
        }

        return string.Empty;
    }

    public static string ToMonthAndDayFormatedDate(this DateTime date)
    {
        return date.ToString("dd MMM");
    }

    public static string ToMonthAndYearFormatedDate(this DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToMonthAndYearFormatedDate();
        }

        return string.Empty;
    }

    public static string ToMonthAndYearFormatedDate(this DateTime date)
    {
        return date.ToString("MMM-yyyy");
    }

    public static string ToDayOfWeekFormatedDate(this DateTime date)
    {
        return date.ToString("ddd, dd MMM, yyyy");
    }

    public static string ToDateMonthYearFormatedDate(this DateTime date)
    {
        return date.ToString("dd MMM, yyyy");
    }

    public static string ToDateMonthYearFormatedDate(this DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToDateMonthYearFormatedDate();
        }

        return string.Empty;
    }

    public static string ToDateOfWeek(this DateTime date)
    {
        return date.ToString("d MMM, ddd");
    }

    public static string ToDateOfWeek(this DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToDateOfWeek();
        }

        return string.Empty;
    }

    public static string ToShortTime(this DateTime date)
    {
        return date.ToString("HH:mm");
    }

    public static DateTime ToEndOfDay(this DateTime time)
    {
        return new DateTimeOffset(time.Year, time.Month, time.Day, 23, 59, 59, TimeSpan.Zero).DateTime;
    }

    public static bool IsSameDateWith(this DateTime sourceDate, DateTime toDate)
    {
        return (sourceDate.Year == toDate.Year && sourceDate.Month == toDate.Month && sourceDate.Day == toDate.Day);
    }

    public static DateTime ToServerTimeZone(this DateTime time, string serverTimeZoneOffset)
    {
        var utcTime = time.ToUniversalTime();
        var serverOffsetSpan = ParseTimeZoneOffsetString(serverTimeZoneOffset);

        var serverTime = utcTime.Add(serverOffsetSpan);

        return serverTime;
    }

    public static DateTime ToClientTimeZone(this DateTime serverTime, string serverTimeZoneOffset, string clientTimeZoneOffset = "7")
    {
        var serverOffsetSpan = ParseTimeZoneOffsetString(serverTimeZoneOffset);

        var utcTime = serverTime.Subtract(serverOffsetSpan);

        var clientOffsetSpan = ParseTimeZoneOffsetString(clientTimeZoneOffset);
        var clientTime = utcTime.Add(clientOffsetSpan);

        return clientTime;
    }

    private static TimeSpan ParseTimeZoneOffsetString(string offset)
    {
        if(!offset.Contains(':')) offset += ":00";
        return TimeSpan.Parse(offset);
    }
}