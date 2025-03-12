namespace HRM.Models.ScheduleArrange;

using System;
using Core.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class EditDefaultShift
{
    public int EmployeeId { get; set; }
    public DateTime FromDate { get; set; }
    public int? ShiftId { get; set; }
}