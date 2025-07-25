using Fanex.Data.Repository;

namespace HRM.Criteria.Company;

public class UpdateSubCompanyCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public int SubCompanyId { get; set; }

    public string Name { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "SubCompany_Update";

    public override bool IsValid() =>
        CompanyId > 0 && SubCompanyId > 0 && !string.IsNullOrWhiteSpace(Name);
}