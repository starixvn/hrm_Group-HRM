using HRM.Models.Group;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRM.Services.Interfaces;

public interface IMonthlyPayrollService
{
    Task CalculateMonthlySalary(IEnumerable<GroupEmployeeModel> employees);
}

