namespace HRM.ViewModels.Section
{
    using System.ComponentModel.DataAnnotations;

    public class EditSectionViewModel : AddSectionViewModel
    {
        [Required]
        public int SectionId { get; set; }
    }
}
