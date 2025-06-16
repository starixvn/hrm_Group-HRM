namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class CreateTeamCriteria : CriteriaBase
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int DepartmentId { get; set; }
    public int? PICEmpId { get; set; }
    public int GroupTeamId { get; set; }
    public string AccountableArea { get; set; }
    public int CreatedBy { get; set; }
    public short CompanyId { get; set; }
    public bool IsOutsource { get; set; }

    public override string GetSettingKey()
    {
        return "Group_Insert";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Code)
               && !string.IsNullOrWhiteSpace(Name)
               && DepartmentId > 0;
    }
}