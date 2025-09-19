using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Report.MonthlyWorkingHours;

public class GetEmployeeRequestCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int ActorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string TeamIds { get; set; }
    public string Employee { get; set; }
    public byte? IsLocal { get; set; }
    public string EmployeeStatusIds { get; set; }

    public override string GetSettingKey() => "Report_MonthlyWorkingHours_EmployeeRequest";

    public override bool IsValid() => CompanyId > 0 && ActorId > 0;
}