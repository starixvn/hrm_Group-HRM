namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class DeleteSalaryStructureCriteria : CriteriaBase
{
    public int SalaryStructureId { get; set; }

    public int DeletedBy { get; set; }

    public override string GetSettingKey()
    {
        return "SalaryStructure_Delete";
    }

    public override bool IsValid()
    {
        return SalaryStructureId > 0 && DeletedBy > 0;
    }
}