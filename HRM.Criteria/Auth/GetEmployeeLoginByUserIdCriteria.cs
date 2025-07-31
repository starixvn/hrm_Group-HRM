namespace HRM.Criteria.Auth;

using Fanex.Data.Repository;

public class GetEmployeeLoginByUserIdCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int UserId { get; set; }

    public override string GetSettingKey() => "Employee_GetByUserId";

    public override bool IsValid() => UserId > 0;
}