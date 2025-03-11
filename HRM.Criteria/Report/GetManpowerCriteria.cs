namespace HRM.Criteria.Report;

using Fanex.Data.Repository;

public class GetManpowerCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int Year { get; set; }

    public override string GetSettingKey() => "Report_Manpower";

    public override bool IsValid() => CompanyId > 0 && Year > 0;
}