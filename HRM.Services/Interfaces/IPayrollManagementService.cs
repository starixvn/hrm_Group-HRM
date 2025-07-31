using System;
using System.Threading.Tasks;
using HRM.Core.Models;

namespace HRM.Services.Interfaces;

public interface IPayrollManagementService
{
    Task<CommandResult> GeneratePayroll(DateTime date);
}