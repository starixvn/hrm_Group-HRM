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
        int empId,
        decimal actualPaidDays,
        decimal companyEvent,
        int standardWorkingDays,
        bool isNewHire,
        bool isResigned,
        bool isMaternity,
        decimal totalUnpaidLeaveDays,
        decimal totalWorkingDays
    );
}
