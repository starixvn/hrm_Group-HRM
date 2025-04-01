namespace HRM.Criteria.Request.ChangeShift;

using Fanex.Data.Repository;

public class GetChangeShiftRequestDetailByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public override string GetSettingKey() => "ChangeShiftRequest_GetDetailById";

    public override bool IsValid() => RequestId > 0;
}