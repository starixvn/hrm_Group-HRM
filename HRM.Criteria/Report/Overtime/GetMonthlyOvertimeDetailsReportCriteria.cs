using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Report.Overtime;

public class GetMonthlyOvertimeDetailsReportCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int EmpId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "Report_MonthlyOvertimeDetails";

    public override bool IsValid() => CompanyId > 0 && EmpId > 0;
}