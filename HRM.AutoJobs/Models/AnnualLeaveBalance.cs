namespace HRM.AutoJobs.Models;

public class AnnualLeaveBalance
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }

    public string Email { get; set; }
    public bool IsLocal { get; set; }

    public float AnnualLeaveCurrentYear { get; set; }

    public float LeaveBalance { get; set; }
    public string Letstalk { get; set; }

    public int SectionId { get; set; }
    public string Section { get; set; }

    public int DepartmentId { get; set; }
    public string Department { get; set; }
}