namespace HRM.Models.Overtime;

using System;
using System.Collections.Generic;
using Request;

public class MonthlyOvertimeRequest
{
    public long RequestId { get; set; }
    public DateTime Month { get; set; }
    public Requester Requester { get; set; }
    public Requester Verifier { get; set; }
    public Requester Approver { get; set; }

    public decimal NormalHours { get; set; }
    public decimal OffHours { get; set; }
    public decimal PublicHours { get; set; }
    public decimal SpecialHours { get; set; }
    public decimal TotalHours => NormalHours + OffHours + PublicHours + SpecialHours;
    public RequestStatus Status { get; set; }
    public DateTime? VerifiedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public bool IsWarning { get; set; }
    public bool IsHost { get; set; }
    public string Nickname => Requester.Nickname;
    public string FullName => Requester.FullName;

    public string BUCode { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
    public string TeamCode { get; set; }
    public string TeamName { get; set; }

    public int VerifierId { get; set; }
    public string VerifierNickname { get; set; }
    public string VerifierFullName { get; set; }

    public int ApproverId { get; set; }
    public string ApproverNickname { get; set; }
    public string ApproverFullName { get; set; }

    public IEnumerable<DailyOvertimeRequest> DailyOvertimes { get; set; } = new List<DailyOvertimeRequest>();
}