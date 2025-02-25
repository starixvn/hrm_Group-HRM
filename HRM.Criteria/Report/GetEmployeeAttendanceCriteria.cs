namespace HRM.Criteria.Report
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetEmployeeAttendanceCriteria : CriteriaBase
    {
        public int? SupervisorId { get; set; }

        public DateTime Fromtime { get; set; }

        public DateTime Totime { get; set; }

        public int? EmpId { get; set; }

        public string TeamIds { get; set; }

        public int? ExcludeUserDelete { get; set; }

        public byte? IsLocal { get; set; }

        public bool? IsRealDate { get; set; }

        [SpParam(SqlParamName = "Type")]
        public short AttendanceType { get; set; }

        public override string GetSettingKey() => "EmployeeAttendance_GetByType";

        public override bool IsValid() => true;
    }
}
