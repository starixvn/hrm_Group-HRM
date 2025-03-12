namespace HRM.Criteria.Department;

using Fanex.Data.Repository;

public class InsertDepartmentCriteria : CriteriaBase
{
    public string Code { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public int  UnitId { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "Department_Insert";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Code) 
               && !string.IsNullOrWhiteSpace(Name)
               && UnitId > 0;
    }
}