namespace HRM.Models.Overtime;

using System;
using System.Collections.Generic;
using HRM.Models.Request;

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
    public string Team => Requester.Team;
    public string Section => Requester.Section;
    public string Department => Requester.Department;

    public IEnumerable<DailyOvertimeRequest> DailyOvertimes { get; set; } = new List<DailyOvertimeRequest>();
}