namespace HRM.Criteria.Request.CheckInOut;

using System;
using Fanex.Data.Repository;

public class GetCheckInOutRequestByDateCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "CheckInOutRequest_GetByDate";
    }

    public override bool IsValid()
    {
        return FromDate <= ToDate;
    }
}