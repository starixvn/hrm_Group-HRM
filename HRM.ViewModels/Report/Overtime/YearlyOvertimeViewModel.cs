namespace HRM.ViewModels.Report.Overtime;

using System;
using System.Collections.Generic;
using Core.Enums;
using Models;
using HRM.Models.Report.Overtime;

public class YearlyOvertimeViewModel
{
    public int Year { get; set; } = DateTimeOffset.Now.Year;
    public string Employee { get; set; }
    public IEnumerable<EntityBase<int>> Teams { get; set; }
    public string TeamIds { get; set; }
    public byte Calendar { get; set; } = (byte)CalendarType.Normal;
    public byte? EmployeeStatusId { get; set; } = (byte)EmployeeStatus.Enabled;
    public byte? IsLocal { get; set; }
    public IEnumerable<YearlyOvertime> Overtimes { get; set; } = new List<YearlyOvertime>();
}