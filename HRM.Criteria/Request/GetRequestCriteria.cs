namespace HRM.Criteria.Request;

using System;
using Core.Enums;
using Fanex.Data;
using Fanex.Data.Repository;

public class GetRequestCriteria : CriteriaBase
{
    public int? SupervisorId { get; set; }

    public int? EmpId { get; set; }

    public string TeamIds { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Status { get; set; }

    public short CompanyId { get; set; }

    public bool IsShowCrossCompany { get; set; }

    public string Role
    {
        get
        {
            var role = RequestViewType switch
            {
                RequestViewType.PersonalSpace => (short)RoleSystem.Verifier + "," + (short)RoleSystem.Approver,
                RequestViewType.HRSpace => ((short)RoleSystem.HR).ToString(),
                _ => ((short)RoleSystem.Member).ToString()
            };

            return role;
        }
    }

    public bool? IsLocal { get; set; }

    public int? RequestType { get; set; }

    [SpParam(Ignored = true)]
    public bool IsQueryByCreatedDate { get; set; }

    [SpParam(Ignored = true)]
    public RequestViewType RequestViewType { get; set; }

    public int? LeaveTypeId { get; set; }

    public override string GetSettingKey()
    {
        return IsQueryByCreatedDate ? "WorkingTimeRequest_GetRequestByCreateDate_AllType" : "WorkingTimeRequest_GetRequestByRequestDate_AllType";
    }

    public override bool IsValid()
    {
        return FromDate <= ToDate;
    }
}