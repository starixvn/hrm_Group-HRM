namespace HRM.ViewModels.Report.Overtime;

using System;
using System.Collections.Generic;
using Core.Enums;
using Models;
using HRM.Models.Report.Overtime;

public class DailyOvertimeDetailsViewModel
{
    public DateTime FromDate { get; set; } = DateTimeOffset.Now.AddMonths(-1).Date;
    public DateTime ToDate { get; set; } = DateTimeOffset.Now.Date;
    public string TeamIds { get; set; }
    public IEnumerable<EntityBase<int>> Teams { get; set; }
    public string Employee { get; set; }
    public byte? IsLocal { get; set; }
    public string EmployeeStatusIds { get; set; } = $"{(byte)EmployeeStatus.Enabled}";
    public IEnumerable<DailyOvertimeDetails> Overtimes { get; set; } = new List<DailyOvertimeDetails>();
}