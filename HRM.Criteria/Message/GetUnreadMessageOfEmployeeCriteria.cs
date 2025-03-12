namespace HRM.Criteria.Message;

using Fanex.Data.Repository;

public class GetUnreadMessageOfEmployeeCriteria : CriteriaBase
{
    public int EmployeeId { get; set; }

    public override string GetSettingKey() => "Message_GetUnreadMessageByEmployee";

    public override bool IsValid() => EmployeeId > 0;
}