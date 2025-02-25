namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;

    public interface IMemoService
    {
        IList<Memo> GetAll(int year, int category);

        Memo GetById(int id);

        CommandResult Create(Memo model);

        CommandResult Edit(Memo model);

        CommandResult Delete(int id);
    }
}