namespace HRM.Criteria.Employee;

using Fanex.Data.Repository;

public class GetMemberByManagerCriteria : CriteriaBase
{
    public string Query { get; set; }

    public int SupervisorId { get; set; }

    public bool ExcludedManager { get; set; }

    public override string GetSettingKey() => "Employee_GetMemberByManager";

    public override bool IsValid() => true;
}