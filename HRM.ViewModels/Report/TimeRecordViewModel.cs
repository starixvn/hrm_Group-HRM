namespace HRM.ViewModels;

using System;
using System.Collections.Generic;
using HRM.Core;

public class TimeRecordViewModel : PagerOptionModel
{
    public DateTime FromDate { get; set; } = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
    public DateTime ToDate { get; set; } = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
    public int EmployeeId { get; set; }
    public string Employee { get; set; }
    public string TeamIds { get; set; }
    public IList<Models.Team> Teams { get; set; }
    public byte? IsLocal { get; set; }
}