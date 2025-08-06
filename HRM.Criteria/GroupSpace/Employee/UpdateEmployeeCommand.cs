using Fanex.Data.Repository;

namespace HRM.Criteria.GroupSpace.Employee;

public class UpdateEmployeeCommand : CriteriaBase
{
    public int EmpId { get; set; }
    public int? CtrId { get; set; }
    public int ModifiedBy { get; set; }

    public override bool IsValid() => EmpId > 0 && ModifiedBy > 0;

    public override string GetSettingKey() => "Employee_GroupScope_Update";
}