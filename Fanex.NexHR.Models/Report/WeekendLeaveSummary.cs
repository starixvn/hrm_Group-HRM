namespace Fanex.NexHR.Models.Report
{
    public class WeekendLeaveSummary
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public bool IsLocal { get; set; }

        public string TeamId { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public double TotalWeekendLeave { get; set; }
    }
}