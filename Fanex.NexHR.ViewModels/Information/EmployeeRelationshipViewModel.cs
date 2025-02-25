namespace Fanex.NexHR.ViewModels.Information
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    public class EmployeeRelationshipViewModel
    {
        private const byte MaxLengthFullName = 200;
        private const byte MaxLengthPhone = 50;

        [Required]
        [StringLength(MaxLengthFullName, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Birthday")]
        public DateTime BirthDay { get; set; }

        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "Phone number is invalid")]
        [Display(Name = "Phone")]
        [StringLength(MaxLengthPhone, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        public string Phone { get; set; }

        [Required]
        [StringLength(MaxLengthPhone, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = "Relationship")]
        public string Relationship { get; set; }

        public int Gender { get; set; }

        [Display(Name = "Emergency")]
        public bool IsEmergency { get; set; }
    }
}