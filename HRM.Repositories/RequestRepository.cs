using HRM.Core.Constants;
using HRM.Core.Models;

namespace HRM.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
using Criteria.Request;
using Fanex.Data;
using Fanex.Data.Repository;
using HRM.Core;
using Interfaces;
using Models;
using Models.Request;
using Models.Request.ChangeShift;
using Models.Shifts;
using Models.Supervisor;

public class RequestRepository : IRequestRepository
{
    public async Task<IList<RequestEntity>> GetRequestAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var entityList = FillData(multi.Read<dynamic>());
        var changeOffDateDetails = await multi.ReadAsync<ChangeShiftDetail>();

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

    public IEnumerable<RequestEntity> MappingNexRequestToGroupRequest(List<NexRequestModel> requests, EmployeeSession sessionUser)
    {
        List<RequestEntity> responseRequests = [];
        foreach (var requestInfo in requests)
        {
            var toDate = requestInfo.ToDate != DateTime.MinValue
                ? requestInfo.ToDate.ConvertToClientTimeZone(Constants.NexTimezone)
                : requestInfo.ToDate;
            var entity = new RequestEntity
            {
                Id = requestInfo.Id,
                RequestType = new RequestTypeModel { Id = (short)requestInfo.RequestType },
                Status = new RequestStatus { Id = requestInfo.Status },
                FromDate = requestInfo.FromDate.ConvertToClientTimeZone(Constants.NexTimezone),
                ToDate = toDate,
                TotalDays = requestInfo.TotalDays ?? 0,
                IsHalfDay = requestInfo.IsHalfDay,
                FromTime = requestInfo.FromDate.ConvertToClientTimeZone(Constants.NexTimezone),
                ToTime = toDate,
                LeaveType = new LeaveType
                {
                    Id = requestInfo.LeaveTypeID,
                    Name = requestInfo.LeaveTypeName ?? string.Empty
                },
                Remark = requestInfo.Remark,
                CreatedDate = requestInfo.CreatedDate.ToServerTimeZone().ConvertToClientTimeZone(Constants.NexTimezone),
                TotalMins = requestInfo.TotalMins ?? 0,
                CrossDay = requestInfo.IsCrossDate,
                ModifiedDate = requestInfo.ModifiedDate?.ConvertToClientTimeZone(Constants.NexTimezone),
                ApproverId = sessionUser.EmpId,
                ApproverNickName =  requestInfo.ProcessedBy,
                ApproverFullName = requestInfo.FullNameProcessedBy,
                ProcessedByFullName = sessionUser.FullName,
                ProcessedByNickName = sessionUser.Nickname,
                ApprovedDate = requestInfo.ProcessedDate?.ConvertToClientTimeZone(Constants.NexTimezone),
                CompanyId = requestInfo.CompanyId,
                RequestedDate = requestInfo.RequestedDate.ToServerTimeZone().ConvertToClientTimeZone(Constants.NexTimezone),
                Requester = new Employee
                {
                    Id = requestInfo.EmpId,
                    NickName = requestInfo.Requester,
                    FullName = requestInfo.FullNameRequester,
                    UnitCode = requestInfo.UnitCode,
                    Team = new Team
                    {
                        Name = requestInfo.Team,
                        Department = new Department
                        {
                            Name = requestInfo.DepartmentName
                        }
                    }
                }
            };
            responseRequests.Add(entity);
        }

        return responseRequests;
    }

    public int? GetNexRequestStatus(string requestStatusId)
    {
        const int nexPending = 1;
        const int nexApproved = 2;
        const int nexRejected = 3;
        const string groupHrmPending = "1";
        const string groupHrmApproved = "4";
        const string groupHrmRejected = "5";
        if (string.IsNullOrEmpty(requestStatusId))
        {
            return null;
        }

        if (requestStatusId.Contains(groupHrmPending))
        {
            return nexPending;
        }

        return requestStatusId switch
        {
            groupHrmApproved => nexApproved,
            groupHrmRejected => nexRejected,
            _ => int.Parse(requestStatusId)
        };
    }

    public int GetGroupByNexRequestStatus(int requestStatusId)
    {
        const int nexPending = 1;
        const int nexApproved = 2;
        const int nexRejected = 3;
        const int groupHrmPending = 1;
        const int groupHrmApproved = 4;
        const int groupHrmRejected = 5;

        return requestStatusId switch
        {
            nexPending => groupHrmPending,
            nexApproved => groupHrmApproved,
            nexRejected => groupHrmRejected,
            _ => requestStatusId
        };
    }

    public string GetNexRequestType(int requestTypeId)
    {
        const int groupAll = 0;
        const int groupCheckInOut = 1;
        const int groupLateInOut = 2;
        const int groupChangeShift = 3;
        const int groupLeave = 4;
        const string nexCheckInOut = "1,2";
        const string nexLateInOut = "3,4";
        const string nexChangeShift = "5";
        const string nexLeave = "6";

        return requestTypeId switch
        {
            groupAll => "",
            groupCheckInOut => nexCheckInOut,
            groupLateInOut => nexLateInOut,
            groupChangeShift => nexChangeShift,
            groupLeave => nexLeave,
            _ => requestTypeId.ToString()
        };
    }

    public int GetNexLeaveType(int leaveTypeId)
    {
        const int groupAnnualLeave = 1;
        const int groupWeddingLeave = 6;
        const int groupBusinessTrip = 9;
        const int groupMaternityLeave = 12;
        const int groupPaternityLeave = 14;
        const int groupBereavementLeave = 21;
        const int groupUnpaidLeave = 25;
        const int groupCompanyEvent = 38;

        const int nexAnnualLeave = 2;
        const int nexWeddingLeave = 6;
        const int nexBusinessTrip = 9;
        const int nexMaternityLeave = 13;
        const int nexPaternityLeave = 27;
        const int nexBereavementLeave = 22;
        const int nexUnpaidLeave = 26;
        const int nexCompanyEvent = 33;

        return leaveTypeId switch
        {
            groupAnnualLeave => nexAnnualLeave,
            groupWeddingLeave => nexWeddingLeave,
            groupBusinessTrip => nexBusinessTrip,
            groupMaternityLeave => nexMaternityLeave,
            groupPaternityLeave => nexPaternityLeave,
            groupBereavementLeave => nexBereavementLeave,
            groupUnpaidLeave => nexUnpaidLeave,
            groupCompanyEvent => nexCompanyEvent,
            _ => leaveTypeId
        };
    }

    private static List<RequestEntity> FillData(dynamic data)
    {
        var requests = new List<RequestEntity>();
        foreach (var request in data)
        {
            var entity = FillDataEntity(request);
            requests.Add(entity);
        }

        return requests;
    }

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
            IsHalfDay = GetBooleanValue(requestInfo.IsHalfDay),
            FromTime = GetDateValue(requestInfo.FromTime),
            ToTime = GetDateValue(requestInfo.ToTime),
            KeepInViewType = GetKeepInViewType(requestInfo.KeepInViewType),
            LeaveCountDetails = GetStringValue(requestInfo.LeaveCountDetails),
            LeaveType = GetLeaveType(requestInfo),
            Remark = GetStringValue(requestInfo.Remark),
            TotalDaysRequestInMonth = requestInfo.TotalDaysRequestInMonth ?? 0,
            CreatedDate = requestInfo.CreatedDate,
            ReasonId = (byte)requestInfo.ReasonId,
            RefNO = GetStringValue(requestInfo.RefNo),
            Requester = GetRequester(requestInfo),
            VerifierId = requestInfo.VerifierId ?? 0,
            VerifierNickName = requestInfo.VerifierNickName,
            VerifierFullName = requestInfo.VerifierFullName,
            VerifiedDate = requestInfo.VerifiedDate,
            ApproverId = requestInfo.ApproverId ?? 0,
            ApproverNickName = requestInfo.ApproverNickName,
            ApproverFullName = requestInfo.ApproverFullName,
            ApprovedDate = requestInfo.ApprovedDate,
            ProcessedByNickName = requestInfo.ProcessedByNickName,
            ProcessedByFullName = requestInfo.ProcessedByFullName,
            ProcessedDate = requestInfo.ProcessedDate,
            FromShift = GetFromShift(requestInfo),
            RequestedDate = GetDateValue(requestInfo.RequestedDate),
            TotalMins = requestInfo.TotalMinutes ?? 0,
            ActualLateInEarlyOut = requestInfo.ActualInOutMins ?? 0,
            CrossDay = IsCrossDay(requestInfo),
            ToShift = GetToShift(requestInfo),
            ChangeShiftType = (byte)(requestInfo.ChangeShiftType ?? 0),
            IsWarning = GetBooleanValue(requestInfo.IsWarning),
            ModifiedDate = requestInfo.ModifiedDate,
            VerifiedStatus = GetStatus(requestInfo.VerifiedStatus),
            ApprovedStatus = GetStatus(requestInfo.ApprovedStatus),
            ProcessedStatus = GetStatus(requestInfo.ProcessedStatus),
            ReworkCount = requestInfo.ReworkCount ?? 0,
            CompanyId = requestInfo.CompanyId
        };

        return entity;
    }

    private static byte GetKeepInViewType(dynamic keepInViewType) => (byte)(keepInViewType ?? 0);

    private static bool IsCrossDay(dynamic requestInfo)
        => (requestInfo.FromTime == null || requestInfo.ToTime == null) ?? requestInfo.FromTime.DayOfYear > requestInfo.ToTime.DayOfYear;

    private static int GetStatus(dynamic value) => value ?? 0;

    private static LeaveType GetLeaveType(dynamic requestInfo)
        => new() { Id = requestInfo.LeaveTypeId ?? 0, Name = requestInfo.LeaveTypeName ?? string.Empty };

    private static Shift GetFromShift(dynamic requestInfo)
        => new()
        {
            Id = requestInfo.FromShiftId ?? 0,
            Name = requestInfo.FromShiftName ?? string.Empty
        };

    private static Shift GetToShift(dynamic requestInfo)
        => new()
        {
            Id = requestInfo.ToShiftId ?? 0,
            Name = requestInfo.ToShiftName ?? string.Empty
        };

    private static bool GetBooleanValue(dynamic value) => value ?? false;

    private static DateTime GetDateValue(dynamic value) => value ?? DateTimeOffset.Now.DateTime;

    private static string GetStringValue(dynamic value) => value ?? string.Empty;

    private static Employee GetRequester(dynamic requestInfo)
    {
        return new Employee
        {
            Id = requestInfo.RequesterId,
            NickName = requestInfo.RequesterNickName,
            FullName = requestInfo.RequesterFullName,
            AnnualLeaveBalance = requestInfo.AnnualLeaveBalance,
            UnitCode = requestInfo.UnitCode,
            Team = new Team
            {
                Id = requestInfo.TeamId,
                Code = requestInfo.TeamCode,
                Name = requestInfo.TeamName,
                Department = new Department
                {
                    Id = requestInfo.DepartmentId,
                    Code = requestInfo.DepartmentCode,
                    Name = requestInfo.DepartmentName
                }
            }
        };
    }
}