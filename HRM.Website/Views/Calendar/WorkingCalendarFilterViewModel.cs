namespace HRM.ViewModels
{
    using System;

    public class WorkingCalendarFilterViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? EmployeeId { get; set; }
        public byte? IsLocal { get; set; }
        public string TeamIds { get; set; }
        public string JobTitleIds { get; set; }
    }
}