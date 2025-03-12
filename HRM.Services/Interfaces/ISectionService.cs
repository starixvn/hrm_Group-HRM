using HRM.Models.Section;

namespace HRM.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;

public interface ISectionService
{
    Task<Section> GetByIdAsync(int id);

    Task<IList<Section>> GetSectionAsync(bool? isActive = true);

    Task<CommandResult> DeleteAsync(int actorId, int id);
}