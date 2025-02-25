namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IShiftService
    {
        IList<Shift> GetAll(int statusId = 1, bool isShowOff = false);

        IList<Shift> GetByQuery(string query, bool isShowOff);

        IList<SelectListItem> GetSelectList(int statusId = 1, bool isShowOff = false);

        Shift GetByDate(int employeeId, DateTime date);

        Shift GetById(int shiftId);

        CommandResult Create(int actorId, CreateShiftModel model);

        CommandResult Edit(int actorId, EditShiftModel model);

        CommandResult Delete(int shiftId);
    }
}