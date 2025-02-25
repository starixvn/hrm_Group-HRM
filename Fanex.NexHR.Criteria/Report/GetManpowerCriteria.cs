namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetManpowerCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Report_Manpower";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}