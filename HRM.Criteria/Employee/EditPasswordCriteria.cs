namespace HRM.Criteria.Employee;

using Fanex.Data.Repository;

public class EditPasswordCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public string Password { get; set; }

    public bool PasswordReset { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "Employee_ChangePassword";

    public override bool IsValid() => EmpId > 0;
}