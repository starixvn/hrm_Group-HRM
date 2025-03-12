namespace HRM.Criteria.EmployeeWorkFromHome;

using System;
using Fanex.Data.Repository;

public class GetEmployeeWorkFromHomeCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string TeamIds { get; set; }
    public byte? IsLocal { get; set; }

    public override string GetSettingKey() => "EmployeeWorkFromHome_Get";

    public override bool IsValid() => ToDate >= FromDate;
}