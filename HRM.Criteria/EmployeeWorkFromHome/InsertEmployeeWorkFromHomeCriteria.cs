namespace HRM.Criteria.EmployeeWorkFromHome
{
    using System;
    using Fanex.Data.Repository;

    public class InsertEmployeeWorkFromHomeCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CreatedBy { get; set; }
        public string Remark { get; set; }

        public override string GetSettingKey() => "EmployeeWorkFromHome_Insert";

        public override bool IsValid() => EmpId > 0 && FromDate <= ToDate && CreatedBy > 0;
    }
}