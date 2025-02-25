namespace HRM.ViewModels.Report.WorkingHours
{
    using System;
    using System.Collections.Generic;
    using HRM.Models;

    public class WeeklyFilterViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TeamIds { get; set; }
        public IEnumerable<EntityBase<int>> Teams { get; set; }
        public string Employee { get; set; }
        public byte? IsLocal { get; set; }
        public byte? EmployeeStatusId { get; set; }
    }
}