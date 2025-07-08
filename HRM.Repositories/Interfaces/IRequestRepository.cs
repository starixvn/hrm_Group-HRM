using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Criteria.Request;
using HRM.Models;
using HRM.Models.Request;

namespace HRM.Repositories.Interfaces;

public interface IRequestRepository
{
    Task<IList<RequestEntity>> GetRequestAsync(ICriteria criteria);

    Task<RequestRelatedPeopleModel> GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria);

    Task<VerifierAndApproverForRequest> GetVerifierAndApproverAsync(ICriteria criteria);

    Task<RequestEntity> GetRequestToIgnoreById(ICriteria criteria);

    Task<List<LeaveDisciplineModel>> GetLeaveRequestToReviewDiscipline(ICriteria criteria);

    IEnumerable<RequestEntity> MappingNexRequestToGroupRequest(List<NexRequestModel> requests, Employee approver);

    int? GetNexRequestStatus(string requestStatusId);

    int GetGroupByNexRequestStatus(int requestStatusId);

    string GetNexRequestType(int requestTypeId);

    int GetNexLeaveType(int leaveTypeId);
}