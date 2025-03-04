using Fanex.Data.Repository;

namespace HRM.Criteria.ManagementLevel;

public class InsertManagementLevelCriteria : CriteriaBase
{
    public string Name { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "ManagementLevel_Insert";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name);
    }
}