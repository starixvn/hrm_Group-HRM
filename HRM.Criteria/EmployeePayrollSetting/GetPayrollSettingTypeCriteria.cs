using Fanex.Data.Repository;

namespace HRM.Criteria.EmployeePayrollSetting;

public class GetPayrollSettingTypeCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "PayrollItemSettingType_Get";
    }

    public override bool IsValid() => true;
}

