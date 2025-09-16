namespace HRM.Models.Unit;

public class Unit : EntityBase<int>
{
    public int GroupUnitId { get; set; }

    public int PICEmpId { get; set; }

    public string PICNickName { get; set; }

    public string SupporterEmpId { get; set; }

    public string SupporterNickName { get; set; }

    public string Category { get; set; }

    public string Type { get; set; }

    public string AccountableArea { get; set; }

    public bool IsActive { get; set; }
}