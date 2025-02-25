namespace HRM.Models.BackgroundJob
{
    public class SectionStatisticAnnualLeave
    {
        public int SectionId { get; set; }
        public string Section { get; set; }

        public int DepartmentId { get; set; }
        public string Department { get; set; }

        public int TotalEmployee { get; set; }

        public int AnnualLeaveBalance { get; set; }

        public double Percentage { get; set; }
    }
}