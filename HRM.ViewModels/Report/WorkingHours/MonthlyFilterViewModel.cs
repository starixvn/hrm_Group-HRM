namespace HRM.ViewModels.Report.WorkingHours;

using System;
using System.Collections.Generic;
using Models;

public class MonthlyFilterViewModel
{
    public string TeamIds { get; set; }
    public IEnumerable<EntityBase<int>> Teams { get; set; }
    public string Employee { get; set; }
    public byte? IsLocal { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string EmployeeStatusIds { get; set; }
}