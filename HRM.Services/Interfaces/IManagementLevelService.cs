using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.ManagementLevel;
using HRM.Models;

namespace HRM.Services.Interfaces;

public interface IManagementLevelService
{
    Task<IList<ManagementLevel>> GetManagementLevel();

    Task<CommandResult> Insert(InsertManagementLevelCriteria criteria);

    Task<CommandResult> Update(UpdateManagementLevelCriteria criteria);

    Task<CommandResult> Delete(int id, int actorId);
}