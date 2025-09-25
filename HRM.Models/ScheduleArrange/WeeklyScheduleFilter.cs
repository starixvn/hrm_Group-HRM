namespace HRM.Models.ScheduleArrange;

using System;

public class WeeklyScheduleFilter
{
    public int ActorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string TeamIds { get; set; }
    public int? EmployeeId { get; set; }
    public bool IsIFrame { get; set; }
    public byte? IsLocal { get; set; }
    public bool IsEditScheduleUnlimited { get; set; }
    public short? CompanyId { get; set; }
    public string EmployeeStatusIds { get; set; }
}