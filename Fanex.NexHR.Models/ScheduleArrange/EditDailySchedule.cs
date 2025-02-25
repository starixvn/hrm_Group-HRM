namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class EditDailySchedule
    {
        public int EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }
        public int? ShiftId { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsOffDay { get; set; }
        public bool IsFixedOffDay { get; set; }
        public bool IsPublic { get; set; }
    }
}