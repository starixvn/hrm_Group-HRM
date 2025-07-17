using Fanex.Data.Repository;

namespace HRM.Criteria.Company;

public class UpdateCompanySettingCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public string SettingsJson { get; set; }

    public override string GetSettingKey() => "SystemConfigs_Update";

    public override bool IsValid() =>
        CompanyId > 0 && !string.IsNullOrWhiteSpace(SettingsJson);
}