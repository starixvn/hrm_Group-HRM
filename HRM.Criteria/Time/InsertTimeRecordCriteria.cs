namespace HRM.Criteria.Time
{
    using Fanex.Data.Repository;
    using System;

    public class InsertTimeRecordCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime RecordTime { get; set; }

        public bool InOutType { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_Insert";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
