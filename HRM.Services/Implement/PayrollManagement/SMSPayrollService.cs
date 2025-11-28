using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using HRM.Services.Helper;
using HRM.Services.Interfaces;
using HRM.ViewModels.SalaryStructure;
using Newtonsoft.Json;

namespace HRM.Services.Implement.PayrollManagement;

public class SMSPayrollManagementService(
    IBaseService baseService,
    IUserControlSession session,
    ISalaryStructureService salaryStructureService,
    IExchangeRateService exchangeRateService) : IPayrollManagementService
{
    private const int MAX_ALLOWED_UNPAID_LEAVE_DAYS = 10;
    private const decimal PROBATION_PERCENTAGE = 0.85m;

    private const string CONFIG_BASIC_SALARY = "1";
    private const string CONFIG_JOB_LEVEL_ADD_ON = "2";
    private const string CONFIG_SPECIAL_SUBSIDY = "3";

    public async Task<IEnumerable<GeneratedPayrollDetail>> GetPayrollByMonthAsync(GetPayrollByMonthCriteria criteria)
    {
        return await baseService.GetDataAsync<GeneratedPayrollDetail>(criteria);
    }

    public async Task<CommandResult> GeneratePayroll(DateTime date, int actorId)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var employeeAttendants = await GetPayrollEmployeeAttendant(companyId, date);
        var grossSalaryConfig = await salaryStructureService.GetBasicSalaryConfigAsync(companyId);
        var exchangeRatesToUSD = await exchangeRateService.GetEffectiveExchangeRatesDictAsync();
        var selectedConfigValues = ParseConfigValues(grossSalaryConfig);

        var payrollResults = employeeAttendants
            .Select(employeeAttendant =>
            {
                var payrollResult = CalculatePayrollResult(employeeAttendant, grossSalaryConfig, selectedConfigValues, exchangeRatesToUSD);
                payrollResult.GeneratedDate = date;
                // Todo: Calculate Allowance and Total Income later



                return payrollResult;
            })
            .ToList();
        var criteria = new CalculatePayrollCriteria
        {
            ModifiedBy = actorId,
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

    private static HashSet<string> ParseConfigValues(BasicSalaryConfigViewModel grossSalaryConfig)
    {
        if (grossSalaryConfig == null || string.IsNullOrWhiteSpace(grossSalaryConfig.Config))
        {
            return null;
        }

        return grossSalaryConfig.Config
            .Split([','], StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .ToHashSet();
    }

    private static PayrollEmployeeResult CalculatePayrollResult(PayrollEmployeeAttendant employeeAttendant, BasicSalaryConfigViewModel grossSalaryConfig, HashSet<string> selectedConfigValues, Dictionary<string, decimal> exchangeRatesToUSD)
    {
        employeeAttendant.ActualPaidDays = CalculateActualPaidDays(employeeAttendant);
        employeeAttendant.GrossSalary = CalculateGrossSalary(employeeAttendant, grossSalaryConfig, selectedConfigValues, exchangeRatesToUSD);

        return new PayrollEmployeeResult
        {
            EmpId = employeeAttendant.EmpId,
            WorkingDays = employeeAttendant.WorkingDays,
            UnpaidLeave = employeeAttendant.TotalUnpaidLeaveDays,
            PaidDays = employeeAttendant.ActualPaidDays,
            BasicSalary = employeeAttendant.EncryptBasicSalary,
            JobLevelAddOn = employeeAttendant.EncryptJobLevelAddOn,
            SpecialSubsidy = employeeAttendant.EncryptSpecialSubsidy,
            CompanyDate = employeeAttendant.CompanyDate,
            ConfirmedDate = employeeAttendant.ConfirmedDate,
            EmploymentStatus = GetEmploymentStatus(employeeAttendant),
            GrossSalary = Cryptography.EncryptPassword(Math.Round(employeeAttendant.GrossSalary, 0).ToString(CultureInfo.InvariantCulture)),
        };
    }

    public decimal DecryptSalary(string salary) => DecryptSalaryStatic(salary);

    private static decimal DecryptSalaryStatic(string salary)
    {
        if (string.IsNullOrEmpty(salary))
        {
            return 0;
        }
        try
        {
            salary = Cryptography.DecryptPassword(salary);
            return decimal.Parse(salary, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    private static decimal CalculateActualPaidDays(PayrollEmployeeAttendant employeeAttendant)
    {
        if (employeeAttendant.IsNewHire)
        {
            return employeeAttendant.TotalWorkingDays + employeeAttendant.TotalNewHireWorkingDays;
        }

        if (employeeAttendant.IsResigned || employeeAttendant.IsMaternity || 
            employeeAttendant.TotalUnpaidLeaveDays > MAX_ALLOWED_UNPAID_LEAVE_DAYS)
        {
            return employeeAttendant.TotalWorkingDays;
        }

        return employeeAttendant.WorkingDays - employeeAttendant.TotalUnpaidLeaveDays;
    }

    private static decimal CalculateNewHireSalary(PayrollEmployeeAttendant employeeAttendant)
    {
        var newHireSalary = employeeAttendant.TotalNewHireWorkingDays * employeeAttendant.GrossSalaryPerDay * PROBATION_PERCENTAGE;
        var confirmedSalary = employeeAttendant.TotalWorkingDays * employeeAttendant.GrossSalaryPerDay;
        return newHireSalary + confirmedSalary;
    }

    private static decimal CalculateGrossSalary(PayrollEmployeeAttendant employeeAttendant, BasicSalaryConfigViewModel grossSalaryConfig, HashSet<string> selectedConfigValues, Dictionary<string, decimal> exchangeRatesToUSD)
    {
        if (grossSalaryConfig == null || selectedConfigValues == null || selectedConfigValues.Count == 0)
        {
            return 0;
        }

        var grossSalary = 0m;
        foreach (var option in grossSalaryConfig.ConfigOptions)
        {
            if (!selectedConfigValues.Contains(option.Value))
            {
                continue;
            }

            grossSalary += option.Value switch
            {
                CONFIG_BASIC_SALARY => DecryptSalaryStatic(employeeAttendant.EncryptBasicSalary),
                CONFIG_JOB_LEVEL_ADD_ON => DecryptSalaryStatic(employeeAttendant.EncryptJobLevelAddOn),
                CONFIG_SPECIAL_SUBSIDY => CalculateSpecialSubsidy(employeeAttendant, exchangeRatesToUSD),
                _ => 0m
            };
        }

        return employeeAttendant.IsNewHire ? CalculateNewHireSalary(employeeAttendant) : grossSalary;
    }

    private static decimal CalculateSpecialSubsidy(PayrollEmployeeAttendant employeeAttendant, Dictionary<string, decimal> exchangeRatesToUSD)
    {
        var specialSubsidy = DecryptSalaryStatic(employeeAttendant.EncryptSpecialSubsidy);
        return CurrencyConverter.ConvertCurrency(specialSubsidy, employeeAttendant.SpecialSubsidyCurrencyCode, employeeAttendant.CurrencyCode, exchangeRatesToUSD);
    }

    private static EmploymentStatus GetEmploymentStatus(PayrollEmployeeAttendant e)
    {
        if (e.IsNewHire)
        {
            return e.TotalWorkingDays == 0 ? EmploymentStatus.Probation : EmploymentStatus.Confirmed;
        }

        if (e.IsResigned)
        {
            return EmploymentStatus.Resignation;
        }

        return e.IsMaternity ? EmploymentStatus.Maternity : EmploymentStatus.Normal;
    }
}