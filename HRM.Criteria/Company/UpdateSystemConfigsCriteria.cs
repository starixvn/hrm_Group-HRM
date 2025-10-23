using Fanex.Data.Repository;

namespace HRM.Criteria.Company;

public class UpdateSystemConfigsCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public string SettingsJson { get; set; }

    public int ModifiedBy { get; set; }

    public int ObjectTypeId { get; set; }

    public override string GetSettingKey() => "SystemConfigs_Update";

    public override bool IsValid() =>
        CompanyId > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(SettingsJson);
}
