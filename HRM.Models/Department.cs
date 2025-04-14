namespace HRM.Models;

public class Department : EntityBase<int>
{
    public Unit.Unit Unit { get; set; }

    public int GroupDeptId { get; set; }

    public int PICEmpId { get; set; }

    public string AccountableArea { get; set; }

    public bool IsActive { get; set; }
}