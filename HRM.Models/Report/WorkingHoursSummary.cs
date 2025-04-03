namespace HRM.Models.Report;

using System;

public class WorkingHoursSummary
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string Team { get; set; }
    public string DepartmentCode { get; set; }
    public string Department { get; set; }

    // contract
    public float WorkingDays { get; set; }
    public float WorkingHours { get; set; }

    // actual working time
    public float TotalWorkingHours { get; set; }
    public double ActualWorkingDays => Math.Round((TotalWorkingHours - TotalOvertimes) / WorkingHours, 1);

    // payment
    public float PaymentWorkingDays { get; set; }
    public float AnnualLeave { get; set; }
    public float BusinessTrip { get; set; }
    public float BereavementLeave { get; set; }
    public float OffsetLeave { get; set; }
    public float WeddingLeave { get; set; }
    public float SuspendWork { get; set; }
    public float HospitalizationLeave { get; set; }
    public float CompanyEvent { get; set; }

    public float TotalPayments => PaymentWorkingDays + AnnualLeave + BusinessTrip + BereavementLeave
                                  + OffsetLeave + WeddingLeave + SuspendWork + HospitalizationLeave + CompanyEvent;

    // unpaid leave
    public float UnpaidLeave { get; set; }
    public float UnpaidLeaveInsurance { get; set; }
    public float PaternityLeave { get; set; }
    public float MaternityLeave { get; set; }
    public float TotalUnpaidLeaves => UnpaidLeaveInsurance + PaternityLeave + MaternityLeave;

    // overtime
    public float NormalOvertimeHours { get; set; }
    public float OffOvertimeHours { get; set; }
    public float PublicOvertimeHours { get; set; }
    public float SpecialOvertimeHours { get; set; }
    public float TotalOvertimes => NormalOvertimeHours + OffOvertimeHours + PublicOvertimeHours + SpecialOvertimeHours;
}