namespace HRM.Models.Discipline;

using System;

public class DisciplineDetailModel
{
    public DateTime WorkDate { get; set; }

    public string ShiftName { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string TotalHours { get; set; }

    public string MinutesLateIn { get; set; }

    public string MinutesEarlyOut { get; set; }

    public EmployeeInfoModel Employee { get; set; }

    public DisciplineModel ChangeShift { get; set; }

    public DisciplineModel MissingCheckInOut { get; set; }

    public DisciplineModel LateIn { get; set; }

    public DisciplineModel EarlyOut { get; set; }

    public DisciplineModel Absent { get; set; }

    public DisciplineModel UnplannedLeave { get; set; }

    public DisciplineModel UnpaidLeave { get; set; }

    public DisciplineModel WorkingUnderTime { get; set; }

    public DisciplineModel UnpaidLeaveAbsent { get; set; }

    public DisciplineModel WeekendLeave { get; set; }
    public DisciplineModel SevenDaysLeave { get; set; }

    public decimal SystemSubtractScore { get; set; }

    public decimal ApproverSubtractScore { get; set; }

    public string ApproverComment { get; set; }

    public string EmployeeComment { get; set; }

    public bool IsWeekend => WorkDate.DayOfWeek == DayOfWeek.Saturday || WorkDate.DayOfWeek == DayOfWeek.Sunday;

    public bool IsOutOfControlReason { get; set; }
}