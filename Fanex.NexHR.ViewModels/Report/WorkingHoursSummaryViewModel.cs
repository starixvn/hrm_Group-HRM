namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;

    public class WorkingHoursSummaryViewModel
    {
        private static DateTime Today = DateTimeOffset.Now.Date;

        public byte DateOption { get; set; } = (byte)DateRangeOption.CurrentMonth;
        public DateTime FromDate { get; set; } = Today.FirstDayOfMonth();
        public DateTime ToDate { get; set; } = Today;
        public byte? IsLocal { get; set; }
        public string TeamIds { get; set; }
        public string Employee { get; set; }
        public byte? EmployeeStatusId { get; set; } = (byte)EmployeeStatus.Enabled;

        public IEnumerable<EntityBase<int>> Teams { get; set; }
        public IEnumerable<WorkingHoursSummary> WorkingHours { get; set; } = new List<WorkingHoursSummary>();
    }
}