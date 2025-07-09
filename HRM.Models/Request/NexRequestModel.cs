using System;

namespace HRM.Models.Request;

public class NexRequestModel
{
    public int Id { get; set; }
 
    public string UnitCode { get; set; }

    public string DepartmentName { get; set; }

    public string Team { get; set; }

    public int RequestType { get; set; }

    public int LeaveTypeID { get; set; }

    public string LeaveTypeName { get; set; }

    public string NickName { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public DateTime RequestedDate { get; set; }

    public decimal? TotalDays { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Remark { get; set; }

    public bool IsCrossDate { get; set; }

    public byte Status { get; set; }

    public bool IsHalfDay { get; set; }

    public int? TotalMins { get; set; }

    public int? TotalDaysRequestInMonth { get; set; }

    public string FromShift { get; set; }

    public string ToShift { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int EmpId { get; set; }

    public string Requester { get; set; }

    public string FullNameRequester { get; set; }

    public int ApproverId { get; set; }

    public string ApproverNickName { get; set; }

    public int CompanyId { get; set; }
}