namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public class LateInEarlyOutRequestRepository : ILateInEarlyOutRequestRepository
    {
        public IList<LateInEarlyOutRequest> GetByDate(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.Query<LateInEarlyOutRequest, Requester, Requester, Requester, Requester, Requester, LateInEarlyOutRequest >(
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
        }

        public LateInEarlyOutRequest GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<LateInEarlyOutRequest, Requester, Requester, Requester, Requester, LateInEarlyOutRequest>(
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
        }

        public IList<LateInEarlyOutRequest> GetHistoryById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.Query<LateInEarlyOutRequest, Requester, Requester, Requester, Requester, Requester, LateInEarlyOutRequest >(
                        (request, employee, verifier, approver, hr, modifier) =>
                        {
                            request.Requester = employee;
                            request.Verifier = verifier;
                            request.Approver = approver;
                            request.HR = hr;
                            request.Modifier = modifier;
                            return request;
                        },
                        splitOn: "EmpId, VerifierId, ApproverId, HRId, ModifierId ",
                        parameterSet: criteria);

                return entity.ToList();
            }
        }
    }
}