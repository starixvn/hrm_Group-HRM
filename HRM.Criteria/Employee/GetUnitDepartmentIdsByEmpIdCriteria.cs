using Fanex.Data.Repository;

namespace HRM.Criteria.Employee
{
    public class GetUnitDepartmentIdsByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey() => "UnitDepartmentIds_EmpBelongTo_Get";

        public override bool IsValid() => true;
    }
}
