namespace HRM.Core.Enums
{
    using System.ComponentModel;

    public enum OvertimeType
    {
        [Description("All")]
        All = 0,

        [Description("Normal")]
        Normal = 1,

        [Description("Off")]
        Off = 2,

        [Description("Holiday")]
        Holiday = 3,

        [Description("Special")]
        Special = 4,
    }
}