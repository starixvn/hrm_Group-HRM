namespace Fanex.NexHR.Models.Request
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Constants;

    public class LeaveRequest : BaseRequest
    {
        public long HistoryId { get; set; }
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal TotalDays { get; set; }

        public LeaveType LeaveType { get; set; }

        public IList<LeaveRequestDetail> LeaveDetail { get; set; } = new List<LeaveRequestDetail>();

        public bool IsHalfDay => TotalDays == Constants.HalfDayOffValue;

        public bool IsHospitalApplication { get; set; }

    }
}