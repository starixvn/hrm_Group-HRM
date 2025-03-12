namespace HRM.Models.Report.Overtime;

using Core.Enums;

public class GetMonthlyOvertimeDetailParam
{
    public short CompanyId { get; set; }
    public int EmployeeId { get; set; }
    public byte Month { get; set; }
    public int Year { get; set; }
    public CalendarType Calendar { get; set; }
}