namespace HRM.Criteria.Company;

using Fanex.Data.Repository;

public class CountEmployeeByIdCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public override string GetSettingKey() => "SubCompany_CountEmployeeById";

    public override bool IsValid() => CompanyId > 0;
}