using Fanex.Data.Repository;

namespace HRM.Criteria.SystemSetting;

public class GetDomainByCompanyIdCriteria : CriteriaBase
{
    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Domain_ByCompanyId_Get";

    public override bool IsValid() => CompanyId > 0;
}