namespace Fanex.NexHR.Criteria.BackgroundJob
{
    using Fanex.Data.Repository;

    public class ImportTimeRecordCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "TimeRecord_ImportData";

        public override bool IsValid() => true;
    }
}