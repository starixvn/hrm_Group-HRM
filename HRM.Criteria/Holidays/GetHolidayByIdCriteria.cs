namespace HRM.Criteria.Holidays
{
    using Fanex.Data.Repository;

    public class GetHolidayByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "Holiday_GetById";

        public override bool IsValid() => Id > 0;
    }
}