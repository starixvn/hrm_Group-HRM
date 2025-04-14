namespace HRM.Models.Report.MealAllowance;

using System;

public class MealAllowanceDetail
{
    public DateTime WorkDate { get; set; }
    public int ShiftId { get; set; }
    public string ShiftCode { get; set; }
    public DateTime InFixed { get; set; }
    public DateTime OutFixed { get; set; }
    public decimal TotalDays { get; set; }
    public decimal TotalWorkHours { get; set; }
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string NickName { get; set; }
    public string TeamCode { get; set; }
    public string TeamName { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
}