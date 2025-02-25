namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;

    public class GetAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int Year { get; set; }

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
}
