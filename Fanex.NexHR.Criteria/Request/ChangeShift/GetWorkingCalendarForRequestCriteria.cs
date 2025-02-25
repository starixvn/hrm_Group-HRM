namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetWorkingCalendarForRequestCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte TypeId { get; set; }
        public long? RequestId { get; set; }

        public override string GetSettingKey() => "ChangeShiftRequest_GetCalendarForRequest";

        public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
    }
}