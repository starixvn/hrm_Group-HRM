namespace HRM.Models;

public class LeaveType : EntityBase<int>
{
	public bool? IsALDeduction { get; set; }

    public bool? IsHoliday { get; set; }

    public bool IsLeaveDuringWorking { get; set; }

    public string Description { get; set; }
}