using HRM.AutoJobs.Services.Interfaces;

namespace HRM.AutoJobs.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fanex.Logging;
using Constants;
using Criteria.WorkingShift;
using Models;
using XMLExtensions;
using Newtonsoft.Json;

public class SyncWorkingShiftService(
    AppSettings appSettings,
    IHttpClientFactory clientFactory,
    IBaseService baseService,
    ILogger logger)
    : ISyncWorkingShiftService
{
    public async Task SyncWorkingShiftAsync()
    {
        const short BatchSize = 1000;
        var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime>(new GetLastModifiedDateCriteria());
        var url = new Uri($"{appSettings.SyncWorkingShiftUrl}/{lastModifiedDate:yyyy-MM-dd}/{DateTimeOffset.Now:yyyy-MM-dd}");
        var httpClient = clientFactory.CreateClient(NamedHttpClients.ProxiedClient);
        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var rawData = await response.Content.ReadAsStringAsync();
            var workingShifts = JsonConvert.DeserializeObject<IList<WorkingShift>>(rawData);
            workingShifts = workingShifts?.Where(x => !string.IsNullOrWhiteSpace(x.EmployeeID)
                                                      && (!x.LastUpdated.HasValue || x.LastUpdated.Value > lastModifiedDate)).ToList();
            if (workingShifts != null && workingShifts.Any())
            {
                foreach (var shift in workingShifts)
                {
                    ProcessData(shift);
                }

                var totalBatch = (int)Math.Ceiling((double)workingShifts.Count() / BatchSize);
                for (int i = 0; i < totalBatch; i++)
                {
                    var command = new ImportWorkingShiftCommand
                    {
                        WorkingShiftXml = workingShifts.Skip(i * BatchSize).Take(BatchSize).ToXML().ToString()
                    };
                    await baseService.ExecuteNonQueryAsync(command);
                }
            }
        }
        else
        {
            var message = await response.Content.ReadAsStringAsync();
            await logger.ErrorAsync(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));
        }
    }

    private static void ProcessData(WorkingShift shift)
    {
        shift.EmployeeID = shift.EmployeeID.Trim();
        shift.ShiftCode = shift.ShiftCode?.Trim();

        CheckOffDay(shift);
        CheckPublicHoliday(shift);

        if (shift.ShiftStartTime.HasValue)
        {
            shift.CheckInTime = shift.ShiftStartTime.Value.ToString("HH:mm");
            if (shift.ShiftStartTime.Value.Date > shift.ShiftDate.Date)
            {
                shift.IsInAfterDay = true;
            }
        }

        if (shift.ShiftEndTime.HasValue)
        {
            shift.CheckOutTime = shift.ShiftEndTime.Value.ToString("HH:mm");
            if (shift.ShiftEndTime.Value.Date > shift.ShiftDate.Date)
            {
                shift.IsOutAfterDay = true;
            }
        }

        GetTotalHours(shift);
    }

    private static void CheckOffDay(WorkingShift shift)
    {
        if (shift.ShiftCode?.IndexOf("OFF", StringComparison.OrdinalIgnoreCase) >= 0
            && !shift.IsOffDay)
        {
            shift.IsOffDay = true;
        }
    }

    private static void CheckPublicHoliday(WorkingShift shift)
    {
        if (!string.IsNullOrWhiteSpace(shift.ShiftCode)
            && shift.ShiftCode.Equals("PH", StringComparison.OrdinalIgnoreCase)
            && !shift.IsPublicHoliday)
        {
            shift.IsPublicHoliday = true;
        }
    }

    private static void GetTotalHours(WorkingShift shift)
    {
        if (shift.ShiftStartTime.HasValue && shift.ShiftEndTime.HasValue)
        {
            if (shift.ShiftStartTime.Value.Date != shift.ShiftEndTime.Value.Date)
            {
                shift.IsCrossDay = true;
            }

            const byte NumberOfDigits = 2;
            shift.TotalHours = Math.Round((shift.ShiftEndTime.Value - shift.ShiftStartTime.Value).TotalHours, NumberOfDigits);
        }
    }
}