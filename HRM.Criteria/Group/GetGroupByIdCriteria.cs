namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class GetGroupByIdCriteria : CriteriaBase
{
    public int TeamId { get; set; }

    public int CompanyId { get; set; }

    public override string GetSettingKey() => "Group_Get";

    public override bool IsValid() => TeamId > 0;
}