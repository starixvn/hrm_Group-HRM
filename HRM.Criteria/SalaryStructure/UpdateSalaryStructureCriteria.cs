namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class UpdateSalaryStructureCriteria : CriteriaBase
{
    public int SalaryStructureId { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public int ModifiedBy { get; set; }
    public int CompanyId { get; set; }

    public override string GetSettingKey() => "SalaryStructure_Update";

    public override bool IsValid() => SalaryStructureId > 0 && CompanyId > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(Name);
}