namespace HRM.Criteria.BackgroundJob;

using System;
using Fanex.Data.Repository;

public class InsertScheduleByDefaultShiftCriteria : CriteriaBase
{
    public int? EmployeeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "Auto_EmployeeCalendar_InsertDefaultShift";

    public override bool IsValid() => true;
}