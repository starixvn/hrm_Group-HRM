namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum RoleSystem
    {
        [StringValue("Member")]
        Member = 1,

        [StringValue("Verifier")]
        Verifier = 2,

        [StringValue("Approver")]
        Approver = 3,

        [StringValue("Shift Current Week Editor")]
        ShiftManager = 4,

        [StringValue("On Behalf Request")]
        OnBehalfRequest = 5,

        [StringValue("HR")]
        HR = 6,

        [StringValue("System Admin")]
        SystemAdmin = 7,

        [StringValue("Discipline Manager")]
        DisciplineManager = 8,

        [StringValue("Shift Arrange")]
        ShiftManager2 = 9,

        [StringValue("Local HR")]
        LocalHR = 10,

        [StringValue("Expat HR")]
        ExpatHR = 11
    }
}