namespace HRM.Core.Enums;

using System.ComponentModel;

public enum Permission
{
    [Description("None")]
    None = 0,

    [Description("Read")]
    Read = 1,

    [Description("Host")]
    Host = 2
}