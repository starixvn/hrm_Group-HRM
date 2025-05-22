using System.Collections.Generic;
using HRM.Models;

namespace HRM.ViewModels.AnnualLeave;

public class LeaveSummaryViewModel
{
    public int Year { get; set; }
    public IEnumerable<Models.Team> Teams { get; set; }

    public string TeamIds { get; set; }
    public string Employee { get; set; }
    public string Period { get; set; }
    public int EmployeeId { get; set; }
    public int? Category { get; set; }
    public string LeaveType { get; set; }
    public bool IsShowCloseBalance { get; set; }
    public IEnumerable<LeaveSummary> LeaveSumaries { get; set; }
    public IEnumerable<LeaveSummaryForJoinedDate> LeaveSumariesForJoinedDate { get; set; }
}