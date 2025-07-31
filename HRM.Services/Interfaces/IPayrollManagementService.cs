using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.PayrollManagement;
using HRM.Models.Payroll;

namespace HRM.Services.Interfaces;

public interface IPayrollManagementService
{
    Task<CommandResult> GeneratePayroll(DateTime date);
    Task<IEnumerable<GeneratedPayrollDetail>> GetPayrollByMonthAsync(GetPayrollByMonthCriteria criteria);
}