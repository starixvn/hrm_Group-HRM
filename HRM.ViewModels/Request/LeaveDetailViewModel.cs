using HRM.Models.Request.Leave;

namespace HRM.ViewModels.Request;

using System.Collections.Generic;
using Models;

public class LeaveDetailViewModel
{
    public IList<LeaveRequestDetail> LeaveRequestDetail { get; set; }

    public IList<LeaveType> LeaveTypeList { get; set; }
}