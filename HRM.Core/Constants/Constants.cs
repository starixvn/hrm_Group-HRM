using System;

namespace HRM.Core.Constants;

public static class Constants
{
    public const string JobTitle = "JT";
    public const string MemoCategory = "MC";

    public const byte December = 12;
    public const byte NumberMonthsInFutureCanLeaveRequest = 7;

    public const string TimeFormat = "HH:mm";

    public const decimal MaxScoreSubtractDate = (decimal)3.5;

    public const byte CurrentPage = 1;
    public const byte ItemsPerPage = 50;

    public const string BeginRootTag = "<ROOT>";
    public const string EndRootTag = "</ROOT>";

    public const byte TotalMinuteInHour = 60;
    public const byte TotalWeekendLeaveLimit = 16;
    public const decimal HalfDayOffValue = 0.5M;
    public const short DefaultRequestFilterStatusId = -2;
    public const short PendingAndVerifiedStatusId = -1;
    public const short OffDayShiftId = -1;

    public const string ProfileUploadPath = "file/uploads/profile/";

    public const string RedColor = "#ff0000";
    public const string OffDayBackgroundColor = "#f8f89c";
    public const string HolidayBackgroundColor = "#f3aa4b";

    public const string SummaryFormula = "SUM({0})";

    public const string Yes = "Yes";

    public const string No = "No";

    public const int NexCompanyId = 1;

    public const int LumiCompanyId = 2;

    public const int NexVisionCompanyId = 34;

    public static readonly TimeSpan NexTimezone = TimeSpan.FromHours(7);

    public static readonly int[] NexCompanyIds = [NexCompanyId, LumiCompanyId, NexVisionCompanyId];

    public const string AllIp = "0.0.0.0";

    public static DateTime MinDate = new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
}