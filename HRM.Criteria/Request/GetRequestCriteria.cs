namespace HRM.Criteria.Request;

using System;
using Fanex.Data;
using Fanex.Data.Repository;
using Core.Enums;

public class GetRequestCriteria : CriteriaBase
{
    public int? SupervisorId { get; set; }

    public int? EmpId { get; set; }

    public string TeamIds { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Status { get; set; }

    public short CompanyId { get; set; }

    public string Role
    {
        get
        {
            string role;
            switch (RequestViewType)
            {
                case RequestViewType.PersonalSpace:
                    role = ((short)RoleSystem.Verifier).ToString() + "," + ((short)RoleSystem.Approver).ToString();
                    break;

                case RequestViewType.HRSpace:
                    role = ((short)RoleSystem.HR).ToString();
                    break;

                default:
                    role = ((short)RoleSystem.Member).ToString();
                    break;
            }

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