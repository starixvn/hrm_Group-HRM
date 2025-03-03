namespace HRM.Services.Interfaces;

using System.Collections.Generic;
using HRM.Core.Models;
using Models;
using System.Threading.Tasks;

public interface ISectionService
{
    Section GetById(int id);

    Task<Section> GetByIdAsync(int id);

    IList<Section> GetSection(bool? isActive = true);

    Task<IList<Section>> GetSectionAsync(bool? isActive = true);

    CommandResult Delete(int actorId, int id);

    Task<CommandResult> DeleteAsync(int actorId, int id);
}