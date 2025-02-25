namespace HRM.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class EditEmployeeSupervisorCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public short RoleId { get; set; }
        public string SupervisorXml { get; set; }
        public int ActorId { get; set; }

        public override bool IsValid() => EmployeeId > 0 && ActorId > 0;

        public override string GetSettingKey() => "EmployeeSupervisor_Update";
    }
}