namespace HRM.Services;

using System.Threading.Tasks;
using HRM.Core.Models;

public interface IHRMAuthService
{
    Task<EmployeeSession> GetByUserIdAsync(int userId);

    int Validate(EmployeeSession employee, string password);
}