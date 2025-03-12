namespace HRM.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Enums;
    using HRM.Models.Request;
    using HRM.Models.Request.ChangeShift;
    using Models.Supervisor;
    using Resources;

    public class ChangeShiftViewModel
    {
        public long RequestId { get; set; }

        [Display(Name = nameof(Labels.Type), ResourceType = typeof(Labels))]
        public byte TypeId { get; set; } = (byte)ChangeShiftType.ChangeShift;

        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public string Remark { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public IEnumerable<ChangeShiftDetail> Details { get; set; } = new List<ChangeShiftDetail>();

        [Display(Name = nameof(Labels.Verifier), ResourceType = typeof(Labels))]
        public int? VerifierId { get; set; }

        [Display(Name = nameof(Labels.Approver), ResourceType = typeof(Labels))]
        public int? ApproverId { get; set; }

        public RequestStatus Status { get; set; }

        public decimal TotalRequestInMonth { get; set; }

        public IList<SupervisorNameModel> Verifiers { get; set; } = new List<SupervisorNameModel>();

        public IList<SupervisorNameModel> Approvers { get; set; } = new List<SupervisorNameModel>();

        public List<string> Files { get; set; }

    }
}