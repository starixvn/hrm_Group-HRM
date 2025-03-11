namespace HRM.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    public class GetMonthlyOvertimeRequestDetailsByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime Month { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public short CompanyId { get; set; }

        public override string GetSettingKey() => "Overtime_GetMonthlyRequestDetailsByEmpId";

        public override bool IsValid() => EmployeeId > 0;
    }
}