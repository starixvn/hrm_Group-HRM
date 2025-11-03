namespace HRM.Criteria.Employee;

using Fanex.Data.Repository;

public class GetEmployeeTransferCriteria : CriteriaBase
{
    public int CurrentEmpId { get; set; }

    public bool IsViewAll { get; set; }

    public override string GetSettingKey() => "EmployeeTransfer_GetList";

    public override bool IsValid() => CurrentEmpId > 0;
}

