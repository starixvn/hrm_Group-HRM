namespace HRM.Models.ScheduleArrange
{
    using System;

    public class LeaveRequest : EntityBase<int>
    {
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
    }
}