namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AnnualLeaveBalanceViewModel
    {
        public int EmployeeId { get; set; }

        public int Year { get; set; }

        [Display(Name = "Include employee's request")]
        public bool ExcludedRequestTrans { get; set; }

        public IList<AnnualLeaveTransactionViewModel> AnnualLeaveBalances { get; set; }

        public Employee Employee { get; set; }
    }
}
