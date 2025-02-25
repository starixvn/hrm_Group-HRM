namespace HRM.Core.Constants
{
    public static class RequestFormatConstants
    {
        public const string MsgLeaveWithNoFormat = "{0} ({1})";
        public const string MsgLeaveHalfDayWithNoFormat = "{0} ({1} → {2})";

        public const string MsgChangeShiftOneDayWithNoFormat = "{0} → {1} ({2})";
        public const string MsgChangeShiftWithNoFormat = "{0}";

        public const string MsgCheckInOutWithNoFormat = "{0} {1} ({2})";

        public const string MsgLateInWithNoFormat = "+{0}' {1} ({2} - Actual: {3})";
        public const string MsgEarlyOutWithNoFormat = "-{0}' {1} ({2} - Actual: {3})";

        public const string MsgKeepInViewOneDayWithNoFormat = "{0} (1 day)";
        public const string MsgKeepInViewHalfdayWithNoFormat = "{0} ({1} → {2}) 0.5 day";
        public const string MsgKeepInViewByHoursWithNoFormat = "{0} ({1} → {2}) {3} hour(s)";

        public const string MsgLeave = "<span class='req leave'>{0}</span> {1}";
        public const string MsgTotalLeaveDays = "{0} day(s)";

        public const string MsgChangeShiftOneDay = "<span class='req changeshift'>{0} <span class='arr'>→</span> {1}</span> {2}";
        public const string MsgChangeShift = "<span class='req changeshift'>{0}</span>";
        public const string MsgCheckInOut = "<span class='req checkinout'>{0} {1}</span> {2}";
        public const string MsgLateIn = "<span class='req inout'>+{0}' {1}</span> {2} (Actual: {3})";
        public const string MsgEarlyOut = "<span class='req inout'>-{0}' {1}</span> {2} (Actual: {3})";

        public const string MsgKeepInViewOneDay = "<span class='req keepinview'>{0}</span> 1 day";
        public const string MsgKeepInViewHalfday = "<span class='req keepinview'>{0} ({1} → {2})</span> 0.5 day";

        public const string MsgKeepInViewByHours = "<span class='req keepinview'>{0} ({1} → {2})</span> {3} hour(s)";
    }
}
