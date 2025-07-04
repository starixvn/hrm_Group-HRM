namespace HRM.Criteria.Request;

using System;
using Core.Enums;
using Fanex.Data;
using Fanex.Data.Repository;

public class GetNexRequestCriteria : CriteriaBase
{
    public int? SupervisorId { get; set; }

    public int? EmpId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Status { get; set; }

    public int? RequestType { get; set; }

    public bool IsQueryByCreatedDate { get; set; }

    public int? LeaveTypeId { get; set; }

    public override string GetSettingKey()
    {
        return "Nex_WorkingTimeRequest_Get";
    }

    public override bool IsValid()
    {
        return FromDate <= ToDate;
    }
}