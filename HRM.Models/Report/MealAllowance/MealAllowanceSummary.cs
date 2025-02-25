namespace HRM.Models.Report.MealAllowance
{
    public class MealAllowanceSummary
    {
        public decimal TotalDays { get; set; }
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public int Rank { get; set; }
        public int TeamId { get; set; }
        public string Team { get; set; }
        public string Section { get; set; }
        public string Department { get; set; }
    }
}
