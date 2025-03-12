namespace HRM.ViewModels;

using System;
using System.Collections.Generic;
using HRM.Models.Report;

public class WorkingCalendarViewModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public IEnumerable<WorkingCalendar> WorkingCalendars { get; set; } = new List<WorkingCalendar>();
}