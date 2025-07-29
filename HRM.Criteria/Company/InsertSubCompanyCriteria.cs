namespace HRM.Criteria.Company;

using Fanex.Data.Repository;

public class InsertSubCompanyCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public string Name { get; set; }

    public int CreatedBy { get; set; }

    public override string GetSettingKey() => "SubCompany_Insert";

    public override bool IsValid() => CompanyId > 0 && CreatedBy > 0 && !string.IsNullOrWhiteSpace(Name);
}