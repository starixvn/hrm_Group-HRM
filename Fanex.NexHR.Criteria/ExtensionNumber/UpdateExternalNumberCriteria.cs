namespace Fanex.NexHR.Criteria.ExtensionNumber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateExternalNumberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ExtID")]
        public int ExternalId { get; set; }

        public string FullName { get; set; }

        public override string GetSettingKey() => "ExtensionNO_Update_GroupExternal";

        public override bool IsValid() => ExternalId > 0 && !string.IsNullOrWhiteSpace(FullName);
    }
}