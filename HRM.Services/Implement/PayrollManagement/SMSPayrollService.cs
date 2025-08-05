using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Fanex.Data;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.PayrollManagement;
using HRM.Models.Payroll;
using HRM.Models.PayrollManagement;
using HRM.Services.Interfaces;
using Newtonsoft.Json;

namespace HRM.Services.Implement.PayrollManagement;

public class SMSPayrollManagementService(
    IBaseService baseService,
    IUserControlSession session) : IPayrollManagementService
{
    private const int MAX_ALLOWED_UNPAID_LEAVE_DAYS = 10;
    private const decimal PROBATION_PERCENTAGE = 0.85m;

    public async Task<IEnumerable<GeneratedPayrollDetail>> GetPayrollByMonthAsync(GetPayrollByMonthCriteria criteria)
    {
        return await baseService.GetDataAsync<GeneratedPayrollDetail>(criteria);
    }

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
        var paidDays = CalculateActualPaidDays(employeeAttendant);
        var payrollResult = new PayrollEmployeeResult
        {
            EmpId = employeeAttendant.EmpId,
            WorkingDays = employeeAttendant.WorkingDays,
            UnpaidLeave = employeeAttendant.TotalUnpaidLeaveDays,
            PaidDays = paidDays,
            BasicSalary = employeeAttendant.Salary,
            CompanyDate = employeeAttendant.CompanyDate,
            ConfirmedDate = employeeAttendant.ConfirmedDate,
        };
        employeeAttendant.BasicSalary = DecryptSalary(employeeAttendant.Salary);
        employeeAttendant.ActualPaidDays = paidDays;

        decimal grossSalary;
        if (employeeAttendant.TotalNewHireWorkingDays > 0)
        {
            payrollResult.EmploymentStatus = GetEmploymentStatusNewHire(employeeAttendant);
            grossSalary = CalculateProbationSalary(employeeAttendant);
        }
        else if (employeeAttendant.IsResigned)
        {
            payrollResult.EmploymentStatus = EmploymentStatus.Resignation;
            grossSalary = CalculateSalary(employeeAttendant);
        }
        else if (employeeAttendant.IsMaternity)
        {
            payrollResult.EmploymentStatus = EmploymentStatus.Maternity;
            grossSalary = CalculateSalary(employeeAttendant);
        }
        else
        {
            payrollResult.EmploymentStatus = EmploymentStatus.Normal;
            grossSalary = CalculateSalary(employeeAttendant);
        }
        payrollResult.GrossSalary = Cryptography.EncryptPassword(grossSalary.ToString(CultureInfo.InvariantCulture));
        return payrollResult;
    }

    private static decimal DecryptSalary(string salary)
    {
        if (string.IsNullOrEmpty(salary))
        {
            return 0;
        }
        try
        {

            salary = Cryptography.DecryptPassword(salary);
            return decimal.Parse(salary);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    private static decimal CalculateActualPaidDays(PayrollEmployeeAttendant employeeAttendant)
    {
        if (employeeAttendant.TotalNewHireWorkingDays > 0)
        {
            return employeeAttendant.TotalWorkingDays + employeeAttendant.TotalNewHireWorkingDays;
        }
        if (employeeAttendant.IsResigned || employeeAttendant.IsMaternity)
        {
            return employeeAttendant.TotalWorkingDays;
        }
        if (employeeAttendant.TotalUnpaidLeaveDays > MAX_ALLOWED_UNPAID_LEAVE_DAYS)
        {
            return employeeAttendant.TotalWorkingDays;
        }
        return employeeAttendant.WorkingDays - employeeAttendant.TotalUnpaidLeaveDays;
    }

    private static decimal CalculateProbationSalary(PayrollEmployeeAttendant employeeAttendant)
    {
        var newHireSalary = employeeAttendant.TotalNewHireWorkingDays * employeeAttendant.BasicSalaryPerDay * PROBATION_PERCENTAGE;
        var confirmedSalary = employeeAttendant.TotalWorkingDays * employeeAttendant.BasicSalaryPerDay;
        return newHireSalary + confirmedSalary;
    }

    private static decimal CalculateSalary(PayrollEmployeeAttendant employeeAttendant)
    {
        return employeeAttendant.ActualPaidDays * employeeAttendant.BasicSalaryPerDay;
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