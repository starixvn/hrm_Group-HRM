namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;

    public interface ISectionService
    {
        Section GetById(int id);

        IList<Section> GetSection(bool? isActive = true);

        CommandResult Delete(int actorId, int id);
    }
}