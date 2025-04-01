namespace HRM.Models.Report;

using ScheduleArrange;

public class WorkingCalendarSummary : DailySchedule
{
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public string Team { get; set; }
    public string DepartmentCode { get; set; }
    public string Section { get; set; }
}