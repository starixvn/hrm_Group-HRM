namespace HRM.Models.ScheduleArrange;

using System;
using Core.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class EditTeamOrder
{
    public int TeamId { get; set; }
}