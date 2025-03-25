namespace HRM.Core;

using System;
using Constants;
using GO.UserControl.Client.Core.Session;

public static class DateTimeExtensions
{
    private static IUserControlSession _userControlSession;
    private static readonly TimeSpan ServerTimeZone = TimeSpan.FromHours(-4);

    public static void Configure(IUserControlSession userControlSession)
    {
        _userControlSession = userControlSession;
    }

    public static DateTime FirstDayOfWeek(this DateTime dt)
    {
        var diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public static DateTime LastDayOfWeek(this DateTime dt)
        => dt.FirstDayOfWeek().AddDays(6);

    public static DateTime FirstDayOfMonth(this DateTime dt)
        => new DateTimeOffset(dt.Year, dt.Month, 1, 0, 0, 0, TimeSpan.Zero).Date;

    public static DateTime LastDayOfMonth(this DateTime dt)
        => dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

    public static string ToFormatedDate(this DateTime? date)
        => date.HasValue ? date.Value.ToFormatedDate() : string.Empty;

    public static string ToFormatedDate(this DateTime date)
        => date.ToString("dd MMM, yyyy hh:mm tt");

    public static string ToShortFormatedDate(this DateTime? date)
        => date.HasValue ? date.Value.ToShortFormatedDate() : string.Empty;

    public static string ToShortFormatedDate(this DateTime date)
        => date.ToString(@"dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");

    public static string ToDetailFormatedDate(this DateTime date)
        => date.ToString(@"ddd, dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");

    public static string ToMonthAndDayFormatedDate(this DateTime? date)
        => date.HasValue ? date.Value.ToMonthAndDayFormatedDate() : string.Empty;

    public static string ToMonthAndDayFormatedDate(this DateTime date)
        => date.ToString("dd MMM");

    public static string ToMonthAndYearFormatedDate(this DateTime? date)
        => date.HasValue ? date.Value.ToMonthAndYearFormatedDate() : string.Empty;

    public static string ToMonthAndYearFormatedDate(this DateTime date)
        => date.ToString("MMM-yyyy");

    public static string ToDayOfWeekFormatedDate(this DateTime date)
        => date.ToString("ddd, dd MMM, yyyy");

    public static string ToDateMonthYearFormatedDate(this DateTime date)
        => date.ToString("dd MMM, yyyy");

    public static string ToDateMonthYearFormatedDate(this DateTime? date)
        => date.HasValue ? date.Value.ToDateMonthYearFormatedDate() : string.Empty;

    public static string ToDateOfWeek(this DateTime date)
        => date.ToString("d MMM, ddd");

    public static string ToDateOfWeek(this DateTime? date)
        => date.HasValue ? date.Value.ToDateOfWeek() : string.Empty;

    public static string ToShortTime(this DateTime date)
        => date.ToString("HH:mm");

    public static DateTime ToStartOfDay(this DateTime time)
        => new DateTimeOffset(time.Year, time.Month, time.Day, 00, 00, 00, TimeSpan.Zero).DateTime;

    public static DateTime ToEndOfDay(this DateTime time)
        => new DateTimeOffset(time.Year, time.Month, time.Day, 23, 59, 59, TimeSpan.Zero).DateTime;

    public static bool IsSameDateWith(this DateTime sourceDate, DateTime toDate)
        => sourceDate.Year == toDate.Year && sourceDate.Month == toDate.Month && sourceDate.Day == toDate.Day;

    public static DateTime ToServerTimeZone(this DateTime time)
    {
        var clientTimeZone = TimeSpan.FromHours(_userControlSession.Get<short>(SessionKey.CurrentTimeZone));
        var offsetTime = new DateTimeOffset(time, clientTimeZone);
        return offsetTime.ToOffset(ServerTimeZone).DateTime;
    }

    public static DateTime ToClientTimeZone(this DateTime time)
    {
        var clientTimeZone = TimeSpan.FromHours(_userControlSession.Get<short>(SessionKey.CurrentTimeZone));
        var offsetTime = new DateTimeOffset(time, ServerTimeZone);
        return offsetTime.ToOffset(clientTimeZone).DateTime;
    }
}