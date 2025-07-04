namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class GetSalaryStructureCriteria : CriteriaBase
{
    public int SalaryStructureId { get; set; }

    public override string GetSettingKey() => "SalaryStructure_GetById";

    public override bool IsValid() => SalaryStructureId > 0;
}