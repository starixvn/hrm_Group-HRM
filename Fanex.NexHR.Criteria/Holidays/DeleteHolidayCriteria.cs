namespace Fanex.NexHR.Criteria.Holidays
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteHolidayCriteria : CriteriaBase
    {
        public int Id { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Holiday_Delete";

        public override bool IsValid() => Id > 0 && ModifiedBy > 0;
    }
}