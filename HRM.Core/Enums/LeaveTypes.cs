namespace HRM.Core.Enums;

using System.ComponentModel;

public enum LeaveTypes
{
    [Description("General Leave")]
    GeneralLeave = 0,

    [Description("Annual Leave")]
    AnnualLeave = 2,

    [Description("Suspend Work")]
    SuspendWork = 3,

    [Description("Employee Wedding (3 Days)")]
    WeddingLeave = 6,

    [Description("Business Trip")]
    BusinessTrip = 9,

    [Description("Compensation Leave (by Day)")]
    KeepInViewByDay = 10,

    [Description("Maternity Leave")]
    MaternityLeave = 12,

    [Description("Paternity Leave")]
    PaternityLeave = 14,

    [Description("Bereavement Leave")]
    BereavementLeave = 21,

    [Description("Unpaid Leave")]
    UnpaidLeave = 25,

    [Description("Business during working")]
    BusinessDuringWorking = 26,

    [Description("Offset Leave")]
    OffsetLeave = 27,

    [Description("Wedding of Employee's Children")]
    WeddingOfEmployeeChildren = 34,

    [Description("Company Event")]
    CompanyEvent = 38,

    [Description("Hospitalization Leave")]
    HospitalizationLeave = 39,

    [Description("Unpaid Leave (Insurance)")]
    UnpaidLeaveInsurance = 41,

    [Description("Compensation Leave (by Hour)")]
    KeepInViewByHour = 48
}