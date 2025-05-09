namespace HRM.Criteria.EmployeePayroll;

using Fanex.Data.Repository;

public class GetSalaryUploadHistoryCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public override string GetSettingKey() => "SalaryUploadHistory_Get";

    public override bool IsValid() => CompanyId > 0;
}