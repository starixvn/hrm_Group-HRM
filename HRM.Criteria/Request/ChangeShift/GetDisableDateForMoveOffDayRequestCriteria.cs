namespace HRM.Criteria.Request.ChangeShift;

using System;
using Fanex.Data;
using Fanex.Data.Repository;

public class GetDisableDateForMoveOffDayRequestCriteria : CriteriaBase
{
    [SpParam(SqlParamName = "EmpId")]
    public int EmployeeId { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public long? RequestId { get; set; }

    public override string GetSettingKey() => "ChangeShiftRequest_MoveOffDay_GetDisableDate";

    public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
}