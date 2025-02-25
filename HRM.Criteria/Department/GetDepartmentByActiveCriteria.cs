namespace HRM.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetDepartmentByActiveCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Department_Get";

        public override bool IsValid() => true;
    }
}