namespace HRM.Criteria.Overtime;

using Fanex.Data.Repository;

public class GetEmployeeToRemindClaimOvertimeCriteria : CriteriaBase
{
    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Overtime_GetEmployeeToRemindClaim";

    public override bool IsValid() => true;
}