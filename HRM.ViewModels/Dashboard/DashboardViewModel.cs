namespace HRM.ViewModels.Dashboard;

using HRM.Models.Dashboard;
using System.Collections.Generic;

public class DashboardViewModel
{
    public IEnumerable<SummaryRequest> SummaryRequests { get; set; }
    public IEnumerable<MemberRequest> MemberRequests { get; set; }
}