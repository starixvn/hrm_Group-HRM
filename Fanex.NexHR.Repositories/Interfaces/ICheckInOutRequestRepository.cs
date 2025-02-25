namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface ICheckInOutRequestRepository
    {
        IList<CheckInOutRequest> GetByDate(ICriteria criteria);

        CheckInOutRequest GetById(ICriteria criteria);

        IList<CheckInOutRequest> GetHistoryById(ICriteria criteria);
    }
}
