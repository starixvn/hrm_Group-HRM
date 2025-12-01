using System;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayrollSetting;
using HRM.Criteria.Group;
using HRM.Models.PayrollManagement;
using HRM.Services.Helper;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement.PayrollManagement;

public class CalculateAllowanceService(IBaseService baseService) : ICalculateAllowanceService
{
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

    public async Task<CalculatedAllowanceResult> CalculateAllowanceAsync(
        int empId,
        decimal actualPaidDays,
        decimal companyEvent,
        int standardWorkingDays,
        bool isNewHire,
        bool isResigned,
        bool isMaternity,
        decimal totalUnpaidLeaveDays,
        decimal totalWorkingDays
    )
    {
        var allowances = await baseService.GetDataAsync<EmployeePayrollAllowance>(
            new GetEmployeePayrollAllowanceByEmpIdCriteria { EmpId = empId }
        );

        var result = new CalculatedAllowanceResult();

        var transportationAllowance = allowances?.FirstOrDefault(a =>
            a.AllowanceName?.Trim()
                .Equals("Transportation Allowance", StringComparison.OrdinalIgnoreCase) == true
        );

        if (
            transportationAllowance != null
            && !string.IsNullOrEmpty(transportationAllowance.Amount)
        )
        {
            var transportAmount = PayrollHelper.DecryptSalary(transportationAllowance.Amount);
            result.TransportationAllowance = CalculateTransportationAllowance(
                actualPaidDays,
                companyEvent,
                transportAmount,
                standardWorkingDays
            );
        }

        // Get Management Allowance (return original value, no calculation)
        var managementAllowance = allowances?.FirstOrDefault(a =>
            a.AllowanceName?.Trim()
                .Equals("Management Allowance", StringComparison.OrdinalIgnoreCase) == true
        );

        if (managementAllowance != null && !string.IsNullOrEmpty(managementAllowance.Amount))
        {
            result.ManagementAllowance = PayrollHelper.DecryptSalary(managementAllowance.Amount);
        }

        // Calculate Mobilephone Allowance
        var mobilephoneAllowance = allowances?.FirstOrDefault(a =>
            a.AllowanceName?.Trim()
                .Equals("Mobilephone Allowance", StringComparison.OrdinalIgnoreCase) == true
        );

        if (mobilephoneAllowance != null && !string.IsNullOrEmpty(mobilephoneAllowance.Amount))
        {
            var mobilephoneAmount = PayrollHelper.DecryptSalary(mobilephoneAllowance.Amount);
            var isProbation = isNewHire && totalWorkingDays == 0;
            result.MobilephoneAllowance = CalculateMobilephoneAllowance(
                actualPaidDays,
                mobilephoneAmount,
                standardWorkingDays,
                isProbation,
                isResigned,
                isMaternity,
                totalUnpaidLeaveDays
            );
        }

        return result;
    }
}
