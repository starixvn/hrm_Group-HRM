namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class GetAllGroupCriteria : CriteriaBase
{
    public bool IsDeleted { get; set; }
        
    public string Keyword { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Group_Get";

    public override bool IsValid() => true;
}