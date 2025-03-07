using Fanex.Data.Repository;

namespace HRM.Criteria.Employee;

public class CheckExistEmployeeCriteria : CriteriaBase
{
    public string Code { get; set; }
    public string Nickname { get; set; }

    public override string GetSettingKey() => "Employee_Insert_CheckExist";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Nickname);
}