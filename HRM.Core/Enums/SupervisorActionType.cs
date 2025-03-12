namespace HRM.Core.Enums;

using System.ComponentModel;

public enum SupervisorActionType
{
    [Description("Verified by")]
    Verifier = 2,

    [Description("Approved by")]
    Approver = 3,

    [Description("Shift Arrangement")]
    ShiftArrangement = 4,

    [Description("On Behalf Request")]
    OnBehalfRequest = 5,

    [Description("Discipline Manager")]
    DisciplineManager = 8
}