namespace HRM.Criteria.Lookup;

using Fanex.Data.Repository;

public class InsertLookupDataCriteria : CriteriaBase
{
    public string LookupCode { get; set; }

    public string Name { get; set; }

    public string CategoryId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "LookupData_Insert";
    }

    public override bool IsValid()
    {
        return true;
    }
}