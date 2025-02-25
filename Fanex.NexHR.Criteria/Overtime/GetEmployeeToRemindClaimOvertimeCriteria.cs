namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;

    public class GetEmployeeToRemindClaimOvertimeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Overtime_GetEmployeeToRemindClaim";

        public override bool IsValid() => true;
    }
}
