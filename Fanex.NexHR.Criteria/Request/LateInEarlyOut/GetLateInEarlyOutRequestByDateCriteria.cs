namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutRequestByDateCriteria : CriteriaBase
    {
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "LateInEarlyOutRequest_GetByDate";

        public override bool IsValid() => FromDate <= ToDate;
    }
}