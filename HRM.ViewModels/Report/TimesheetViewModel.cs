namespace HRM.ViewModels.Report;

public class TimesheetViewModel
{
    public int Year { get; set; }

    public int Month { get; set; }

    public int? EmpId { get; set; }

    public bool IsHavePrivilegeCreateCheckInOutRecord { get; set; }
}