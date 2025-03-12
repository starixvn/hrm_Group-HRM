namespace HRM.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Models;

    public class WorkingCalendarSummaryViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte? IsLocal { get; set; }
        public string TeamIds { get; set; }
        public string Employee { get; set; }
        public byte? EmployeeStatusId { get; set; }

        public IEnumerable<EntityBase<int>> Teams { get; set; }
    }
}