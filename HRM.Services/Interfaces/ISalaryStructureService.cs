namespace HRM.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models.SalaryStructure;

public interface ISalaryStructureService
{
    Task<IEnumerable<SalaryStructure>> GetSalaryStructureAsync(int companyId);

    Task<SalaryStructure> GetByIdAsync(int salaryStructureId);

    Task<CommandResult> DeleteAsync(int actorId, int id);

    Task<string> GetBasicSalaryConfigAsync(short companyId);
}
