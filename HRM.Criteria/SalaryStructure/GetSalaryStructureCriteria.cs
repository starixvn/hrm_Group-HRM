namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class GetSalaryStructureCriteria : CriteriaBase
{
    public override string GetSettingKey() => "SalaryStructure_Get";

    public override bool IsValid() => true;
}