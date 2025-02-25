namespace HRM.Criteria.Holidays
{
    using Fanex.Data.Repository;

    public class GetHolidaysCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey() => "Holiday_GetByYear";

        public override bool IsValid() => Year > 0;
    }
}