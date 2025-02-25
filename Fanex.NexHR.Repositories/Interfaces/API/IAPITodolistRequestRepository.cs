namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.API.Request;

    public interface IAPITodolistRequestRepository
    {
        IList<RequestEntity> GetPendingRequestByUserId(ICriteria criteria);
    }
}