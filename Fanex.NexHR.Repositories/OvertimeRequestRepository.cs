namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;

    public class OvertimeRequestRepository : IOvertimeRequestRepository
    {
        public IEnumerable<MonthlyOvertimeRequest> GetMonthlyRequestByEmployeeId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, MonthlyOvertimeRequest>(
                    (re, stt, verifier, approver) =>
                    {
                        re.Status = stt;
                        re.Verifier = verifier;
                        re.Approver = approver;
                        return re;
                    },
                    "StatusId, VerifierId, ApproverId",
                    criteria);
            }
        }

        public MonthlyOvertimeRequest GetMonthlyRequestDetailsByEmployeeId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var monthlyRequest = multi.Read<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, Requester, MonthlyOvertimeRequest>(
                    (re, stt, requester, verifier, approver) =>
                    {
                        re.Requester = requester;
                        re.Status = stt;
                        re.Verifier = verifier;
                        re.Approver = approver;
                        return re;
                    },
                    "StatusId, EmpId, VerifierId, ApproverId").FirstOrDefault();

                    if (monthlyRequest != null)
                    {
                        monthlyRequest.DailyOvertimes = multi.Read<DailyOvertimeRequest>();
                    }

                    return monthlyRequest;
                }
            }
        }

        public IEnumerable<MonthlyOvertimeRequest> GetMemberMonthlyRequest(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, Requester, MonthlyOvertimeRequest>(
                    (req, stt, requester, verifier, approver) =>
                    {
                        req.Status = stt;
                        req.Requester = requester;
                        req.Verifier = verifier;
                        req.Approver = approver;
                        return req;
                    },
                    "StatusId, EmpId, VerifierId, ApproverId",
                    criteria);
            }
        }
    }
}