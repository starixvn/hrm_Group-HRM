namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface ILateInEarlyOutRequestRepository
    {
        IList<LateInEarlyOutRequest> GetByDate(ICriteria criteria);

        LateInEarlyOutRequest GetById(ICriteria criteria);

        IList<LateInEarlyOutRequest> GetHistoryById(ICriteria criteria);
    }
}
