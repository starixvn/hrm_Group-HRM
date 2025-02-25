namespace HRM.Models.Report.Overtime
{
    using HRM.Core.Enums;

    public class GetMonthlyOvertimeDetailParam
    {
        public int EmployeeId { get; set; }
        public byte Month { get; set; }
        public int Year { get; set; }
        public CalendarType Calendar { get; set; }
    }
}