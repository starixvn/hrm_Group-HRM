using Fanex.Data.Repository;

namespace HRM.Criteria.ManagementLevel;

public class UpdateManagementLevelCriteria : CriteriaBase
{
    public int ManagementLevelId { get; set; }

    public string Name { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "ManagementLevel_Update";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && ManagementLevelId > 0;
    }
}