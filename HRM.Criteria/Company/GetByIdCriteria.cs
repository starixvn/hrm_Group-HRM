namespace HRM.Criteria.Company;

using Fanex.Data.Repository;

public class GetByIdCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public override string GetSettingKey() => "Company_GetById";

    public override bool IsValid() => CompanyId > 0;
}