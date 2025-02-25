namespace HRM.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_WorkingShift_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
}