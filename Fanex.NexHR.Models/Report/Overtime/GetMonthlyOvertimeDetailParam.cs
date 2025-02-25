namespace Fanex.NexHR.Models.Report.Overtime
{
    using Fanex.NexHR.Core.Enums;

    public class GetMonthlyOvertimeDetailParam
    {
        public int EmployeeId { get; set; }
        public byte Month { get; set; }
        public int Year { get; set; }
        public CalendarType Calendar { get; set; }
    }
}