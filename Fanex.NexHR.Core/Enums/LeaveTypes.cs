namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum LeaveTypes
    {
        [StringValue("General Leave")]
        GeneralLeave = 0,

        [StringValue("Annual Leave")]
        AnnualLeave = 2,

        [StringValue("Suspend Work")]
        SuspendWork = 3,

        [StringValue("Employee Wedding (3 Days)")]
        WeddingLeave = 6,

        [StringValue("Business Trip")]
        BusinessTrip = 9,

        [StringValue("Compensation Leave (by Day)")]
        KeepInViewByDay = 10,

        [StringValue("Maternity Leave")]
        MaternityLeave = 12,

        [StringValue("Paternity Leave")]
        PaternityLeave = 14,

        [StringValue("Bereavement Leave")]
        BereavementLeave = 21,

        [StringValue("Unpaid Leave")]
        UnpaidLeave = 25,

        [StringValue("Business during working")]
        BusinessDuringWorking = 26,

        [StringValue("Offset Leave")]
        OffsetLeave = 27,

        [StringValue("Wedding of Employee's Children")]
        WeddingOfEmployeeChildren = 34,

        [StringValue("Company Event")]
        CompanyEvent = 38,

        [StringValue("Hospitalization Leave")]
        HospitalizationLeave = 39,

        [StringValue("Unpaid Leave (Insurance)")]
        UnpaidLeaveInsurance = 41,

        [StringValue("Compensation Leave (by Hour)")]
        KeepInViewByHour = 48
    }
}