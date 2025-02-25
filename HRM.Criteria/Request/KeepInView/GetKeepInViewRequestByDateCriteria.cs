namespace HRM.Criteria.Request.KeepInView
{
    using System;
    using Fanex.Data.Repository;

    public class GetKeepInViewRequestByDateCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "KeepInViewRequest_GetByDate";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
