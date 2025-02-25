namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System;

    public class AnnualLeaveTransactionViewModel : BaseAnnualLeaveBalanceViewModel
    {
        public long LeaveTransId { get; set; }

        public int EmpId { get; set; }

        public long? RequestId { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
