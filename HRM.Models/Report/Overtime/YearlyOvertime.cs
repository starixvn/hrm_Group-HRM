namespace HRM.Models.Report.Overtime;

using HRM.Models.Common;

public class YearlyOvertime : MonthlyDataBase
{
    public int EmpId { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public bool Enabled { get; set; }
    public bool IsDeleted { get; set; }
    public string TeamCode { get; set; }
    public string TeamName { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
}