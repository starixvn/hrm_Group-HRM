namespace HRM.ViewModels.Discipline
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRM.Models.Discipline;

    public class AddOtherDisciplineViewModel
    {
        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        [Required(ErrorMessage = "Discipline Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime WorkDate { get; set; }

        [Required(ErrorMessage = "Other discipline type is required.")]
        [Display(Name = "Discipline Type")]
        public short DisciplineTypeId { get; set; }

        [Display(Name = "Minus Score")]
        public decimal MinusScore { get; set; }

        public string Comment { get; set; }

        public IList<DisciplineTypeModel> OtherDisciplineTypes { get; set; }
    }
}