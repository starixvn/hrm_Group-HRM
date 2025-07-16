namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class InsertSalaryStructureCriteria : CriteriaBase
{
    public int CompanyId { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public int CreatedBy { get; set; }

    public override string GetSettingKey() => "SalaryStructure_Insert";

    public override bool IsValid() => CompanyId > 0 && CreatedBy > 0 && !string.IsNullOrWhiteSpace(Name);
}