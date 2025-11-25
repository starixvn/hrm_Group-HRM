using HRM.Core.Constants;

namespace HRM.Models;

using Common;
using System;

public class LeaveSummary : MonthlyDataBase
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }

    public bool Enabled { get; set; }
    public bool IsDeleted { get; set; }
    public int TeamId { get; set; }
    public string Team { get; set; }
    public string Department { get; set; }

    public float AnnualLeaveLastYear { get; set; }
    public float CreditKIV { get; set; }
    public float AnnualLeaveCurrentYear { get; set; }

    public float LeaveBalance { get; set; }
    public float KIVBalance { get; set; }
    public float KIVMinuteBalance { get; set; }
    public double KIVHourBalance => Math.Round(KIVMinuteBalance / Constants.TotalMinuteInHour, 1);
    public string Remark { get; set; }
}