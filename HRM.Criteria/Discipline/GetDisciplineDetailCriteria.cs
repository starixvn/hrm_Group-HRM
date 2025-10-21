namespace HRM.Criteria.Discipline;

using System;
using Fanex.Data;
using Fanex.Data.Repository;

public class GetDisciplineDetailCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string TeamIds { get; set; }

    public int? EmployeeId { get; set; }

    public bool? IsLocal { get; set; }

    public int? ReasonId { get; set; }

    public int ApproverUserId { get; set; }

    public bool? HasIgnored { get; set; }

    [SpParam(Ignored = true)]
    public string EmployeeName { get; set; }

    [SpParam(Ignored = true)]
    public string TeamName { get; set; }

    public short CompanyId { get; set; }

    public override bool IsValid() => FromDate <= ToDate && ApproverUserId > 0;

    public override string GetSettingKey() => "Report_DisciplineDetail";
}