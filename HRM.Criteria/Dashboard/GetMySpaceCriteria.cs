namespace HRM.Criteria.Dashboard
{
    using System;
    using Fanex.Data.Repository;

    public class GetMySpaceCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }

        public override string GetSettingKey() => "Dashboard_Get";

        public override bool IsValid() => EmployeeId > 0;
    }
}