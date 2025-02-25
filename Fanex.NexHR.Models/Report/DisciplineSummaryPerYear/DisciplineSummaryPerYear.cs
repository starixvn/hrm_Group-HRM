namespace Fanex.NexHR.Models.Report
{
    using System;
    using System.Collections.Generic;

    public class DisciplineSummaryPerYear
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public bool IsLocal { get; set; }
        public string Team { get; set; }
        public string Section { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTime? CompanyDate { get; set; }
        public DateTime? MilitaryFromDate { get; set; }
        public DateTime? MilitaryToDate { get; set; }
        public string LengthOfService { get; set; }

        public IList<DisciplineSummaryPerYearDetail> Disciplines { get; set; }
    }
}