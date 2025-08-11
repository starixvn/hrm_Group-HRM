using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fanex.Logging;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Core.XMLExtensions;
using HRM.Criteria.ScheduleArrange;
using HRM.Criteria.SystemSetting;
using HRM.Models.ScheduleArrange;
using HRM.Models.SyncData;
using Newtonsoft.Json;

namespace HRM.Services.Implement;

public interface ISyncWorkingShiftService
{
    Task<CommandResult> SyncWorkingShiftAsync(SyncWorkingShiftCommand command);

    Task<SyncWorkingShiftConfig> GetSyncWorkingShiftConfigAsync(short? companyId = null);
}

public class SyncWorkingShiftService(
    AppSettings appSettings,
    IHttpClientFactory clientFactory,
    ILogger logger,
    IBaseService baseService,
    IUserControlSession userControlSession) : ISyncWorkingShiftService
{
    private const int BatchSize = 1000;

    public async Task<CommandResult> SyncWorkingShiftAsync(SyncWorkingShiftCommand command)
    {
        var commandResult = new CommandResult { Code = 0, Message = string.Empty };
        var companyId = command.CompanyId;
        var config = await GetSyncWorkingShiftConfigAsync(companyId);
        for (var date = command.FromDate.Date; date.Date <= command.ToDate.Date; date = date.AddDays(1))
        {
            try
            {
                commandResult = await SyncWorkingShiftAsync(companyId, command, date, config);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
            }
        }

        return commandResult;
    }

    public async Task<SyncWorkingShiftConfig> GetSyncWorkingShiftConfigAsync(short? companyId = null)
    {
        companyId ??= userControlSession.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = companyId.Value,
            Key = SystemConfigKey.SyncWorkingShift
        };

        var result = await baseService.GetDataEntityAsync<string>(criteria);
        return result == null ? null : JsonConvert.DeserializeObject<SyncWorkingShiftConfig>(result);
    }

    private async Task<CommandResult> SyncWorkingShiftAsync(short companyId, SyncWorkingShiftCommand command, DateTime date, SyncWorkingShiftConfig config)
    {
        var commandResult = new CommandResult { Code = 0, Message = string.Empty };

        var url = new Uri($"{appSettings.OWPWorkingShiftAPI}?locationCode={config.LocationCode}&fromDate={date:yyyy-MM-dd}&toDate={date:yyyy-MM-dd}&timezone={config.Timezone}");
        var httpClient = clientFactory.CreateClient();
        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var rawData = await response.Content.ReadAsStringAsync();
            var workingShifts = JsonConvert.DeserializeObject<IList<OWPWorkingShift>>(rawData);
            workingShifts = workingShifts?.Where(x => !string.IsNullOrWhiteSpace(x.EmployeeID)).ToList();

            if (workingShifts == null || !workingShifts.Any())
            {
                return commandResult;
            }

            foreach (var shift in workingShifts)
            {
                ProcessData(shift);
            }

            var totalBatch = (int)Math.Ceiling((double)workingShifts.Count / BatchSize);
            for (var i = 0; i < totalBatch; i++)
            {
                var batchData = workingShifts.Skip(i * BatchSize).Take(BatchSize).ToList();
                await ImportShiftAsync(companyId, command.ActorId, batchData);

                commandResult = await baseService.ExecuteNonQueryAsync(new SyncWorkingShiftCommand
                {
                    CompanyId = companyId,
                    ActorId = command.ActorId,
                    EmployeeIds = command.EmployeeIds,
                    FromDate = date,
                    ToDate = date,
                    WorkingShiftXml = batchData.ToXML().ToString()
                });
            }
        }
        else
        {
            var message = await response.Content.ReadAsStringAsync();
            await logger.ErrorAsync($"Sync working shift - StatusCode: {response.StatusCode}\r\nMessage: {message}");
            commandResult.Code = (int)response.StatusCode;
            commandResult.Message = message;
        }

        return commandResult;
    }

    private async Task ImportShiftAsync(short companyId, int actorId, IList<OWPWorkingShift> workingShifts)
    {
        var shifts = workingShifts.Where(x => !string.IsNullOrWhiteSpace(x.ShiftCode)
                    && !string.IsNullOrWhiteSpace(x.CheckInTime)
                    && !string.IsNullOrWhiteSpace(x.CheckOutTime))
            .GroupBy(x => new OWPShift
            {
                ShiftCode = x.ShiftCode,
                CheckInTime = x.CheckInTime,
                CheckOutTime = x.CheckOutTime,
                IsInAfterDay = x.IsInAfterDay,
                IsOutAfterDay = x.IsOutAfterDay,
                IsCrossDay = x.IsCrossDay,
                TotalHours = x.TotalHours
            }).Select(x =>
            {
                return new OWPShift
                {
                    ShiftCode = x.Key.ShiftCode,
                    CheckInTime = x.Key.CheckInTime,
                    CheckOutTime = x.Key.CheckOutTime,
                    IsInAfterDay = x.Key.IsInAfterDay,
                    IsOutAfterDay = x.Key.IsOutAfterDay,
                    IsCrossDay = x.Key.IsCrossDay,
                    TotalHours = x.Key.TotalHours,
                    LastUpdated = x.Max(y => y.LastUpdated)
                };
            }).ToList();
        var command = new SyncShiftCommand
        {
            CompanyId = companyId,
            ActorId = actorId,
            ShiftXml = shifts.ToXML().ToString()
        };
        await baseService.ExecuteNonQueryAsync(command);
    }

    private static void ProcessData(OWPWorkingShift shift)
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

    private static void CheckOffDay(OWPWorkingShift shift)
    {
        if (shift.ShiftCode?.IndexOf("OFF", StringComparison.OrdinalIgnoreCase) >= 0
            && !shift.IsOffDay)
        {
            shift.IsOffDay = true;
        }
    }

    private static void CheckPublicHoliday(OWPWorkingShift shift)
    {
        if (!string.IsNullOrWhiteSpace(shift.ShiftCode)
            && shift.ShiftCode.Equals("PH", StringComparison.OrdinalIgnoreCase)
            && !shift.IsPublicHoliday)
        {
            shift.IsPublicHoliday = true;
        }
    }

    private static void GetTotalHours(OWPWorkingShift shift)
    {
        if (!shift.ShiftStartTime.HasValue || !shift.ShiftEndTime.HasValue)
        {
            return;
        }

        if (shift.ShiftStartTime.Value.Date != shift.ShiftEndTime.Value.Date)
        {
            shift.IsCrossDay = true;
        }

        const byte numberOfDigits = 2;
        shift.TotalHours = Math.Round((shift.ShiftEndTime.Value - shift.ShiftStartTime.Value).TotalHours, numberOfDigits);
    }
}