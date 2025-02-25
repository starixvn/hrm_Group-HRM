namespace HRM.Criteria.Holidays
{
    using System;
    using Fanex.Data.Repository;

    public class EditHolidayCriteria : CriteriaBase
    {
        public int Id { get; set; }
        public string HolidayName { get; set; }
        public byte TypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Holiday_Update";

        public override bool IsValid() => Id > 0 && !string.IsNullOrWhiteSpace(HolidayName) && ModifiedBy > 0;
    }
}