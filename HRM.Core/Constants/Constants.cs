namespace HRM.Core.Constants
{
    public static class Constants
    {
        public const string JobTitle = "JT";
        public const string MemoCategory = "MC";

        public const byte December = 12;
        public const byte NumberMonthsInFutureCanLeaveRequest = 7;

        public const string DateFormat = "dd MMM, yyyy";
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

        public const string ProfileUploadPath = "/file/uploads/profile/";

        public const string RedColor = "#ff0000";
        public const string OffDayBackgroundColor = "#f8f89c";
        public const string HolidayBackgroundColor = "#f3aa4b";

        public const string SummaryFormula = "SUM({0})";
    }
}