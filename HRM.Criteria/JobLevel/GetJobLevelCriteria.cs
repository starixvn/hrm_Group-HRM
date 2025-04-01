namespace HRM.Criteria.JobLevel;

using Fanex.Data.Repository;

public class GetJobLevelCriteria : CriteriaBase
{
    public override string GetSettingKey() => "JobLevel_Get";

    public override bool IsValid() => true;
}