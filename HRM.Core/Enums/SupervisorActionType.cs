namespace HRM.Core.Enums;

using System.ComponentModel;

public enum SupervisorActionType
{
    [Description("Verifed by")]
    Verifier = 2,

    [Description("Approved by")]
    Approver = 3,

    [Description("Shift Arranged by")]
    ShiftArrangement = 4,

    [Description("Request Submitted on Behalf by")]
    OnBehalfRequest = 5,

    [Description("Discipline Managed by")]
    DisciplineManager = 8,

    [Description("Leader Takeover")]
    LeaderTakeover = 14
}