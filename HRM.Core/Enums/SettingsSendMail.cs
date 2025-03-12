namespace HRM.Core.Enums;

using System.ComponentModel;

public enum SettingsSendMail
{
    [Description("New")]
    New = 1,

    [Description("Verify")]
    Verify = 2,

    [Description("NotVerify")]
    NotVerify = 3,

    [Description("Approve")]
    Approve = 4,

    [Description("Reject")]
    Reject = 5,

    [Description("Process")]
    Process = 6,

    [Description("Void")]
    Void = 7,

    [Description("Reset1")]
    Reset1 = 8,

    [Description("Reset2")]
    Reset2 = 9,

    [Description("Reset3")]
    Reset3 = 10,

    [Description("Edit")]
    Edit = 11,

    [Description("Cancel")]
    Cancel = 12,

    [Description("Reedit")]
    Reedit = 13,

    [Description("Delete")]
    Delete = 14
}