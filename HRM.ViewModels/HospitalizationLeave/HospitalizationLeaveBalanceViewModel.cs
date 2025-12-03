namespace HRM.ViewModels.HospitalizationLeave;

using System.Collections.Generic;
using HRM.Core.Enums;
using HRM.Models.HospitalizationLeave;

public class HospitalizationLeaveBalanceViewModel
{
    public int Year { get; set; }
    public IEnumerable<Models.Team> Teams { get; set; }
    public string TeamIds { get; set; }
    public int? EmployeeId { get; set; }
    public string Employee { get; set; }
    public bool? IsLocal { get; set; }
    public bool IsHost { get; set; }
    public string EmployeeStatusIds { get; set; } = $"{(byte)EmployeeStatus.Enabled}";
    public IEnumerable<HospitalizationLeaveBalanceSummaryReport> Employees { get; set; }
}