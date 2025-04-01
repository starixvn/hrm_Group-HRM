namespace HRM.Criteria.Request.ChangeShift;

using Fanex.Data.Repository;

public class GetChangeShiftRequestHistoryByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public override string GetSettingKey() => "ChangeShiftRequest_GetHistoryById";

    public override bool IsValid() => RequestId > 0;
}