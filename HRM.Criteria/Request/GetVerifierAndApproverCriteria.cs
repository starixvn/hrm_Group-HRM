namespace HRM.Criteria.Request
{
    using Fanex.Data.Repository;

    public class GetVerifierAndApproverCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public byte RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetVerifierAndApprover";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && RequestType > 0;
        }
    }
}
