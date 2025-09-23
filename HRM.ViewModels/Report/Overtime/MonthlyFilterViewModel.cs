namespace HRM.ViewModels.Report.Overtime;

using System;

public class MonthlyFilterViewModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string Employee { get; set; }
    public string EmployeeStatusIds { get; set; }
    public string TeamIds { get; set; }
    public byte? IsLocal { get; set; }
}