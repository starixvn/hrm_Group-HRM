namespace HRM.Criteria.Supervisor;

using Fanex.Data.Repository;
    
public class GetEmployeeSupervisorByQueryCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int EmpId { get; set; }

    public byte Role { get; set; }

    public byte? RequestType { get; set; }

    public string Query { get; set; }

    public bool IncludeLoginUser { get; set; }

    public override string GetSettingKey() => "EmployeeSupervisor_GetByQuery";

    public override bool IsValid() => EmpId > 0;
}