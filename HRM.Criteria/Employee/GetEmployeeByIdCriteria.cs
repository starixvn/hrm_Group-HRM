namespace HRM.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey() => "Employee_GetEmployeeByID";

        public override bool IsValid() => EmpId > 0;
    }
}