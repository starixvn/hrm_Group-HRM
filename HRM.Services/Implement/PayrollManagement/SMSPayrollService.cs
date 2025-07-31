using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;
using HRM.Services.Interfaces;
using Newtonsoft.Json;

namespace HRM.Services.Implement.PayrollManagement;

public class SMSPayrollManagementService(IBaseService baseService, IUserControlSession session) : IPayrollManagementService
{
    private const int MAX_ALLOWED_UNPAID_LEAVE_DAYS = 10;
    private const decimal PROBATION_PERCENTAGE = 0.85m;
    public async Task<CommandResult> GeneratePayroll(DateTime date)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var employeeAttendants = await GetPayrollEmployeeAttendant(companyId, date);
        var payrollResults = new List<PayrollEmployeeResult>();

        foreach (var employeeAttendant in employeeAttendants)
        {
            var payrollResult = CalculatePayrollResult(employeeAttendant);
            payrollResults.Add(payrollResult);
        }
        var criteria = new CalculatePayrollCriteria
        {
            CompanyId = companyId,
            Date = date,
            PayrollListJson = JsonConvert.SerializeObject(payrollResults),
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);
        return result;
    }

    private async Task<IEnumerable<PayrollEmployeeAttendant>> GetPayrollEmployeeAttendant(int companyId, DateTime date)
    {
        var criteria = new GetPayrollAttendantEmployeeCriteria
        {
            CompanyId = companyId,
            Date = date,
        };
        return await baseService.GetDataAsync<PayrollEmployeeAttendant>(criteria);
    }

    private static PayrollEmployeeResult CalculatePayrollResult(PayrollEmployeeAttendant employeeAttendant)
    {
        var payrollResult = new PayrollEmployeeResult
        {
            EmpId = employeeAttendant.EmpId,
            WorkingDays = employeeAttendant.WorkingDays,
            PaidDays = employeeAttendant.TotalWorkingDays + employeeAttendant.TotalNewHireWorkingDays,
            UnpaidLeave = employeeAttendant.TotalUnpaidLeaveDays,
            BasicSalary = employeeAttendant.Salary,
            JoinDate = employeeAttendant.JoinDate,
            ConfirmedDate = employeeAttendant.ConfirmedDate,
        };
        employeeAttendant.BasicSalary = DecryptSalary(employeeAttendant.Salary);

        decimal grossSalary = 0;
        if (employeeAttendant.TotalNewHireWorkingDays > 0)
        {
            payrollResult.EmploymentStatus = GetEmploymentStatusNewHire(employeeAttendant);
            grossSalary = CalculateProbationCaseSalary(employeeAttendant);
        }
        else if (employeeAttendant.IsResigned)
        {
            payrollResult.EmploymentStatus = EmploymentStatus.Resignation;
            grossSalary = CalculateSalaryPerDay(employeeAttendant);
        }
        else if (employeeAttendant.IsMaternity)
        {
            payrollResult.EmploymentStatus = EmploymentStatus.Maternity;
            grossSalary = CalculateSalaryPerDay(employeeAttendant);
        }
        else
        {
            payrollResult.EmploymentStatus = EmploymentStatus.Normal;
            grossSalary = CalculateNormalCaseSalary(employeeAttendant);
        }
        payrollResult.GrossSalary = Cryptography.EncryptPassword(grossSalary.ToString());
        return payrollResult;
    }

    private static decimal DecryptSalary(string salary)
    {
        if (string.IsNullOrEmpty(salary))
        {
            return 0;
        }
        _ = decimal.TryParse(Cryptography.DecryptPassword(salary), out var basicSalary);
        return basicSalary;
    }

    private static decimal CalculateNormalCaseSalary(PayrollEmployeeAttendant employeeAttendant)
    {
        if (employeeAttendant.TotalUnpaidLeaveDays > MAX_ALLOWED_UNPAID_LEAVE_DAYS)
        {
            return CalculateSalaryPerDay(employeeAttendant);
        }
        return employeeAttendant.BasicSalary * (employeeAttendant.WorkingDays - employeeAttendant.TotalUnpaidLeaveDays);
    }

    private static decimal CalculateProbationCaseSalary(PayrollEmployeeAttendant employeeAttendant)
    {
        var newHireSalary = employeeAttendant.TotalNewHireWorkingDays * employeeAttendant.BasicSalaryPerDay * PROBATION_PERCENTAGE;
        var confirmedSalary = CalculateSalaryPerDay(employeeAttendant);
        return newHireSalary + confirmedSalary;
    }

    private static decimal CalculateSalaryPerDay(PayrollEmployeeAttendant employeeAttendant)
    {
        return employeeAttendant.BasicSalaryPerDay * employeeAttendant.TotalWorkingDays;
    }

    private static EmploymentStatus GetEmploymentStatusNewHire(PayrollEmployeeAttendant employeeAttendant)
    {
        if (employeeAttendant.TotalWorkingDays == 0)
        {
            return EmploymentStatus.Probation;
        }
        return EmploymentStatus.Confirmed;
    }
}