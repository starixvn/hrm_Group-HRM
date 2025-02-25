namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.API.Request;

    public interface IAPITodolistRequestRepository
    {
        IList<RequestEntity> GetPendingRequestByUserId(ICriteria criteria);
    }
}