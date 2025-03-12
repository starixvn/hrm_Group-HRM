namespace HRM.ViewModels.AnnualLeave;

using Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class AnnualLeaveBalanceViewModel
{
    public int EmployeeId { get; set; }

    public int Year { get; set; }

    [Display(Name = "Include employee's request")]
    public bool ExcludedRequestTrans { get; set; }

    public IEnumerable<AnnualLeaveTransactionViewModel> AnnualLeaveBalances { get; set; }

    public Employee Employee { get; set; }
}