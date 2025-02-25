namespace HRM.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRM.Core.Enums;
    using HRM.Models.Supervisor;
    using HRM.Resources;

    public class CheckInOutRequestViewModel
    {
        private const byte MaxHour = 23;
        private const byte MaxMinute = 59;

        public long RequestId { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AttendantDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ForDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaxHour, ErrorMessage = "Hour is less than 23")]
        public int Hour { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaxMinute, ErrorMessage = "Minutes is less than 59")]
        public int Minute { get; set; }

        [Required]
        public byte RequestTypeId { get; set; }

        public string RequestTypeName => RequestTypeId == (byte)RequestType.CheckIn ? Labels.CheckIn : Labels.CheckOut;

        public byte ReasonId { get; set; }

        public string Shift { get; set; }

        public string RefNO { get; set; }

        [Required(ErrorMessage = "Remark is required.")]
        public string Remark { get; set; }

        [Display(Name = nameof(Labels.Verifier), ResourceType = typeof(Labels))]
        public int? VerifierId { get; set; }

        [Display(Name = nameof(Labels.Approver), ResourceType = typeof(Labels))]
        public int? ApproverId { get; set; }

        public IList<SupervisorNameModel> Verifiers { get; set; } = new List<SupervisorNameModel>();

        public IList<SupervisorNameModel> Approvers { get; set; } = new List<SupervisorNameModel>();

        public IEnumerable<DateTime> DisabledDates { get; set; } = new List<DateTime>();

        public List<string> Files { get; set; }
    }
}