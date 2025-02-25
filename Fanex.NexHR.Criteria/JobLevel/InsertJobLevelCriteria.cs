namespace Fanex.NexHR.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class InsertJobLevelCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "JobLevel_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
    }
}
