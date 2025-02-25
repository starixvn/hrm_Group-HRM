namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class EditDefaultShift
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public int? ShiftId { get; set; }
    }
}