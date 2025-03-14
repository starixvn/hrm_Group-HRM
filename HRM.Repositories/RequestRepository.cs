using HRM.Models.Section;
using HRM.Models.Shifts;

namespace HRM.Repositories;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
using Criteria.Request;
using Fanex.Data;
using Fanex.Data.Repository;
using Interfaces;
using Models;
using Models.Request;
using Models.Request.ChangeShift;
using Models.Supervisor;

public class RequestRepository : IRequestRepository
{
    public async Task<IList<RequestEntity>> GetRequests(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var entityList = FillData(multi.Read<dynamic>());
        var changeOffDateDetails = (await multi.ReadAsync<ChangeShiftDetail>()).ToList();

        foreach (var request in entityList.Where(entity => entity.RequestType.Id == (byte)RequestType.ChangeShift))
        {
            request.ChangeShiftDetails = changeOffDateDetails.Where(detail => detail.RequestID == request.Id);
        }

        return entityList;
    }

    public async Task<RequestRelatedPeopleModel> GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria)
    {
	    using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
	    using var multi = await db.QueryMultipleAsync(criteria);
	    var relatedPeople = new RequestRelatedPeopleModel
        {
	        Requester = multi.ReadFirstOrDefault<SupervisorNameModel>(),
	        Verifier = multi.ReadFirstOrDefault<SupervisorNameModel>(),
	        Approver = multi.ReadFirstOrDefault<SupervisorNameModel>(),
	        ListHR = multi.Read<SupervisorNameModel>().ToList(),
	        EmailSetting = multi.ReadFirstOrDefault<EmailSettingsModel>()
        };

        return relatedPeople;
    }

    public async Task<VerifierAndApproverForRequest> GetVerifierAndApproverAsync(ICriteria criteria)
    {
	    using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
	    using var multi = await db.QueryMultipleAsync(criteria);
	    var supervisor = new VerifierAndApproverForRequest
        {
	        Verifiers = multi.Read<SupervisorNameModel>().ToList(),
	        Approvers = multi.Read<SupervisorNameModel>().ToList()
        };

        return supervisor;
    }

    public async Task<RequestEntity> GetRequestToIgnoreById(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryEntityAsync<RequestEntity, Shift, Employee, RequestTypeModel, RequestEntity>(
            (request, shift, requester, type) =>
            {
                request.RequestType = type;
                request.FromShift = shift;
                request.Requester = requester;

                return request;
            },
            "ShiftId, RequesterId, RequestType",
            criteria);
    }

    public async Task<List<LeaveDisciplineModel>> GetLeaveRequestToReviewDiscipline(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var disciplines = multi.Read<LeaveDisciplineModel>().ToList();
        var leaveDetails = multi.Read<LeaveDisciplineDetailModel>().ToList();
        foreach (var discipline in disciplines)
        {
            discipline.LeaveDetail = leaveDetails.Where(t => t.WorkDate == discipline.WorkDate).ToList();
        }

        return disciplines;
    }

    private static List<RequestEntity> FillData(dynamic data)
    {
        var entityList = new List<RequestEntity>();
        foreach (var request in data)
        {
            RequestEntity entity = FillDataEntity(request);
            entityList.Add(entity);
        }

        return entityList;
    }

    [SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
    private static RequestEntity FillDataEntity(dynamic requestInfo)
    {
        var entity = new RequestEntity
        {
            Id = requestInfo.RequestId,
            RequestType = new RequestTypeModel { Id = (short)requestInfo.RequestType },
            Status = new RequestStatus { Id = requestInfo.Status },
            FromDate = requestInfo.FromDate,
            ToDate = requestInfo.ToDate,
            TotalDays = requestInfo.TotalDays,
            IsHalfDay = requestInfo.IsHalfDay ?? false,
            FromTime = requestInfo.FromTime ?? DateTimeOffset.Now.DateTime,
            ToTime = requestInfo.ToTime ?? DateTimeOffset.Now.DateTime,
            KeepInViewType = (byte)(requestInfo.KeepInViewType ?? 0),
            LeaveCountDetails = requestInfo.LeaveCountDetails ?? string.Empty,
            LeaveType = new LeaveType { Id = requestInfo.LeaveTypeId ?? 0,  Name = requestInfo.LeaveTypeName ?? string.Empty },
            Remark = requestInfo.Remark ?? string.Empty,
            TotalDaysRequestInMonth = requestInfo.TotalDaysRequestInMonth ?? 0,
            CreatedDate = requestInfo.CreatedDate,
            ReasonId = (byte)requestInfo.ReasonId,
            RefNO = requestInfo.RefNo ?? string.Empty,
            ApprovedDate = requestInfo.ApprovedDate,
            VerifiedDate = requestInfo.VerifiedDate,
            ProcessedDate = requestInfo.ProcessedDate,
            Requester = GetRequester(requestInfo),
            ModifiedBy = new Employee
            {
                Id = requestInfo.ModifiedById ?? 0,
                NickName = requestInfo.ModifiedByNickName ?? string.Empty,
                FullName = requestInfo.ModifiedByFullName ?? string.Empty
            },
            VerifiedBy = new Employee
            {
                Id = requestInfo.VerifierId ?? 0,
                NickName = requestInfo.VerifierNickName ?? string.Empty,
                FullName = requestInfo.VerifierFullName ?? string.Empty
            },
            Approver = new Employee
            {
                Id = requestInfo.ApproverId ?? 0,
                NickName = requestInfo.ApproverNickName ?? string.Empty,
                FullName = requestInfo.ApproverFullName ?? string.Empty
            },
            ApprovedBy = new Employee
            {
                Id = requestInfo.ApprovedId ?? 0,
                NickName = requestInfo.ApprovedNickName ?? string.Empty,
                FullName = requestInfo.ApprovedFullName ?? string.Empty
            },
            ProcessedBy = new Employee
            {
                Id = requestInfo.ProcessedId ?? 0,
                NickName = requestInfo.ProcessedNickName ?? string.Empty,
                FullName = requestInfo.ProcessedFullName ?? string.Empty
            },
            FromShift = new Shift
            {
                Id = requestInfo.FromShiftId ?? 0,
                Name = requestInfo.FromShiftName ?? string.Empty
            },
            RequestedDate = requestInfo.RequestedDate ?? DateTimeOffset.Now.DateTime,
            TotalMins = requestInfo.TotalMinutes ?? 0,
            ActualLateInEarlyOut = requestInfo.ActualInOutMins ?? 0,
            CrossDay = (requestInfo.FromTime == null || requestInfo.ToTime == null) ??  requestInfo.FromTime.DayOfYear > requestInfo.ToTime.DayOfYear,
            ToShift = new Shift
            {
                Id = requestInfo.ToShiftId ?? 0,
                Name = requestInfo.ToShiftName ?? string.Empty
            },
            ChangeShiftType = (byte)(requestInfo.ChangeShiftType ?? 0),
            IsWarning = requestInfo.IsWarning ?? false,
            ModifiedDate = requestInfo.ModifiedDate,
            VerifiedStatus = requestInfo.VerifiedStatus ?? 0,
            ApprovedStatus = requestInfo.ApprovedStatus ?? 0,
            ProcessedStatus = requestInfo.ProcessedStatus ?? 0,
            ReworkCount = requestInfo.ReworkCount ?? 0
        };

        return entity;
    }

    private static Employee GetRequester(dynamic requestInfo)
    {
        return new Employee
        {
            Id = requestInfo.RequesterId,
            NickName = requestInfo.RequesterNickName,
            FullName = requestInfo.RequesterFullName,
            AnnualLeaveBalance = requestInfo.AnnualLeaveBalance,
            Team = new Team
            {
                Id = requestInfo.TeamId,
                Code = requestInfo.TeamCode,
                Name = requestInfo.TeamName,
                Section = new Section
                {
                    Id = requestInfo.SectionId,
                    Code = requestInfo.SectionCode,
                    Department = new Department
                    {
                        Id = requestInfo.DepartmentId,
                        Code = requestInfo.DepartmentCode
                    }
                }
            }
        };
    }
}