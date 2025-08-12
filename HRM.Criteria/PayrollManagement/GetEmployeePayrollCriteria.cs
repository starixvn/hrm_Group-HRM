using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.PayrollManagement;

public class GetPayrollByMonthCriteria : CriteriaBase
{
    public DateTime? Month { get; set; }

    public int CompanyId { get; set; }
    public bool IsLocal { get; set; }

    public override string GetSettingKey() => "PayrollManagement_GetByMonth";

    public override bool IsValid() => CompanyId > 0;
}