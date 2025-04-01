using HRM.Repositories.Interfaces;

namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Models.Request;

public class LateInEarlyOutRequestRepository : ILateInEarlyOutRequestRepository
{
    public async Task<IList<LateInEarlyOutRequest>> GetByDateAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var entity = await db.QueryAsync<LateInEarlyOutRequest, Requester, Requester, Requester, Requester, Requester, LateInEarlyOutRequest>(
            (request, employee, verifier, approver, hr, approvedBy) =>
            {
                request.Requester = employee;
                request.Verifier = verifier;
                request.Approver = approver;
                request.HR = hr;
                request.ApprovedBy = approvedBy;
                return request;
            },
            splitOn: "EmpId, VerifierId, ApproverId, HRId, ApprovedById",
            parameterSet: criteria);

        return entity.ToList();
    }

    public async Task<LateInEarlyOutRequest> GetByIdAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var entity = await db.QueryEntityAsync<LateInEarlyOutRequest, Requester, Requester, Requester, Requester, LateInEarlyOutRequest>(
            (request, employee, verifier, approver, hr) =>
            {
                request.Requester = employee;
                request.Verifier = verifier;
                request.Approver = approver;
                request.HR = hr;
                return request;
            },
            splitOn: "EmpId, VerifierId, ApproverId, HRId",
            parameterSet: criteria);

        return entity;
    }

    public async Task<IList<LateInEarlyOutRequest>> GetHistoryByIdAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var entity = await db.QueryAsync<LateInEarlyOutRequest, Requester, Requester, Requester, Requester, Requester, LateInEarlyOutRequest>(
            (request, employee, verifier, approver, hr, modifier) =>
            {
                request.Requester = employee;
                request.Verifier = verifier;
                request.Approver = approver;
                request.HR = hr;
                request.Modifier = modifier;
                return request;
            },
            splitOn: "EmpId, VerifierId, ApproverId, HRId, ModifierId",
            parameterSet: criteria);

        return entity.ToList();
    }
}