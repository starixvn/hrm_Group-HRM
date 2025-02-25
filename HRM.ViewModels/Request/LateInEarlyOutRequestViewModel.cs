namespace HRM.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRM.Models.Supervisor;
    using HRM.Resources;

    public class LateInEarlyOutRequestViewModel
    {
        private const byte MaximumMinute = 60;

        public long RequestId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime OnDate { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Remark is required.")]
        public string Remark { get; set; }

        [Required]
        [Range(1, MaximumMinute, ErrorMessage = "The Minute must be between 1 and 60")]
        public int Minute { get; set; }

        [Required]
        public byte RequestType { get; set; }

        [Required]
        public byte ReasonId { get; set; }

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