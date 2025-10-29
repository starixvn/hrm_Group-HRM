namespace HRM.Criteria.PayrollManagement;

using Fanex.Data.Repository;

public class GetPayrollItemSettingsCriteria : CriteriaBase
{
    public int? CompanyId { get; set; }

    public int? PayrollItemSettingId { get; set; }

    public int? Category { get; set; }

    public override string GetSettingKey()
    {
        return "PayrollItemSettings_Get";
    }

    public override bool IsValid() => true;
}