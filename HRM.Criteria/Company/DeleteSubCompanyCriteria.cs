namespace HRM.Criteria.Company;

using Fanex.Data.Repository;

public class DeleteSubCompanyCriteria : CriteriaBase
{
    public int SubCompanyId { get; set; }


    public int DeletedBy { get; set; }

    public override string GetSettingKey()
    {
        return "SubCompany_Delete";
    }

    public override bool IsValid()
    {
        return SubCompanyId > 0 && DeletedBy > 0;
    }
}