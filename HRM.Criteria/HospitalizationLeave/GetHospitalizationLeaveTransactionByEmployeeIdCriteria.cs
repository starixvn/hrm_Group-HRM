namespace HRM.Criteria.HospitalizationLeave;

using Fanex.Data;
using Fanex.Data.Repository;

public class GetHospitalizationLeaveTransactionByEmployeeIdCriteria : CriteriaBase
{
    [SpParam(SqlParamName = "EmpId")]
    public int EmployeeId { get; set; }
    public int Year { get; set; }
    public bool IncludedRequestTrans { get; set; }

    public override string GetSettingKey() => "HospitalizationLeave_GetTransByEmpId";

    public override bool IsValid() => EmployeeId > 0;
}