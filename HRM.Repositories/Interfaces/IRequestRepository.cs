namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRM.Criteria.Request;
    using HRM.Models.Request;

    public interface IRequestRepository
    {
        IList<RequestEntity> GetRequests(ICriteria criteria);

        RequestRelatedPeopleModel GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria);

        Task<VerifierAndApproverForRequest> GetVerifierAndApproverAsync(ICriteria criteria);

        RequestEntity GetRequestToIgnoreById(ICriteria criteria);

        IList<LeaveDisciplineModel> GetLeaveRequestToReviewDiscipline(ICriteria criteria);
    }
}