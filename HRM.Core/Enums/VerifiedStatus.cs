namespace HRM.Core.Enums;

using System.ComponentModel;

public enum VerifiedStatus
{
    [Description("Pending")]
    Pending = 1,

    [Description("Verified")]
    Verified = 2,

    [Description("NotVerified")]
    NotVerified = 3
}