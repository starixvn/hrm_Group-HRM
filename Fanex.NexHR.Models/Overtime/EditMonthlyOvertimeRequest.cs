namespace Fanex.NexHR.Models.Overtime
{
    using System;
    using System.Collections.Generic;

    public class EditMonthlyOvertimeRequest
    {
        public int EmployeeId { get; set; }
        public DateTime Month { get; set; }
        public int VerifierId { get; set; }
        public int ApproverId { get; set; }
        public IEnumerable<DailyOvertimeRequest> DailyOvertimes { get; set; } = new List<DailyOvertimeRequest>();
    }
}