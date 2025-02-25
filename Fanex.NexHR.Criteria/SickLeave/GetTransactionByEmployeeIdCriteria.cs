namespace Fanex.NexHR.Criteria.SickLeave
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetTransactionByEmployeeIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public bool IncludedRequestTrans { get; set; }

        public override string GetSettingKey() => "SickLeave_GetTransByEmpId";

        public override bool IsValid() => EmployeeId > 0;
    }
}
