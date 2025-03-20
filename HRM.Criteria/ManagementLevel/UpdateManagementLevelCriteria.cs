using Fanex.Data.Repository;

namespace HRM.Criteria.ManagementLevel;

public class UpdateManagementLevelCriteria : CriteriaBase
{
    public short ManagementLevelId { get; set; }

    public string Name { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "ManagementLevel_Update";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && ManagementLevelId > 0;
}