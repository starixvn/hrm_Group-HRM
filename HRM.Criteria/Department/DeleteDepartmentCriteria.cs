namespace HRM.Criteria.Department;

using Fanex.Data.Repository;

public class DeleteDepartmentCriteria : CriteriaBase
{
    public int DepartmentId { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "Department_Delete";

    public override bool IsValid() => DepartmentId > 0;
}