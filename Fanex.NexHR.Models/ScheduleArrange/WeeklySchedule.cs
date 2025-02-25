namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;
    using Newtonsoft.Json;

    public class WeeklySchedule
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
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
        public bool IsPublic => MondaySchedule != null && MondaySchedule.IsPublic
#pragma warning restore S1541 // Methods and properties should not be too complex
            && TuesdaySchedule != null && TuesdaySchedule.IsPublic
            && WednesdaySchedule != null && WednesdaySchedule.IsPublic
            && ThursdaySchedule != null && ThursdaySchedule.IsPublic
            && FridaySchedule != null && FridaySchedule.IsPublic
            && SaturdaySchedule != null && SaturdaySchedule.IsPublic
            && SundaySchedule != null && SundaySchedule.IsPublic;
#pragma warning restore S1067 // Expressions should not be too complex

        public bool IsSelected { get; set; }

        public DailySchedule MondaySchedule { get; set; }
        public DailySchedule TuesdaySchedule { get; set; }
        public DailySchedule WednesdaySchedule { get; set; }
        public DailySchedule ThursdaySchedule { get; set; }
        public DailySchedule FridaySchedule { get; set; }
        public DailySchedule SaturdaySchedule { get; set; }
        public DailySchedule SundaySchedule { get; set; }
    }
}