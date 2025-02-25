namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;

    public interface IHolidayService
    {
        Holiday GetById(int holidayId);

        IList<Holiday> GetByYear(int year);

        CommandResult Create(int actorId, Holiday model);

        CommandResult Edit(int actorId, Holiday model);

        CommandResult Delete(int actorId, int id);
    }
}