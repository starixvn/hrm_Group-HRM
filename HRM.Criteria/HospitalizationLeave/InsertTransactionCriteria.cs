namespace HRM.Criteria.HospitalizationLeave
{
    using System;
    using Fanex.Data.Repository;

    public class InsertTransactionCriteria : CriteriaBase
    {
        public int? RequestId { get; set; }
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal DeductDays { get; set; }
        public decimal CreditDays { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "HospitalizationLeave_InsertTransaction";

        public override bool IsValid() => EmpId > 0 && CreatedBy > 0;
    }
}