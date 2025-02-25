namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Models.Request;

    public class KeepInViewRequestRepository : IKeepInViewRequestRepository
    {
        public IList<KeepInViewRequest> GetByDate(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.Query<KeepInViewRequest, Requester, Requester, Requester, Requester, Requester, KeepInViewRequest>(
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

        public KeepInViewRequest GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<KeepInViewRequest, Requester, Requester, Requester, Requester, KeepInViewRequest>(
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

        public IList<KeepInViewRequest> GetHistoryById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.Query<KeepInViewRequest, Requester, Requester, Requester, Requester, Requester, KeepInViewRequest >(
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
    }
}