namespace HRM.Models.Report;

using ScheduleArrange;
using System.Collections.Generic;

public class WorkingCalendar
{
    public int EmployeeId { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string Team { get; set; }
    public string DepartmentCode { get; set; }
    public IEnumerable<DailySchedule> Schedules { get; set; }
}