namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Models;

    public interface IMemoService
    {
        IList<Memo> GetAll(int year, int category);

        Memo GetById(int id);

        CommandResult Create(Memo model);

        CommandResult Edit(Memo model);

        CommandResult Delete(int id);
    }
}