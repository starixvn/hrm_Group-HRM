namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class InsertSalaryStructureCriteria : CriteriaBase
{
    public string SalaryStructureName { get; set; }

    public int JobLevelId { get; set; }

    public int? LocalSalary { get; set; }

    public int? ExpatSalary { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "SalaryStructure_Insert";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(SalaryStructureName)
               && JobLevelId > 0;
    }
}