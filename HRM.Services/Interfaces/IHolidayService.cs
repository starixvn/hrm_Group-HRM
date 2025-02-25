namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Models;

    public interface IHolidayService
    {
        Holiday GetById(int holidayId);

        IList<Holiday> GetByYear(int year);

        CommandResult Create(int actorId, Holiday model);

        CommandResult Edit(int actorId, Holiday model);

        CommandResult Delete(int actorId, int id);
    }
}