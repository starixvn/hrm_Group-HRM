namespace HRM.Models.HospitalizationLeave;

using HRM.Models.Common;

public class HospitalizationLeaveBalanceSummaryReport : MonthlyDataBase
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public bool Enabled { get; set; }
    public bool IsDeleted { get; set; }
    public string Team { get; set; }
    public string Department { get; set; }

    public float Balance { get; set; }
    public float RemainingBalance { get; set; }
}