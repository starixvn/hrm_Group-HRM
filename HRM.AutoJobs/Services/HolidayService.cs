namespace HRM.AutoJobs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.AutoJobs.Criteria;
    using HRM.AutoJobs.Models;
    using HRM.AutoJobs.XMLExtensions;

    public class HolidayService : IHolidayService
    {
        private readonly IBaseService baseService;
        private readonly ChineseLunisolarCalendar chinaCalendar = new ChineseLunisolarCalendar();

        public HolidayService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task InsertHolidayInYearAsync()
        {
            var year = DateTimeOffset.Now.Year + 1;
            var holidays = new List<Holiday>();
            holidays.AddRange(SolarHolidays(year));
            holidays.AddRange(LunarHolidays(year));
            holidays = holidays.OrderBy(h => h.FromDate).ToList();
            var command = new InsertHolidayInYearCommand
            {
                Holidays = holidays.ToXML().ToString()
            };
            await baseService.ExecuteNonQueryAsync(command);
        }

        private DateTime ConvertLunarToSolarCalendar(int year, int month, int day)
        {
            var solarDate = chinaCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return solarDate;
        }

        private static IEnumerable<Holiday> SolarHolidays(int year)
        {
            var solarHolidays = new List<Holiday>();
            var newYear = new Holiday
            {
                HolidayName = $"New Year Day {year}",
                FromDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime
            };

            var vitoryDay = new Holiday
            {
                HolidayName = "Victory Day",
                FromDate = new DateTimeOffset(year, 4, 30, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, 4, 30, 0, 0, 0, TimeSpan.Zero).DateTime
            };

            var labourDay = new Holiday
            {
                HolidayName = "Labour Day",
                FromDate = new DateTimeOffset(year, 5, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, 5, 1, 0, 0, 0, TimeSpan.Zero).DateTime
            };

            var independenceDay = new Holiday
            {
                HolidayName = "Independence Day",
                FromDate = new DateTimeOffset(year, 9, 2, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, 9, 2, 0, 0, 0, TimeSpan.Zero).DateTime
            };

            solarHolidays.Add(newYear);
            solarHolidays.Add(vitoryDay);
            solarHolidays.Add(labourDay);
            solarHolidays.Add(independenceDay);

            return solarHolidays;
        }

        private IEnumerable<Holiday> LunarHolidays(int year)
        {
            var lunarHolidays = new List<Holiday>();
            var newYear1stOnSolarDay = ConvertLunarToSolarCalendar(year, 1, 1);
            const byte TotalOffDays = 5; // holiday from 30 Dec to 4 Jan
            var holiday = new Holiday
            {
                HolidayName = $"Lunar New Year {year}",
                FromDate = newYear1stOnSolarDay.AddDays(-1),
                ToDate = newYear1stOnSolarDay.AddDays(TotalOffDays - 1)
            };
            lunarHolidays.Add(holiday);

            var kingAnniSolar = ConvertLunarToSolarCalendar(year, 3, 10);
            var kingAnniHoliday = new Holiday
            {
                HolidayName = "King Hung's Anniversary",
                FromDate = kingAnniSolar,
                ToDate = kingAnniSolar
            };
            lunarHolidays.Add(kingAnniHoliday);
            return lunarHolidays;
        }
    }
}