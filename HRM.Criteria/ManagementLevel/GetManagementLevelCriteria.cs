using Fanex.Data.Repository;

namespace HRM.Criteria.ManagementLevel;

public class GetManagementLevelCriteria : CriteriaBase
{
    public override string GetSettingKey() => "ManagementLevel_Get";

    public override bool IsValid() => true;
}