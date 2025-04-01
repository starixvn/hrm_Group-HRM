namespace HRM.ViewModels.AnnualLeave;

using System.ComponentModel.DataAnnotations;

public class AnnualLeaveBalanceMultiViewModel : BaseAnnualLeaveBalanceViewModel
{
    [Required]
    public string EmpIds { get; set; }
}