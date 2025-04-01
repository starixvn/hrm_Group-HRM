namespace HRM.Criteria.HospitalizationLeave;

using System;
using Fanex.Data.Repository;

public class InsertMultipleTransactionCriteria : CriteriaBase
{
    public string EmployeeIds { get; set; }
    public string Description { get; set; }
    public DateTime FromDate { get; set; }
    public decimal DeductDays { get; set; }
    public decimal CreditDays { get; set; }
    public int ActorId { get; set; }

    public override string GetSettingKey() => "HospitalizationLeave_InsertMultipleTransactions";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(EmployeeIds) && ActorId > 0;
}