namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum RequestCommand
    {
        [StringValue("Accept")]
        Accept = 1,

        [StringValue("NotAccept")]
        NotAccept = 2,

        [StringValue("Approve")]
        Approve = 3,

        [StringValue("Reject")]
        Reject = 4,

        [StringValue("Process")]
        Process = 5,

        [StringValue("Void")]
        Void = 6,

        [StringValue("Reset")]
        Reset = 7
    }
}