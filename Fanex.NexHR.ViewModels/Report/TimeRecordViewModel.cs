namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class TimeRecordViewModel : PagerOptionModel
    {
        public DateTime FromDate { get; set; } = DateTimeOffset.Now.Date;
        public DateTime ToDate { get; set; } = DateTimeOffset.Now.Date;
        public int EmployeeId { get; set; }
        public string Employee { get; set; }
        public string TeamIds { get; set; }
        public IList<Models.Team> Teams { get; set; }
        public byte? IsLocal { get; set; }
    }
}