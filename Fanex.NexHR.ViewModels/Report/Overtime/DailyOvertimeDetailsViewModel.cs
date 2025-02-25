namespace Fanex.NexHR.ViewModels.Report.Overtime
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report.Overtime;

    public class DailyOvertimeDetailsViewModel
    {
        public DateTime FromDate { get; set; } = DateTimeOffset.Now.AddMonths(-1).Date;
        public DateTime ToDate { get; set; } = DateTimeOffset.Now.Date;
        public string TeamIds { get; set; }
        public IEnumerable<EntityBase<int>> Teams { get; set; }
        public string Employee { get; set; }
        public byte? IsLocal { get; set; }
        public byte? EmployeeStatusId { get; set; } = (byte)EmployeeStatus.Enabled;
        public IEnumerable<DailyOvertimeDetails> Overtimes { get; set; } = new List<DailyOvertimeDetails>();
    }
}