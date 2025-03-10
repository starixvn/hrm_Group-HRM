namespace HRM.Criteria.Report;

using Fanex.Data.Repository;

public class GetManpowerCriteria : CriteriaBase
{
    public int Year { get; set; }

    public override string GetSettingKey() => "Report_Manpower";

    public override bool IsValid() => Year > 0;
}