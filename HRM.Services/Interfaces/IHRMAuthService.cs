using System.Threading.Tasks;
using HRM.Core.Models;

namespace HRM.Services.Interfaces;

public interface IHRMAuthService
{
    Task<EmployeeSession> GetByUserIdAsync(int userId);

    int Validate(EmployeeSession employee, string password);
}