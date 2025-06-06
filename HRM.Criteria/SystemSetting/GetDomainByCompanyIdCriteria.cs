using Fanex.Data.Repository;

namespace HRM.Criteria.SystemSetting;

public class GetDomainByCompanyIdCriteria : CriteriaBase
{
    public int CompanyId { get; set; }
    public override string GetSettingKey()
    {
        return "Domain_ByCompanyId_Get";
    }
    public override bool IsValid()
    {
        return CompanyId > 0;
    }
}