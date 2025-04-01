namespace HRM.Models;

public class LeaveSummary
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int TeamId { get; set; }
    public string Team { get; set; }
    public string Section { get; set; }
    public string Department { get; set; }

    public float AnnualLeaveLastYear { get; set; }
    public float CreditKIV { get; set; }
    public float AnnualLeaveCurrentYear { get; set; }

    public float Jan { get; set; }
    public float Feb { get; set; }
    public float Mar { get; set; }
    public float Apr { get; set; }
    public float May { get; set; }
    public float Jun { get; set; }
    public float Jul { get; set; }
    public float Aug { get; set; }
    public float Sep { get; set; }
    public float Oct { get; set; }
    public float Nov { get; set; }
    public float Dec { get; set; }

    public float LeaveBalance { get; set; }
    public float KIVBalance { get; set; }

    public string Remark { get; set; }
}