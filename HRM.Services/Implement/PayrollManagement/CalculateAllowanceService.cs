using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayrollSetting;
using HRM.Models.PayrollManagement;
using HRM.Services.Helper;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement.PayrollManagement;

public class CalculateAllowanceService(IBaseService baseService) : ICalculateAllowanceService
{
    public async Task<CalculatedAllowanceResult> CalculateAllowanceAsync(
        PayrollEmployeeAttendant employeeAttendant,
        Dictionary<string, decimal> exchangeRatesToUSD
    )
    {
        var allowances = await baseService.GetDataAsync<EmployeePayrollAllowance>(
            new GetEmployeePayrollAllowanceByEmpIdCriteria { EmpId = employeeAttendant.EmpId }
        );

        var result = new CalculatedAllowanceResult();

        var transportationAllowance = allowances?.FirstOrDefault(a =>
            a.AllowanceName?.Trim().ToLowerInvariant() == "transportation allowance"
        );

        if (
            transportationAllowance != null
            && !string.IsNullOrEmpty(transportationAllowance.Amount)
        )
        {
            var transportAmount = PayrollHelper.DecryptSalary(transportationAllowance.Amount);
            var convertedTransportAmount = CurrencyConverter.ConvertCurrency(
                transportAmount,
                transportationAllowance.CurrencyCode,
                employeeAttendant.CurrencyCode,
                exchangeRatesToUSD
            );
            result.TransportationAllowance = CalculateTransportationAllowance(
                employeeAttendant.ActualPaidDays,
                employeeAttendant.TotalCompanyEventDays,
                convertedTransportAmount,
                employeeAttendant.WorkingDays
            );
        }

        var managementAllowance = allowances?.FirstOrDefault(a =>
            a.AllowanceName?.Trim().ToLowerInvariant() == "management allowance"
        );

        if (managementAllowance != null && !string.IsNullOrEmpty(managementAllowance.Amount))
        {
            var managementAmount = PayrollHelper.DecryptSalary(managementAllowance.Amount);
            result.ManagementAllowance = CurrencyConverter.ConvertCurrency(
                managementAmount,
                managementAllowance.CurrencyCode,
                employeeAttendant.CurrencyCode,
                exchangeRatesToUSD
            );
        }

        var mobilephoneAllowance = allowances?.FirstOrDefault(a =>
            a.AllowanceName?.Trim().ToLowerInvariant() == "mobilephone allowance"
        );

        if (mobilephoneAllowance != null && !string.IsNullOrEmpty(mobilephoneAllowance.Amount))
        {
            var mobilephoneAmount = PayrollHelper.DecryptSalary(mobilephoneAllowance.Amount);
            var convertedMobilephoneAmount = CurrencyConverter.ConvertCurrency(
                mobilephoneAmount,
                mobilephoneAllowance.CurrencyCode,
                employeeAttendant.CurrencyCode,
                exchangeRatesToUSD
            );
            var isProbation = employeeAttendant.IsNewHire && employeeAttendant.TotalWorkingDays == 0;
            result.MobilephoneAllowance = CalculateMobilephoneAllowance(
                employeeAttendant.ActualPaidDays,
                convertedMobilephoneAmount,
                employeeAttendant.WorkingDays,
                isProbation,
                employeeAttendant.IsResigned,
                employeeAttendant.IsMaternity,
                employeeAttendant.TotalUnpaidLeaveDays
            );
        }

        return result;
    }

    public decimal CalculateTransportationAllowance(
        decimal actualPaidDays,
        decimal companyEvent,
        decimal transportAmount,
        int standardWorkingDays
    )
    {
        if (transportAmount <= 0)
        {
            return 0;
        }

        if (actualPaidDays >= standardWorkingDays)
        {
            return transportAmount;
        }

        var daysForCalculation = actualPaidDays - companyEvent;
        if (daysForCalculation <= 0)
        {
            return 0;
        }

        return transportAmount / standardWorkingDays * daysForCalculation;
    }

    public decimal CalculateMobilephoneAllowance(
        decimal actualPaidDays,
        decimal mobilephoneAmount,
        int standardWorkingDays,
        bool isProbation,
        bool isResigned,
        bool isMaternity,
        decimal unpaidLeaveDays
    )
    {
        if (mobilephoneAmount <= 0)
        {
            return 0;
        }

        // Check if need to calculate prorated: probation, resignation, maternity, or unpaid leave > half month
        var halfMonth = standardWorkingDays / 2.0m;
        var needProrated = isProbation || isResigned || isMaternity || unpaidLeaveDays > halfMonth;

        if (!needProrated)
        {
            return mobilephoneAmount;
        }

        // Calculate: mobilephoneAmount / standardWorkingDays * actualPaidDays
        return mobilephoneAmount / standardWorkingDays * actualPaidDays;
    }
}
