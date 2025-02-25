namespace HRM.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetDepartmentByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "Department_GetById";

        public override bool IsValid() => Id > 0;
    }
}