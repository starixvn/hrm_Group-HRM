namespace Fanex.NexHR.Models.Report.Overtime
{
    using Fanex.NexHR.Models.Overtime;

    public class DailyOvertimeDetails : DailyOvertimeRequest
    {
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Team { get; set; }
        public string DepartmentCode { get; set; }
        public string Section { get; set; }

    }
}