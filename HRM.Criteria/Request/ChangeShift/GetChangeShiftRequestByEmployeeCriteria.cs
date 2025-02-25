namespace HRM.Criteria.Request.ChangeShift
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetChangeShiftRequestByEmployeeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "ChangeShiftRequest_GetByEmployee";

        public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
    }
}