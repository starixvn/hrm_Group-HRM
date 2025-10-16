using Fanex.Data.Repository;

namespace HRM.Criteria.CompanySite;

public class GetAllCompanySiteCriteria : CriteriaBase
{
    public override string GetSettingKey() => "CompanySite_GetAll";

    public override bool IsValid() => true;
}