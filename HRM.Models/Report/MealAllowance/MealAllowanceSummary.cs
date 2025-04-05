namespace HRM.Models.Report.MealAllowance;

public class MealAllowanceSummary
{
    public decimal TotalDays { get; set; }
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string NickName { get; set; }
    public string TeamCode { get; set; }
    public string TeamName { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
}