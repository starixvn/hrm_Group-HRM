namespace HRM.ViewModels.ScheduleArrange;

using System.Collections.Generic;
using System.Linq;
using HRM.Models.ScheduleArrange;

public class WeeklyScheduleViewModel
{
    public int TeamId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public bool IsPublic => Members.Any(x => x.IsPublic);
    public bool IsSelected => Members.All(x => x.IsSelected);

    public IEnumerable<WeeklySchedule> Members { get; set; } = new List<WeeklySchedule>();
}