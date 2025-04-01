namespace HRM.Criteria.CompanySite;

using Fanex.Data.Repository;

public class GetCompanySiteByDomainCriteria : CriteriaBase
{
    public string Domain { get; set; }

    public override string GetSettingKey() => "CompanySite_GetByDomain";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Domain);
}