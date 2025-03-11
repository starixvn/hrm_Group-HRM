namespace HRM.Criteria.Report.MealAllowance;

using System;
using Fanex.Data.Repository;

public class GetMealAllowanceSummaryReportCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string TeamIds { get; set; }
    public byte? IsLocal { get; set; }

    public override bool IsValid() => CompanyId > 0;

    public override string GetSettingKey() => "WorkFromHome_MealAllowanceSummaryReport";
}