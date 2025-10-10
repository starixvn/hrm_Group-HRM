namespace HRM.Core.Enums;

using System.ComponentModel;

public enum SupervisorActionType
{
    [Description("Verify by")]
    Verifier = 2,

    [Description("Approve by")]
    Approver = 3,

    [Description("Shift Arrangement")]
    ShiftArrangement = 4,

    [Description("On Behalf Request")]
    OnBehalfRequest = 5,

    [Description("Discipline Manager")]
    DisciplineManager = 8,

    [Description("Leader Takeover")]
    LeaderTakeover = 14
}