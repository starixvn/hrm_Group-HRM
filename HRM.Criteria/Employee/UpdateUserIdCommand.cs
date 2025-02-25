namespace HRM.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class UpdateUserIdCommand : CriteriaBase
    {
        public int EmpId { get; set; }
        public int UserId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Employee_UpdateUserId";

        public override bool IsValid() => EmpId > 0 && UserId > 0 && ModifiedBy > 0;
    }
}