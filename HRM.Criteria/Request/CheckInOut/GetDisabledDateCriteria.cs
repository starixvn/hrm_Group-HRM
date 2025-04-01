namespace HRM.Criteria.Request.CheckInOut;

using System;
using Fanex.Data;
using Fanex.Data.Repository;

public class GetDisabledDateCriteria : CriteriaBase
{
    [SpParam(SqlParamName = "EmpId")]
    public int EmployeeId { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public long? RequestId { get; set; }
    public byte RequestTypeId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "CheckInOutRequest_GetDisableDate";

    public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
}