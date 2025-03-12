namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Models.Request;

    public interface ICheckInOutRequestRepository
    {
        Task<IList<CheckInOutRequest>> GetByDateAsync(ICriteria criteria);
        Task<CheckInOutRequest> GetByIdAsync(ICriteria criteria);
        Task<IList<CheckInOutRequest>> GetHistoryByIdAsync(ICriteria criteria);
    }
}
