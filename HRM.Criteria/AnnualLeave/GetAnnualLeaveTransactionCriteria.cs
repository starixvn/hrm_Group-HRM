namespace HRM.Criteria.AnnualLeave;

using System;
using Fanex.Data.Repository;

public class GetAnnualLeaveTransactionCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public DateTime FromDate { get; set; }

    public bool ExcludedRequestTrans { get; set; }

    public override string GetSettingKey()
    {
        return "AnnualLeave_GetTransByEmp";
    }

    public override bool IsValid()
    {
        return EmpId > 0;
    }
}