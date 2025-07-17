namespace HRM.Criteria.Company;

using Fanex.Data.Repository;

public class GetByIdCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public override string GetSettingKey() => "CompanySetting_GetById";

    public override bool IsValid() => CompanyId > 0;
}