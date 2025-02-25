namespace Fanex.NexHR.Criteria.Request.Leave
{
    using System;
    using Fanex.Data.Repository;

    public class GetLeaveRequestByDateCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveRequest_GetByDate";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
