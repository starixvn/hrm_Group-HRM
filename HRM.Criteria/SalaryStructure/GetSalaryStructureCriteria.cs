namespace HRM.Criteria.SalaryStructure;

using Fanex.Data.Repository;

public class GetSalaryStructureCriteria : CriteriaBase
{
    public int? CompanyId { get; set; }

    public override string GetSettingKey() => "SalaryStructure_Get";

    public override bool IsValid() => CompanyId > 0;
}