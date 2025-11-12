using System.Threading.Tasks;
using HRM.Models.GroupDashboard;

namespace HRM.Services.Interfaces;

public interface IGroupDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync(int year);
}

