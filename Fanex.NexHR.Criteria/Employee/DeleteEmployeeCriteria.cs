namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteEmployeeCriteria : CriteriaBase
    {
        public int EmpId { get; set; }
        public int ActorId { get; set; }
        public DateTime ResignedDate { get; set; }

        public override string GetSettingKey() => "Employee_Delete";

        public override bool IsValid() => EmpId > 0 && ActorId > 0;
    }
}
