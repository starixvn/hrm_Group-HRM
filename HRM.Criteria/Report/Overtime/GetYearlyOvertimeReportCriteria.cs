using Fanex.Data.Repository;

namespace HRM.Criteria.Report.Overtime;

public class GetYearlyOvertimeReportCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int ActorId { get; set; }
    public string TeamIds { get; set; }
    public string Employee { get; set; }
    public string EmployeeStatusIds { get; set; }
    public string MonthXml { get; set; }
    public byte? IsLocal { get; set; }

    public override string GetSettingKey() => "Report_YearlyOvertime";

    public override bool IsValid() => CompanyId > 0 && ActorId > 0;
}