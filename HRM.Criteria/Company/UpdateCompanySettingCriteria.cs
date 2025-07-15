using Fanex.Data.Repository;

namespace HRM.Criteria.Company;

public class UpdateCompanySettingCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public string HasBIO { get; set; }

    public string EnableLetstalk { get; set; }

    public string WhitelistIPs { get; set; }

    public string Currency { get; set; }

    public override string GetSettingKey() => "SystemConfigs_UpdateCompanySetting";

    public override bool IsValid() => CompanyId > 0;
}