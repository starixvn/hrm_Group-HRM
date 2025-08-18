namespace HRM.Criteria.CompanySite;

using Fanex.Data.Repository;

public class GetDomainCriteria : CriteriaBase
{
    public short? CompanyId { get; set; }

    public override string GetSettingKey() => "Company_GetDomain";

    public override bool IsValid() => true;
}