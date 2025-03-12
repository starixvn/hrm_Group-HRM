namespace HRM.Criteria.ExtensionNumber;

using Fanex.Data;
using Fanex.Data.Repository;

public class GetExtensionNumberMemberByIdCriteria : CriteriaBase
{
    public bool IsInternal { get; set; }

    [SpParam(SqlParamName = "EmpExtID")]
    public int EmployeeExtensionNoId { get; set; }

    public override string GetSettingKey() => "ExtensionNO_GetEmployeeInfo";

    public override bool IsValid() => true;
}