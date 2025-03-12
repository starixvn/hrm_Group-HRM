namespace HRM.Models;

public class LeaveType : EntityBase<int>
{
    public short? IsAnnualQuota { get; set; }

    public bool? IsDefault { get; set; }

    public bool? IsSalaryDeduction { get; set; }

    public bool? IsALDeduction { get; set; }

    public bool? IsHoliday { get; set; }

    public bool IsLeaveDuringWorking { get; set; }

    public string Description { get; set; }
}