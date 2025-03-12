namespace HRM.Core.Enums;

using System.ComponentModel;

public enum JobTitleCategory
{
    [Description("Default")]
    Default = 0,

    [Description("Worker")]
    Worker = 1,

    [Description("Staff")]
    Staff = 2,

    [Description("Executive")]
    Executive = 3,

    [Description("Executive With Position")]
    ExecutiveWithPosition = 4,

    [Description("Management")]
    Management = 5
}