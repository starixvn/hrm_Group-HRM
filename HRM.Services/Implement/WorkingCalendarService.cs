namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Criteria.Calendar;
    using HRM.Models.Report;
    using HRM.Models.ScheduleArrange;
    using HRM.Repositories;

    public class WorkingCalendarService : IWorkingCalendarService
    {
        private readonly IWorkingCalendarRepository workingCalendarRepository;

        public WorkingCalendarService(IWorkingCalendarRepository workingCalendarRepository)
        {
            this.workingCalendarRepository = workingCalendarRepository;
        }

        public IEnumerable<DailySchedule> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria)
        {
            return workingCalendarRepository.GetWorkingSchedule(criteria);
        }

        public IEnumerable<WorkingCalendar> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria)
        {
            return workingCalendarRepository.GetWorkingCalendars(criteria);
        }
    }
}