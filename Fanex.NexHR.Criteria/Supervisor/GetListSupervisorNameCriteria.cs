namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class GetListSupervisorNameCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int? Role { get; set; }

        public int? RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetListSupervisorByEmpID";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
