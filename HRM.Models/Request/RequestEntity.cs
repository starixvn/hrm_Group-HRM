namespace HRM.Models.Request;

using System;
using System.Collections.Generic;
using ChangeShift;
using Core;
using HRM.Core.Models;
using Shifts;

public class RequestEntity : EntityBase<long>
{
    private DateTime serverRequestDate;
    private DateTime serverCreatedDate;

    public Employee Requester { get; set; }

    public DateTime RequestedDate
    {
        get => serverRequestDate;
        set => serverRequestDate = value.ToClientTimeZone();
    }

    public DateTime? OriginalRequestedDate { get; set; }

    public string CustomId => $"{Id}_{CompanyId}";

    public RequestTypeModel RequestType { get; set; }

    public RequestStatus Status { get; set; }

    public DateTime CreatedDate
    {
        get => serverCreatedDate;
        set => serverCreatedDate = value.ToClientTimeZone();
    }

    public DateTime? OriginalCreatedDate { get; set; }

    public Employee VerifiedBy { get; set; }
    public Employee ApprovedBy { get; set; }

    public int VerifierId { get; set; }
    public string VerifierNickName { get; set; }
    public string VerifierFullName { get; set; }

    public int ApproverId { get; set; }
    public string ApproverNickName { get; set; }
    public string ApproverFullName { get; set; }

    public string ProcessedByNickName { get; set; }
    public string ProcessedByFullName { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public DateTime? VerifiedDate { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public string Remark { get; set; }

    public IEnumerable<S3FileObject> AttachFiles { get; set; }

    public string RefNO { get; set; }

    public decimal TotalDaysRequestInMonth { get; set; }

    public int VerifiedStatus { get; set; }

    public int ApprovedStatus { get; set; }

    public int ProcessedStatus { get; set; }

    public bool IsWarning { get; set; }

    // Leave
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public decimal TotalDays { get; set; }

    public LeaveType LeaveType { get; set; }

    public int ReworkCount { get; set; }

    public string LeaveCountDetails { get; set; }

    public decimal AnnualLeaveBalance { get; set; }

    // ChangeShift
    public Shift FromShift { get; set; }

    public Shift ToShift { get; set; }

    public byte ChangeShiftType { get; set; }

    public IEnumerable<ChangeShiftDetail> ChangeShiftDetails { get; set; }

    // LateIn-EarlyOut
    public int TotalMins { get; set; }

    public int ActualLateInEarlyOut { get; set; }
    public byte ReasonId { get; set; }

    // Overtime
    public short OvertimeType { get; set; }

    public bool IsOvertimeHours { get; set; }

    public decimal NormalHours { get; set; }

    public decimal OffHours { get; set; }

    public decimal PublicHours { get; set; }

    public decimal ToTalHours { get; set; }

    public bool IsShowWarning { get; set; }

    // Keep In View
    public DateTime FromTime { get; set; }

    public DateTime ToTime { get; set; }

    public bool IsHalfDay { get; set; }

    public bool CrossDay { get; set; }

    public byte KeepInViewType { get; set; }

    public int CompanyId { get; set; }

    public int RequestTimeZone { get; set; }
}