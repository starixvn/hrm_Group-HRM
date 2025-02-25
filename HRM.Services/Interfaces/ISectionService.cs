namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Models;

    public interface ISectionService
    {
        Section GetById(int id);

        IList<Section> GetSection(bool? isActive = true);

        CommandResult Delete(int actorId, int id);
    }
}