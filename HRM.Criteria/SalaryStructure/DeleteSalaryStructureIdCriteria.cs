namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class DeleteSalaryStructureIdCriteria : CriteriaBase
{
    public int SalaryStructureId { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "SalaryStructure_Delete";
    }

    public override bool IsValid()
    {
        return SalaryStructureId > 0;
    }
}