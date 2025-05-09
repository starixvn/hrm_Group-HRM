namespace HRM.Core.Enums;

using System.ComponentModel;

public enum RoleSystem
{
    [Description("Member")]
    Member = 1,

    [Description("Verifier")]
    Verifier,

    [Description("Approver")]
    Approver,

    [Description("Shift Current Week Editor")]
    ShiftManager,

    [Description("On Behalf Request")]
    OnBehalfRequest,

    [Description("HR")]
    HR,

    [Description("System Admin")]
    SystemAdmin,

    [Description("Discipline Manager")]
    DisciplineManager,

    [Description("Shift Arrange")]
    ShiftManager2,

    [Description("Local HR")]
    LocalHR,

    [Description("Expat HR")]
    ExpatHR,

    [Description("HR Assistant")]
    HRAssistant,

    [Description("Payroll Admin")]
    PayrollAdmin
}