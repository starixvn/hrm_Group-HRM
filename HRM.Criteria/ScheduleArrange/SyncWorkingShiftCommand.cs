namespace HRM.Criteria.ScheduleArrange;

using System;
using Fanex.Data.Repository;

public class SyncWorkingShiftCommand : CriteriaBase
{
    public short CompanyId { get; set; }
    public int ActorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string EmployeeIds { get; set; }
    public string WorkingShiftXml { get; set; }

    public override string GetSettingKey() => "ScheduleArrangement_SyncWorkingShift";

    public override bool IsValid() => CompanyId > 0 && ActorId > 0 && !string.IsNullOrWhiteSpace(EmployeeIds) && !string.IsNullOrWhiteSpace(WorkingShiftXml);
}