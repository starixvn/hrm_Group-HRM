namespace HRM.Criteria.Report;

using Fanex.Data.Repository;

public class GetManpowerCategoryCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int Year { get; set; }

    public override string GetSettingKey() => "Report_ManpowerCategory";

    public override bool IsValid() => CompanyId > 0 && Year > 0;
}