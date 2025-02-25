namespace Fanex.NexHR.Models.Report
{
    using System;

    public class GetShiftStatisticParam
    {
        public int SupervisorId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TeamIds { get; set; }
        public bool IsShowWholeCompany { get; set; }
        public byte? IsLocal { get; set; }
    }
}