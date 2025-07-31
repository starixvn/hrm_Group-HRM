namespace HRM.Criteria.PayrollManagement;

using System;
using Fanex.Data.Repository;

public class GetPayrollAttendantEmployeeCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public DateTime Date { get; set; }

    public override string GetSettingKey()
    {
        return "Payroll_GetAttendantEmployee";
    }

    public override bool IsValid()
    {
        return CompanyId > 0 && Date != DateTime.MinValue;
    }
}