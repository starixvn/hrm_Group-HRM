using System.Threading.Tasks;
using HRM.Core.Models;
using Microsoft.AspNetCore.Http;

namespace HRM.Services.Interfaces;

public interface IUploadSalaryService
{
    Task<CommandResult> ImportEmpSalaryAsync(IFormFile file);
}