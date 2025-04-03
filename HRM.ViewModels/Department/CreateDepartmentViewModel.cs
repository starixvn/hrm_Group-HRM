namespace HRM.ViewModels.Department;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CreateDepartmentViewModel
{
    [Required]
    public string Code { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int UnitId { get; set; }

    public bool IsActive { get; set; } = true;

    public IEnumerable<Models.Unit.Unit> Units { get; set; }
}