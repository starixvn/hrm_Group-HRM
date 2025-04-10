namespace HRM.Criteria.Unit;

using Fanex.Data.Repository;
using HRM.Resources;
using System.ComponentModel.DataAnnotations;

public class InsertUnitCriteria : CriteriaBase
{
    public string Code { get; set; }

    public string Name { get; set; }

    public int GroupUnitId { get; set; }

    public int? PICEmpId { get; set; }

    public string Category { get; set; }

    public string Type { get; set; }

    public string AccountableArea { get; set; }

    public bool IsActive { get; set; }
        
    public int CreatedBy { get; set; }

    public override string GetSettingKey()
    {
        return "Unit_Insert";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
    }
}