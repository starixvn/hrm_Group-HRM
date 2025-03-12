namespace HRM.Criteria.Overtime;

using Fanex.Data.Repository;

public class GetMonthlyOvertimeRequestDetailsByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Overtime_GetMonthlyRequestDetailsById";

    public override bool IsValid() => RequestId > 0;
}