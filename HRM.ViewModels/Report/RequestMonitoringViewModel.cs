namespace HRM.ViewModels.Report
{
    using System;

    public class RequestMonitoringViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int MaxRequest { get; set; }

        public int? EmpId { get; set; }

        public string Team { get; set; }

        public byte? IsLocal { get; set; }

        public bool IsExcel { get; set; }

        public string ViewRole { get; set; }
    }
}
