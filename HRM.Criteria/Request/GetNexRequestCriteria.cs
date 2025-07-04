using HRM.Core.Constants;

namespace HRM.Criteria.Request;

using System;
using Core.Enums;
using Fanex.Data;
using Fanex.Data.Repository;

public class GetNexRequestCriteria : CriteriaBase
{
    public int? ActorId { get; set; }

    public int? EmpId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int? Status { get; set; }

    public string RequestType { get; set; }

    public bool isFilterByEffectedDate { get; set; }

    public int? LeaveTypeId { get; set; }

    public int Role { get; set; }

    public int ExcludedEmpID { get; set; }

    [SpParam(Ignored = true)]
    public int? CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return CompanyId switch
        {
            Constants.NexCompanyId => "Nex_WorkingTimeRequest_GetRequestByDate",
            Constants.LumiCompanyId => "Lumi_WorkingTimeRequest_GetRequestByDate",
            Constants.NexVisionCompanyId => "NexVision_WorkingTimeRequest_GetRequestByDate",
            _ => "Nex_WorkingTimeRequest_GetRequestByDate"
        };
    }

    public override bool IsValid()
    {
        return FromDate <= ToDate;
    }
}