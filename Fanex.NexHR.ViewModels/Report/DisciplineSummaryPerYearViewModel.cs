namespace Fanex.NexHR.ViewModels.Report
{
    public class DisciplineSummaryPerYearViewModel
    {
        public int FromYear { get; set; }
        public int ToYear { get; set; }
        public string TeamId { get; set; }
        public bool? IsLocal { get; set; }
        public string Employee { get; set; }
        public int? EmployeeId { get; set; }
        public bool IsShowAll { get; set; }
        public bool LengthOfService { get; set; }
    }
}