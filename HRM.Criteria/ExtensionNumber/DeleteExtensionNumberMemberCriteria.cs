namespace HRM.Criteria.ExtensionNumber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DeleteExtensionNumberMemberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ExtID")]
        public int ExtensionNumberId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_DELETE_EmployeeInfo";
        }

        public override bool IsValid()
        {
            return ExtensionNumberId > 0;
        }
    }
}
