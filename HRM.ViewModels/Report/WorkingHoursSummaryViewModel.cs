namespace HRM.ViewModels.Report;

using System;
using System.Collections.Generic;
using Core;
using Core.Enums;
using Models;
using HRM.Models.Report;

public class WorkingHoursSummaryViewModel
{
    private static readonly DateTime Today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;

    public byte DateOption { get; set; } = (byte)DateRangeOption.CurrentMonth;
    public DateTime FromDate { get; set; } = Today.FirstDayOfMonth();
    public DateTime ToDate { get; set; } = Today;
    public byte? IsLocal { get; set; }
    public string TeamIds { get; set; }
    public string Employee { get; set; }
    public string EmployeeStatusIds { get; set; } = $"{(byte)EmployeeStatus.Enabled}";

    public IEnumerable<EntityBase<int>> Teams { get; set; }
    public IEnumerable<WorkingHoursSummary> WorkingHours { get; set; } = new List<WorkingHoursSummary>();
}