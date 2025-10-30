using Fanex.Data.Repository;
using HRM.Core.Enums;

namespace HRM.Criteria.EmployeePayrollSetting;

public class GetPayrollSettingTypeCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public PayrollSettingType PayrollSettingCategory { get; set; }

    public override string GetSettingKey()
    {
        return "PayrollItemSettingType_Get";
    }

    public override bool IsValid() => true;
}

