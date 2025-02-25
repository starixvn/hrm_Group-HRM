namespace HRM.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetManpowerCategoryCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey() => "Report_ManpowerCategory";

        public override bool IsValid() => Year > 0;
    }
}