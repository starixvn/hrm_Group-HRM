namespace HRM.Criteria.Message
{
    using System;
    using Fanex.Data.Repository;

    public class GetMessageByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? IsDisabled { get; set; }

        public override string GetSettingKey() => "Message_GetByEmployee";

        public override bool IsValid() => EmployeeId > 0;
    }
}