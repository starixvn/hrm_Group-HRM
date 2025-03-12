namespace HRM.Models;

using System.ComponentModel.DataAnnotations;

public class ExtensionNoModel
{
    public int ExtensionNoId { get; set; }

    [Required]
    public int TeamId { get; set; }

    public string Team { get; set; }

    public string TeamName { get; set; }

    public string FullName { get; set; }

    public string NickName { get; set; }

    public int EmployeeExtensionId { get; set; }

    [Required]
    public string ExtensionNo { get; set; }

    public int OrderNoGroup { get; set; }

    public int OrderNoMember { get; set; }

    public bool IsInternal { get; set; }

    [Required(ErrorMessage = "The Employee field is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "The Employee field is required.")]
    public int EmpId { get; set; }

    public string Employee { get; set; }

    public int TotalMember { get; set; }
}