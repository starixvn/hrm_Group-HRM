namespace HRM.Models.ScheduleArrange;

using System;
using Newtonsoft.Json;

public class WeeklySchedule
{
    public int EmployeeId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public bool Enabled { get; set; }
    public bool IsDeleted { get; set; }
    public EntityBase<int> Team { get; set; }
    public string Remark { get; set; }

    [JsonIgnore]
    public string PickupRemark { get; set; }
    public int DisplayOrder { get; set; }
    public ShiftBase DefaultShift { get; set; }
    public short OffDaysbyWeek { get; set; }
    public bool IsUsedWeeklyDefaultShift { get; set; }
    public WeeklyDefaultShift WeeklyDefaultShift { get; set; }

    public string Modifier { get; set; }
    public DateTime ModifiedDate { get; set; }

#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S1067 // Expressions should not be too complex
    public bool IsPublic => MondaySchedule is { IsPublic: true }
                            && TuesdaySchedule is { IsPublic: true }
                            && WednesdaySchedule is { IsPublic: true }
                            && ThursdaySchedule is { IsPublic: true }
                            && FridaySchedule is { IsPublic: true }
                            && SaturdaySchedule is { IsPublic: true }
                            && SundaySchedule is { IsPublic: true };
#pragma warning restore S1067 // Expressions should not be too complex

    public bool IsSelected { get; set; }

    public DailySchedule MondaySchedule { get; set; } = new();
    public DailySchedule TuesdaySchedule { get; set; } = new();
    public DailySchedule WednesdaySchedule { get; set; } = new();
    public DailySchedule ThursdaySchedule { get; set; } = new();
    public DailySchedule FridaySchedule { get; set; } = new();
    public DailySchedule SaturdaySchedule { get; set; } = new();
    public DailySchedule SundaySchedule { get; set; } = new();
}