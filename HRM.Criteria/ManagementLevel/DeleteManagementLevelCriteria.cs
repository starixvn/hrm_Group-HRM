using Fanex.Data.Repository;

namespace HRM.Criteria.ManagementLevel;

public class DeleteManagementLevelCriteria : CriteriaBase
{
    public short ManagementLevelId { get; set; }
    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "ManagementLevel_Delete";

    public override bool IsValid() => ManagementLevelId > 0;
}