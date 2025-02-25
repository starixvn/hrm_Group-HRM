namespace HRM.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetHRAndSupervisorByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey() => "Employee_GetHRAndSupervisorByEmpId";

        public override bool IsValid() => EmpId > 0;
    }
}
