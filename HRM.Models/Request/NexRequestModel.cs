using System;

namespace HRM.Models.Request;

public class NexRequestModel
{
    public int RequestId { get; set; }
 
    public string UnitCode { get; set; }

    public string DepartmentName { get; set; }

    public string TeamName { get; set; }

    public int RequestType { get; set; }

    public int LeaveTypeID { get; set; }

    public string LeaveTypeName { get; set; }

    public string NickName { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public decimal? TotalDays { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Remark { get; set; }

    public bool CrossDay { get; set; }

    public byte Status { get; set; }

    public bool IsHalfDay { get; set; }

    public int? TotalMinutes { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int RequesterId { get; set; }

    public string RequesterNickName { get; set; }

    public string RequesterFullName { get; set; }

    public int ApproverId { get; set; }

    public string ApproverNickName { get; set; }

    public int CompanyId { get; set; }
}