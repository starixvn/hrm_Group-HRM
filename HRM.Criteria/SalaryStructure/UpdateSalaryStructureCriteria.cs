namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class UpdateSalaryStructureCriteria : CriteriaBase
{
    public int SalaryStructureId { get; set; }

    public string Name { get; set; }

    public int JobLevelId { get; set; }

    public string LocalSalary { get; set; }

    public string ExpatSalary { get; set; }

    public int ModifiedBy { get; set; }

    public int CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "SalaryStructure_Update";
    }

    public override bool IsValid()
    {
        return IsValidId(SalaryStructureId) &&
            IsValidId(JobLevelId) &&
            IsValidId(CompanyId) &&
            IsValidId(ModifiedBy) &&
            !string.IsNullOrWhiteSpace(Name);
    }

    private static bool IsValidId(int value) => value > 0;
}