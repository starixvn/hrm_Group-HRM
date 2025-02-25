namespace HRM.Criteria.Request.KeepInView
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDisabledDateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long? RequestId { get; set; }
        public byte TypeId { get; set; }

        public override string GetSettingKey() => "KeepInViewRequest_GetDisableDate";

        public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
    }
}