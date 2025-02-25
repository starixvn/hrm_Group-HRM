namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.Request;

    public interface ICheckInOutRequestRepository
    {
        IList<CheckInOutRequest> GetByDate(ICriteria criteria);

        CheckInOutRequest GetById(ICriteria criteria);

        IList<CheckInOutRequest> GetHistoryById(ICriteria criteria);
    }
}
