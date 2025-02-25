namespace HRM.AutoJobs.Criteria.WorkingShift
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_WorkingShift_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
}