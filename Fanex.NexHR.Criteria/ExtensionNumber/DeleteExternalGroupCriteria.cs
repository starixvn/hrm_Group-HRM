namespace Fanex.NexHR.Criteria.ExtensionNumber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DeleteExternalGroupCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ExtID")]
        public int ExternalId { get; set; }

        public override string GetSettingKey() => "ExtensionNO_DELETE_GroupExternal";

        public override bool IsValid() => ExternalId > 0;
    }
}