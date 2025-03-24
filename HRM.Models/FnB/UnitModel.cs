using System;

namespace HRM.Models.FnB;

public class UnitModel
{
    public int UnitId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public DateTime ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }
}