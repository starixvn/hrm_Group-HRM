namespace HRM.ViewModels.Request
{
    using System.Collections.Generic;
    using HRM.Models;
    using HRM.Models.Request;

    public class LeaveDetailViewModel
    {
        public IList<LeaveRequestDetail> LeaveRequestDetail { get; set; }

        public IList<LeaveType> LeaveTypeList { get; set; }
    }
}
