using System.ComponentModel.DataAnnotations;

namespace HRM.ViewModels.Section;

public class DeleteSectionViewModel
{
    [Required]
    public int Id { get; set; }
}