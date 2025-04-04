namespace HRM.Criteria.Report;

using System;
using Fanex.Data.Repository;

public class GetDisciplineSummaryPerYearCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string TeamIds { get; set; }
    public int? EmployeeId { get; set; }
    public bool? IsLocal { get; set; }
    public int ApproverId { get; set; }
    public bool IsShowAll { get; set; }
    public string RoleIds { get; set; }

    public override bool IsValid() => CompanyId > 0 && FromDate <= ToDate && ApproverId > 0;

    public override string GetSettingKey() => "Report_DisciplineSummaryPerYear";
}