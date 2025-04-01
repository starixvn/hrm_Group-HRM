namespace HRM.Core.Enums;

using System.ComponentModel;

public enum RoleSystem
{
    [Description("Member")]
    Member = 1,

    [Description("Verifier")]
    Verifier = 2,

    [Description("Approver")]
    Approver = 3,

    [Description("Shift Current Week Editor")]
    ShiftManager = 4,

    [Description("On Behalf Request")]
    OnBehalfRequest = 5,

    [Description("HR")]
    HR = 6,

    [Description("System Admin")]
    SystemAdmin = 7,

    [Description("Discipline Manager")]
    DisciplineManager = 8,

    [Description("Shift Arrange")]
    ShiftManager2 = 9,

    [Description("Local HR")]
    LocalHR = 10,

    [Description("Expat HR")]
    ExpatHR = 11
}