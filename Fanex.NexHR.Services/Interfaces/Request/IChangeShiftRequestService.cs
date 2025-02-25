namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IChangeShiftRequestService
    {
        int GetNumberOfRequest(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DailySchedule> GetWorkingCalendarForRequest(GetWorkingCalendarForRequestParam param);

        IEnumerable<DateTime> GetDisableDateForMoveOffDayRequest(int employeeId, DateTime requestDate, long? requestId = null);

        IEnumerable<ChangeShiftRequest> GetByEmployee(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<ChangeShiftRequest> GetHistoryById(long requestId);

        ChangeShiftRequest GetById(long requestId);

        IEnumerable<ChangeShiftDetail> GetDetailById(long requestId);

        CommandResult Create(int actorId, CreateChangeShift model);

        CommandResult Edit(int actorId, EditChangeShift model);

        CommandResult Delete(int actorId, long requestId);

        void SendMailAndSendLetstalkCreateRequest(long requestId);

        void SendMailEditRequest(long requestId);

        void SendMailDeleteRequest(long requestId);

        void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}