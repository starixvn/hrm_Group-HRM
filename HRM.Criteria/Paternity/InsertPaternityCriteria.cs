namespace HRM.Criteria.Paternity
{
    using System;
    using Fanex.Data.Repository;

    public class InsertPaternityCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime ChildBornDate { get; set; }

        public int CreatedBy { get; set; }

        public int LeaveDay { get; set; }

        public string Remark { get; set; }

        public override string GetSettingKey() => "Paternity_Insert";

        public override bool IsValid() => EmpId > 0 && CreatedBy > 0;
    }
}