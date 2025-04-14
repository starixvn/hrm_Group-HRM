namespace HRM.Models.Report;

using ScheduleArrange;

public class WorkingCalendarSummary : DailySchedule
{
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }

    public string TeamCode { get; set; }
    public string TeamName { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
}