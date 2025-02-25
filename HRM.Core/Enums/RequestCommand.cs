namespace HRM.Core.Enums
{
    using System.ComponentModel;

    public enum RequestCommand
    {
        [Description("Accept")]
        Accept = 1,

        [Description("NotAccept")]
        NotAccept = 2,

        [Description("Approve")]
        Approve = 3,

        [Description("Reject")]
        Reject = 4,

        [Description("Process")]
        Process = 5,

        [Description("Void")]
        Void = 6,

        [Description("Reset")]
        Reset = 7
    }
}