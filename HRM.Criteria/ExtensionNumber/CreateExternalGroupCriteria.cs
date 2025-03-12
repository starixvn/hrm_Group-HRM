namespace HRM.Criteria.ExtensionNumber;

using Fanex.Data.Repository;

public class CreateExternalGroupCriteria : CriteriaBase
{
    public string FullName { get; set; }

    public override string GetSettingKey() => "ExtensionNO_Insert_GroupExternal";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(FullName);
}