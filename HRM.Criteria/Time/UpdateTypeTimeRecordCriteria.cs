namespace HRM.Criteria.Time
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateTypeTimeRecordCriteria : CriteriaBase
    {
        public int RecordTimeId { get; set; }

        public byte InOutType { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_UpdateType";
        }

        public override bool IsValid()
        {
            return RecordTimeId > 0;
        }
    }
}
