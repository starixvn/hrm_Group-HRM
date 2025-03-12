namespace HRM.ViewModels.Request;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Supervisor;
using Resources;

public class KeepInViewRequestViewModel
{
    public long RequestId { get; set; }

    [Required]
    public string Employee { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public string RefNo { get; set; }

    public byte KeepInViewType { get; set; }

    public bool HalfDay { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    public string FromTime { get; set; }

    public string ToTime { get; set; }

    public bool CrossDay { get; set; }

    [Required(ErrorMessage = "Remark is required.")]
    public string Remark { get; set; }

    [Display(Name = nameof(Labels.Verifier), ResourceType = typeof(Labels))]
    public int? VerifierId { get; set; }

    [Display(Name = nameof(Labels.Approver), ResourceType = typeof(Labels))]
    public int? ApproverId { get; set; }

    public int? ShiftId { get; set; }

    public DateTime MinDate { get; set; }
    public DateTime MaxDate { get; set; }
    public IList<SupervisorNameModel> Verifiers { get; set; } = new List<SupervisorNameModel>();
    public IList<SupervisorNameModel> Approvers { get; set; } = new List<SupervisorNameModel>();

    public IEnumerable<DateTime> DisabledDates { get; set; } = new List<DateTime>();

    public List<string> Files { get; set; }
}