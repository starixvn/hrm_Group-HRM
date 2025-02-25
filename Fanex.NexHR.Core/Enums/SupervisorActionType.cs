namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum SupervisorActionType
    {
        [StringValue("Verified by")]
        Verifier = 2,

        [StringValue("Approved by")]
        Approver = 3,

        [StringValue("Shift Arrangement")]
        ShiftArrangement = 4,

        [StringValue("On Behalf Request")]
        OnBehalfRequest = 5,

        [StringValue("Discipline Manager")]
        DisciplineManager = 8
    }
}