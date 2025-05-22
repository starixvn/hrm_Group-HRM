namespace HRM.Criteria.AnnualLeave;

using System;
using Fanex.Data.Repository;

public class GetAnnualLeaveBalanceForJoinedDateCriteria : CriteriaBase
{
    public int? EmpId { get; set; }

    public string TeamIds { get; set; }

    public DateTime FromDate { get; set; }

    public byte? IsLocal { get; set; }

    public int? SupervisorId { get; set; }

    public string Role { get; set; }

    public int CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "Report_LeaveSummaryForJoinedDate";
    }

    public override bool IsValid()
    {
        return true;
    }
}