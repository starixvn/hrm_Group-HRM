namespace HRM.ViewModels.Report.Overtime
{
    using System;
    using HRM.Core;

    public class MonthlyFilterViewModel
    {
        public DateTime FromDate { get; set; } = DateTimeOffset.Now.Date.FirstDayOfMonth();
        public DateTime ToDate { get; set; } = DateTimeOffset.Now.Date.LastDayOfMonth();
        public string Employee { get; set; }
        public byte? EmployeeStatusId { get; set; }
        public string TeamIds { get; set; }
        public byte? IsLocal { get; set; }
    }
}