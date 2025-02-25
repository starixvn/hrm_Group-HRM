namespace Fanex.NexHR.ViewModels.HospitalizationLeave
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models.HospitalizationLeave;

    public class HospitalizationLeaveTransactionViewModel
    {
        public int EmployeeId { get; set; }
        public string Nickname { get; set; }
        public int Year { get; set; }

        [Display(Name = "Include employee's request")]
        public bool IncludedRequestTrans { get; set; }

        public IEnumerable<HospitalizationLeaveTransaction> Transactions { get; set; }
    }
}