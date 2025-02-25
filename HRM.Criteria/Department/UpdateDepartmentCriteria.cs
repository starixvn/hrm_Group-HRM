namespace HRM.Criteria.Department
{
    using Fanex.Data.Repository;

    public class UpdateDepartmentCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UnitId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Update";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0 
                && !string.IsNullOrWhiteSpace(Code) 
                && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
