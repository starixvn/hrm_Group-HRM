namespace HRM.Criteria.Holidays;

using System;
using Fanex.Data.Repository;

public class CreateHolidayCriteria : CriteriaBase
{
    public string HolidayName { get; set; }
    public byte TypeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int ModifiedBy { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Holiday_Insert";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(HolidayName) && ModifiedBy > 0;
}