namespace HRM.Criteria.HospitalizationLeave;

using System;
using Fanex.Data.Repository;

public class DeleteHospitalizationLeaveTransactionCriteria : CriteriaBase
{
    public long TransId { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "HospitalizationLeave_DeleteTransaction";

    public override bool IsValid() => TransId > 0 && ModifiedBy > 0;
}