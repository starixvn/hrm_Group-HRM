namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
    using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Models.Overtime;
using Models.Request;
using System.Threading.Tasks;
using HRM.Repositories.Interfaces;

public class OvertimeRequestRepository : IOvertimeRequestRepository
{
        public async Task<IEnumerable<MonthlyOvertimeRequest>> GetMonthlyRequestByEmployeeIdAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryAsync<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, MonthlyOvertimeRequest>(
                var entity = await db.QueryAsync<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, MonthlyOvertimeRequest>(
            (re, stt, verifier, approver) =>
            {
                re.Status = stt;
                re.Verifier = verifier;
                re.Approver = approver;
                return re;
            },
            "StatusId, VerifierId, ApproverId",
            criteria);

                return entity;
    }

        public async Task<MonthlyOvertimeRequest> GetMonthlyRequestDetailsByEmployeeIdAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
                using (var multi = await db.QueryMultipleAsync(criteria))
                    var monthlyRequest = (multi.Read<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, Requester, MonthlyOvertimeRequest>(
            (re, stt, requester, verifier, approver) =>
            {
                re.Requester = requester;
                re.Status = stt;
                re.Verifier = verifier;
                re.Approver = approver;
                return re;
            },
                    "StatusId, EmpId, VerifierId, ApproverId")).FirstOrDefault();

        if (monthlyRequest != null)
        {
            monthlyRequest.DailyOvertimes = await multi.ReadAsync<DailyOvertimeRequest>();
        }

        return monthlyRequest;
    }

        public async Task<IEnumerable<MonthlyOvertimeRequest>> GetMemberMonthlyRequestAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryAsync<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, Requester, MonthlyOvertimeRequest>(
                var entity = await db.QueryAsync<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, Requester, MonthlyOvertimeRequest>(
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

                return entity;
    }
}