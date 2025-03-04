using Fanex.Data.Repository;

namespace HRM.Criteria.ManagementLevel;

public class DeleteManagementLevelCriteria : CriteriaBase
{
    public int ManagementLevelId { get; set; }
    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "ManagementLevel_Delete";
    }

    public override bool IsValid()
    {
        return ManagementLevelId > 0;
    }
}