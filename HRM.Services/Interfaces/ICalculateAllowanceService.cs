using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Models.PayrollManagement;

namespace HRM.Services.Interfaces;

public interface ICalculateAllowanceService
{
    decimal CalculateTransportationAllowance(
        decimal actualPaidDays,
        decimal companyEvent,
        decimal transportAmount,
        int standardWorkingDays
    );

    decimal CalculateMobilephoneAllowance(
        decimal actualPaidDays,
        decimal mobilephoneAmount,
        int standardWorkingDays,
        bool isProbation,
        bool isResigned,
        bool isMaternity,
        decimal unpaidLeaveDays
    );

    Task<CalculatedAllowanceResult> CalculateAllowanceAsync(
        PayrollEmployeeAttendant employeeAttendant,
        Dictionary<string, decimal> exchangeRatesToUSD
    );
}
