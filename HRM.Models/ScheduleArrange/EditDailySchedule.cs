namespace HRM.Models.ScheduleArrange;

using System;
using Core.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class EditDailySchedule
{
    public int EmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public int? ShiftId { get; set; }
    public bool IsPublic { get; set; }
}