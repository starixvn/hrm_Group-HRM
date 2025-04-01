namespace HRM.Criteria.HospitalizationLeave;

using Fanex.Data.Repository;

public class GetTransactionByIdCriteria : CriteriaBase
{
    public long TransId { get; set; }

    public override string GetSettingKey() => "HospitalizationLeave_GetTransById";

    public override bool IsValid() => TransId > 0;
}