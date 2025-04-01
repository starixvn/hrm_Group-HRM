namespace HRM.Criteria.ExtensionNumber;

using Fanex.Data.Repository;

public class InsertExtensionNumberCriteria : CriteriaBase
{
    public string FullName { get; set; }

    public string NickName { get; set; }

    public string ExtensionNo { get; set; }

    public bool IsInternal { get; set; }

    public int EmpId { get; set; }

    public int TeamId { get; set; }

    public short OrderNo { get; set; }
        
    public short CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "ExtensionNO_Insert_EmployeeInfo";
    }

    public override bool IsValid()
    {
        return true;
    }
}