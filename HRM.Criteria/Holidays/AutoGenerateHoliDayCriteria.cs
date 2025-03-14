namespace HRM.Criteria.Holidays;

using Fanex.Data.Repository;

public class AutoGenerateHolidayCriteria : CriteriaBase
{
    public string Holidays { get; set; }

    public override string GetSettingKey() => "Holiday_AutoGenerate";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Holidays);
}