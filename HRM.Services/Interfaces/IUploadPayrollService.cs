using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models.Payroll;

namespace HRM.Services.Interfaces;

public interface IUploadPayrollService
{
    Task<CommandResult> ImportEmpPayrollAsync(UploadPayrollModel model);
}