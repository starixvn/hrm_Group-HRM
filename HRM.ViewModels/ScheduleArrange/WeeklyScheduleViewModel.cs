namespace HRM.ViewModels.ScheduleArrange;

using System.Collections.Generic;
using HRM.Models.ScheduleArrange;

public class WeeklyScheduleViewModel
{
    public int TeamId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }

    public IEnumerable<WeeklySchedule> Members { get; set; } = new List<WeeklySchedule>();
}