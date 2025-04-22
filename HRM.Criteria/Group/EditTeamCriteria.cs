namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class EditTeamCriteria : CriteriaBase
{
    public int TeamId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int GroupTeamId { get; set; }
    public int DepartmentId { get; set; }
    public int? PICEmpId { get; set; }
    public string AccountableArea { get; set; }
    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "Group_Update";
    }

    public override bool IsValid()
    {
        return TeamId > 0 &&
               !string.IsNullOrWhiteSpace(Code) &&
               !string.IsNullOrWhiteSpace(Name);
    }
}