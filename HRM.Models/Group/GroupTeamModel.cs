using System;

namespace HRM.Models.Group;

public class GroupTeamModel
{
    public int Id { get; set; }

    public int GroupTeamId { get; set; }

    public string UnitCode { get; set; }

    public int DepartmentId { get; set; }

    public int PICEmpId { get; set; }

    public string DepartmentName { get; set; }

    public string DepartmentCode { get; set; }

    public string DepartmentPIC { get; set; }

    public string TeamName { get; set; }

    public string TeamCode { get; set; }

    public string TeamPIC { get; set; }

    public string AccountableArea { get; set; }

    public string CompanyName { get; set; }

    public int NoOfMembers { get; set; }

    public int CompanyId { get; set; }

    public DateTime? ModifiedDate { get; set; }
}