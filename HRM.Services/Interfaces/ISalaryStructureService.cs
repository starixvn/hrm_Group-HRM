namespace HRM.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using Models;

public interface ISalaryStructureService
{
    Task<IList<SalaryStructure>> GetSalaryStructureAsync();

    Task<SalaryStructure> GetByIdAsync(int salaryStructureId);

    Task<CommandResult> DeleteAsync(int actorId, int id);
}