using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    ICalculateAllowanceService calculateAllowanceService,
    IExchangeRateService exchangeRateService
) : IPayrollManagementService
{
    private const int MAX_ALLOWED_UNPAID_LEAVE_DAYS = 10;
    private const decimal PROBATION_PERCENTAGE = 0.85m;

    private const string CONFIG_BASIC_SALARY = "1";
    private const string CONFIG_JOB_LEVEL_ADD_ON = "2";
    private const string CONFIG_SPECIAL_SUBSIDY = "3";

    public async Task<IEnumerable<GeneratedPayrollDetail>> GetPayrollByMonthAsync(GetPayrollByMonthCriteria criteria) 
        => await baseService.GetDataAsync<GeneratedPayrollDetail>(criteria);

    public async Task<CommandResult> GeneratePayroll(DateTime date, int actorId)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var payrollResults = await GeneratePayrollResults(date, companyId);

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

    private async Task<IEnumerable<PayrollEmployeeResult>> GeneratePayrollResults(DateTime date, short companyId)
    {
        var employeeAttendantsTask = GetPayrollEmployeeAttendant(companyId, date);
        var grossSalaryConfigTask = salaryStructureService.GetBasicSalaryConfigAsync(companyId);
        var exchangeRatesToUSDTask = exchangeRateService.GetEffectiveExchangeRatesDictAsync();
        var roundingRulesTask = exchangeRateService.GetRoundingRulesDictAsync();
        await Task.WhenAll(employeeAttendantsTask, grossSalaryConfigTask, exchangeRatesToUSDTask, roundingRulesTask);

        var employeeAttendants = await employeeAttendantsTask;
        var grossSalaryConfig = await grossSalaryConfigTask;
        var exchangeRatesToUSD = await exchangeRatesToUSDTask;
        var roundingRules = await roundingRulesTask;
        var selectedConfigValues = ParseConfigValues(grossSalaryConfig);

        var payrollResults = new List<PayrollEmployeeResult>();
        foreach (var e in employeeAttendants)
        {
            var payrollResult = CalculatePayrollResult(e, grossSalaryConfig, selectedConfigValues, exchangeRatesToUSD);
            payrollResult.GeneratedDate = date;

            var allowance = await calculateAllowanceService.CalculateAllowanceAsync(
                new CalculateAllowanceRequest
                {
                    EmpId = e.EmpId,
                    ActualPaidDays = e.ActualPaidDays,
                    CompanyEvent = e.TotalCompanyEventDays,
                    StandardWorkingDays = e.WorkingDays,
                    IsNewHire = e.IsNewHire,
                    IsResigned = e.IsResigned,
                    IsMaternity = e.IsMaternity,
                    TotalUnpaidLeaveDays = e.TotalUnpaidLeaveDays,
                    TotalWorkingDays = e.TotalWorkingDays,
                }
            );

            payrollResult.TransportationAllowance = PayrollHelper.EncryptAndRound(
                allowance.TransportationAllowance,
                0
            );
            payrollResult.ManagementAllowance = PayrollHelper.EncryptAndRound(
                allowance.ManagementAllowance,
                0
            );
            payrollResult.MobilephoneAllowance = PayrollHelper.EncryptAndRound(
                allowance.MobilephoneAllowance,
                0
            );

            // Update employee attendant for TotalIncome calculation
            e.TransportationAllowance = allowance.TransportationAllowance;
            e.ManagementAllowance = allowance.ManagementAllowance;
            e.MobilephoneAllowance = allowance.MobilephoneAllowance;

            var roundedTotalIncome = CurrencyConverter.RoundAmount(
                e.TotalIncome,
                roundingRules,
                e.CurrencyCode
            );
            payrollResult.TotalIncome = PayrollHelper.EncryptSalary(roundedTotalIncome);

            payrollResults.Add(payrollResult);
        }

        return payrollResults;
    }

    private async Task<IEnumerable<PayrollEmployeeAttendant>> GetPayrollEmployeeAttendant(
        int companyId,
        DateTime date
    )
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
            return [];
        }

        return grossSalaryConfig
            .Config.Split([','], StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .ToHashSet();
    }

    private static PayrollEmployeeResult CalculatePayrollResult(
        PayrollEmployeeAttendant employeeAttendant,
        BasicSalaryConfigViewModel grossSalaryConfig,
        HashSet<string> selectedConfigValues,
        Dictionary<string, decimal> exchangeRatesToUSD
    )
    {
        employeeAttendant.ActualPaidDays = CalculateActualPaidDays(employeeAttendant);
        employeeAttendant.SpecialSubsidy = CalculateSpecialSubsidy(employeeAttendant, exchangeRatesToUSD);
        employeeAttendant.GrossSalary = CalculateGrossSalary(employeeAttendant, grossSalaryConfig, selectedConfigValues);

        return new PayrollEmployeeResult
        {
            EmpId = employeeAttendant.EmpId,
            WorkingDays = employeeAttendant.WorkingDays,
            UnpaidLeave = employeeAttendant.TotalUnpaidLeaveDays,
            PaidDays = employeeAttendant.ActualPaidDays,
            CompanyEventLeave = employeeAttendant.TotalCompanyEventDays,
            BasicSalary = employeeAttendant.EncryptBasicSalary,
            JobLevelAddOn = employeeAttendant.EncryptJobLevelAddOn,
            SpecialSubsidy = PayrollHelper.EncryptAndRound(employeeAttendant.SpecialSubsidy),
            CompanyDate = employeeAttendant.CompanyDate,
            ConfirmedDate = employeeAttendant.ConfirmedDate,
            EmploymentStatus = GetEmploymentStatus(employeeAttendant),
            GrossSalary = PayrollHelper.EncryptAndRound(employeeAttendant.GrossSalary),
        };
    }

    private static decimal CalculateActualPaidDays(PayrollEmployeeAttendant employeeAttendant)
    {
        if (employeeAttendant.IsNewHire)
        {
            return employeeAttendant.TotalWorkingDays + employeeAttendant.TotalNewHireWorkingDays;
        }

        if (employeeAttendant.IsResigned || employeeAttendant.IsMaternity || employeeAttendant.TotalUnpaidLeaveDays > MAX_ALLOWED_UNPAID_LEAVE_DAYS)
        {
            return employeeAttendant.TotalWorkingDays;
        }

        return employeeAttendant.WorkingDays - employeeAttendant.TotalUnpaidLeaveDays;
    }

    private static decimal CalculateNewHireSalary(PayrollEmployeeAttendant employeeAttendant, decimal grossSalary)
    {
        var grossSalaryPerDay = employeeAttendant.WorkingDays > 0 ? grossSalary / employeeAttendant.WorkingDays : 0;
        var newHireSalary = employeeAttendant.TotalNewHireWorkingDays * grossSalaryPerDay * PROBATION_PERCENTAGE;
        var confirmedSalary = employeeAttendant.TotalWorkingDays * grossSalaryPerDay;
        return newHireSalary + confirmedSalary;
    }

    private static decimal CalculateGrossSalary(
        PayrollEmployeeAttendant employeeAttendant,
        BasicSalaryConfigViewModel grossSalaryConfig,
        HashSet<string> selectedConfigValues
    )
    {
        if (grossSalaryConfig == null || selectedConfigValues == null || selectedConfigValues.Count == 0)
        {
            return 0;
        }

        var grossSalary = grossSalaryConfig
            .ConfigOptions.Where(option => selectedConfigValues.Contains(option.Value))
            .Select(option =>
                option.Value switch
                {
                    CONFIG_BASIC_SALARY => PayrollHelper.DecryptSalary(employeeAttendant.EncryptBasicSalary),
                    CONFIG_JOB_LEVEL_ADD_ON => PayrollHelper.DecryptSalary(employeeAttendant.EncryptJobLevelAddOn),
                    CONFIG_SPECIAL_SUBSIDY => employeeAttendant.SpecialSubsidy,
                    _ => 0m,
                }
            )
            .Sum();

        return employeeAttendant.IsNewHire
            ? CalculateNewHireSalary(employeeAttendant, grossSalary)
            : grossSalary;
    }

    private static decimal CalculateSpecialSubsidy(
        PayrollEmployeeAttendant employeeAttendant,
        Dictionary<string, decimal> exchangeRatesToUSD
    )
    {
        var specialSubsidy = PayrollHelper.DecryptSalary(employeeAttendant.EncryptSpecialSubsidy);
        return CurrencyConverter.ConvertCurrency(
            specialSubsidy,
            employeeAttendant.SpecialSubsidyCurrencyCode,
            employeeAttendant.CurrencyCode,
            exchangeRatesToUSD
        );
    }

    private static EmploymentStatus GetEmploymentStatus(PayrollEmployeeAttendant e)
    {
        if (e.IsNewHire)
        {
            return e.TotalWorkingDays == 0
                ? EmploymentStatus.Probation
                : EmploymentStatus.Confirmed;
        }

        if (e.IsResigned)
        {
            return EmploymentStatus.Resignation;
        }

        return e.IsMaternity ? EmploymentStatus.Maternity : EmploymentStatus.Normal;
    }
}
