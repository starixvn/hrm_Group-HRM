namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.Request;

    public interface IKeepInViewRequestRepository
    {
        IList<KeepInViewRequest> GetByDate(ICriteria criteria);

        KeepInViewRequest GetById(ICriteria criteria);

        IList<KeepInViewRequest> GetHistoryById(ICriteria criteria);
    }
}
