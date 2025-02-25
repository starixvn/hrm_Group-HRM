namespace Fanex.NexHR.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class DeleteJobLevelCriteria : CriteriaBase
    {
        public int JobLevelId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "JobLevel_Delete";
        }

        public override bool IsValid()
        {
            return JobLevelId > 0;
        }
    }
}
