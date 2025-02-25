namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum SettingsSendMail
    {
        [StringValue("New")]
        New = 1,

        [StringValue("Verify")]
        Verify = 2,

        [StringValue("NotVerify")]
        NotVerify = 3,

        [StringValue("Approve")]
        Approve = 4,

        [StringValue("Reject")]
        Reject = 5,

        [StringValue("Process")]
        Process = 6,

        [StringValue("Void")]
        Void = 7,

        [StringValue("Reset1")]
        Reset1 = 8,

        [StringValue("Reset2")]
        Reset2 = 9,

        [StringValue("Reset3")]
        Reset3 = 10,

        [StringValue("Edit")]
        Edit = 11,

        [StringValue("Cancel")]
        Cancel = 12,

        [StringValue("Reedit")]
        Reedit = 13,

        [StringValue("Delete")]
        Delete = 14
    }
}