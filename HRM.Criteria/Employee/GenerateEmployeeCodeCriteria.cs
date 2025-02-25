namespace HRM.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GenerateEmployeeCodeCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Employee_GenerateEmployeeCode";
    }
}