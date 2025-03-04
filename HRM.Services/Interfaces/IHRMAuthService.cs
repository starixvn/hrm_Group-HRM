namespace HRM.Services;

using System.Threading.Tasks;
using Fanex.NexHR.Core.Enums;
using HRM.Core.Models;

public interface IHRMAuthService
{
    Task<EmployeeSession> GetByUserIdAsync(int userId);

    int Validate(EmployeeSession employee, string password);

    HashProvider HashProvider { get; }
}