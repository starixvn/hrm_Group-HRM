namespace HRM.ViewModels.Unit
{
    using System.ComponentModel.DataAnnotations;

    public class EditUnitViewModel : AddUnitViewModel
    {
        [Required]
        public int UnitId { get; set; }
    }
}
