using Fanex.Data.Repository;

namespace HRM.Criteria.Company;

public class GetAllCompanyCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Company_GetAll";

    public override bool IsValid() => true;
}