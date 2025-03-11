namespace HRM.Criteria.Shift
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetEmployeeShiftByDateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public short CompanyId { get; set; }

        public override string GetSettingKey() => "EmployeeCalendar_GetShiftByDate";

        public override bool IsValid() => EmployeeId > 0;
    }
}