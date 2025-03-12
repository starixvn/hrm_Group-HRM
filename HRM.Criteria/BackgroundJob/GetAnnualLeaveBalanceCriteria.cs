namespace HRM.Criteria.BackgroundJob;

using Fanex.Data.Repository;

public class GetAnnualLeaveBalanceCriteria : CriteriaBase
{
    public int Year { get; set; }

    public override string GetSettingKey() => "Auto_AnnualLeaveBalance_Get";

    public override bool IsValid() => true;
}