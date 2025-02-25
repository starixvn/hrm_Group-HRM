namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public class CheckInOutRequestRepository : ICheckInOutRequestRepository
    {
        public IList<CheckInOutRequest> GetByDate(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.Query<CheckInOutRequest, Requester, Requester, Requester, Requester, Requester, CheckInOutRequest>(
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

        public CheckInOutRequest GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<CheckInOutRequest, Requester, Requester, Requester, Requester, CheckInOutRequest>(
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

        public IList<CheckInOutRequest> GetHistoryById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = db.Query<CheckInOutRequest, Requester, Requester, Requester, Requester, Requester, CheckInOutRequest>(
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