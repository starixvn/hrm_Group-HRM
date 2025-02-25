namespace HRM.Core.Enums
{
    using System.ComponentModel;

    public enum MessageCategory
    {
        [Description("Notification")]
        Notification = 0,

        [Description("Warning")]
        Warning = 1,

        [Description("Violation")]
        Violation = 2
    }
}
