namespace HRM.Criteria.Unit;

using Fanex.Data.Repository;

public class UpdateUnitCriteria : CriteriaBase
{
    public int UnitId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public int GroupUnitId { get; set; }

    public int? PICEmpId { get; set; }

    public string SupporterEmpId { get; set; }

    public string Category { get; set; }

    public string Type { get; set; }

    public string AccountableArea { get; set; }

    public bool IsActive { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "Unit_Update";
    }

    public override bool IsValid()
    {
        return UnitId > 0 && !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
    }
}