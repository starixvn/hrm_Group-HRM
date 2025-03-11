namespace HRM.Criteria.Report;

using System;
using Fanex.Data.Repository;

public class GetDailyOvertimeReportCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int ActorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string Employee { get; set; }
    public string TeamIds { get; set; }
    public byte? IsLocal { get; set; }
    public byte? EmployeeStatusId { get; set; }

    public override string GetSettingKey() => "Report_DailyOvertime";

    public override bool IsValid() => CompanyId > 0 && ActorId > 0;
}