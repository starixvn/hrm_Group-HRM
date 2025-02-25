namespace HRM.Criteria.Department
{
    using Fanex.Data.Repository;

    public class DeleteDepartmentCriteria : CriteriaBase
    {
        public int departmentId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Delete";
        }

        public override bool IsValid()
        {
            return departmentId > 0;
        }
    }
}