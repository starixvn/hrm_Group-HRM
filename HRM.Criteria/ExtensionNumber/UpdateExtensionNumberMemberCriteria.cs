namespace HRM.Criteria.ExtensionNumber;

using Fanex.Data.Repository;

public class UpdateExtensionNumberMemberCriteria : CriteriaBase
{
    public int ExtId { get; set; }

    public string FullName { get; set; }

    public string NickName { get; set; }

    public string ExtensionNo { get; set; }

    public bool IsInternal { get; set; }

    public int EmpId { get; set; }

    public int TeamId { get; set; }

    public short OrderNo { get; set; }

        public int CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "ExtensionNO_Update_EmployeeInfo";
    }

    public override bool IsValid()
    {
        return ExtId > 0;
    }
}