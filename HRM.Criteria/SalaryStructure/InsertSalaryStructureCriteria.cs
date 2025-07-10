namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class InsertSalaryStructureCriteria : CriteriaBase
{
    public string Name { get; set; }

    public int JobLevelId { get; set; }

    public string LocalSalary { get; set; }

    public string ExpatSalary { get; set; }

    public int CreatedBy { get; set; }

    public int CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "SalaryStructure_Insert";
    }

    public override bool IsValid()
    {
        return IsValidId(JobLevelId) &&
            IsValidId(CompanyId) &&
            IsValidId(CreatedBy) &&
            !string.IsNullOrWhiteSpace(Name);
    }

    private static bool IsValidId(int value) => value > 0;
}