namespace HRM.Criteria.Request.ChangeShift
{
    using System;
    using Fanex.Data.Repository;

    public class GetNumberOfRequestCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "ChangeShiftRequest_GetNumberOfRequest";

        public override bool IsValid() => EmployeeId > 0;
    }
}