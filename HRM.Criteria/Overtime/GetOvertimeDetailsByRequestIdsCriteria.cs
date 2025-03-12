namespace HRM.Criteria.Overtime;

using Fanex.Data.Repository;

public class GetOvertimeDetailsByRequestIdsCriteria : CriteriaBase
{
    public string RequestIds { get; set; }

    public override string GetSettingKey() => "Overtime_GetDetailsByRequestIds";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(RequestIds);
}