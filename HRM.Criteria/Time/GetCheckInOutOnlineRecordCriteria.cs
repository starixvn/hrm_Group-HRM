namespace HRM.Criteria.Time
{
    using System;
    using Fanex.Data.Repository;

    public class GetCheckInOutOnlineRecordCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public override string GetSettingKey() => "TimeRecord_GetCheckInOutOnline";

        public override bool IsValid() => true;
    }
}