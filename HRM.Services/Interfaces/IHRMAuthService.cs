using System.Threading.Tasks;
using HRM.Core.Models;
using Microsoft.AspNetCore.Http;

namespace HRM.Services.Interfaces;

public interface IHRMAuthService
{
    Task<EmployeeSession> GetByUserIdAsync(int userId);

    int Validate(EmployeeSession employee, string password);

    string GetClientIp(HttpContext httpContext);
}