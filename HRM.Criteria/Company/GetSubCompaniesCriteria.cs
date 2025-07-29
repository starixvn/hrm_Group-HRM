using Fanex.Data.Repository;

namespace HRM.Criteria.Company;

public class GetSubCompaniesCriteria : CriteriaBase
{
    public int? CompanyId { get; set; }
    public override string GetSettingKey() => "SubCompanies_ByCompanyId_Get";

    public override bool IsValid() => true;
}