using Fanex.Data.Repository;

namespace HRM.Criteria.Employee;

public class GetTransferHistoryWorkCriteria : CriteriaBase
{
    public int EmpId { get; set; }
    public int? CompanyId { get; set; }

    public override string GetSettingKey() => "Transfer_HistoryWork_Get";

    public override bool IsValid() => true; 
}

