namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using HRM.Core;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Core.XMLExtensions;
    using HRM.Criteria.ScheduleArrange;
    using HRM.Models.SyncData;
    using Newtonsoft.Json;

    public interface ISyncWorkingShiftService
    {
        Task<CommandResult> SyncWorkingShiftAsync(SyncWorkingShiftCommand command);
    }

    public class SyncWorkingShiftService : ISyncWorkingShiftService, IDisposable
    {
        private bool isDisposed;
        private const int BatchSize = 1000;
        private readonly IBaseService baseService;
        private readonly HttpClient _httpClient;

        public SyncWorkingShiftService(IBaseService baseService)
        {
            this.baseService = baseService;

            if (ConfigSettings.IsEnableProxy)
            {
                var proxy = new WebProxy(ConfigSettings.Proxy_Address, ConfigSettings.Proxy_BypassOnLocal);
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy
                };
                _httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: true);
            }
            else
            {
                _httpClient = new HttpClient();
            }
        }

        public async Task<CommandResult> SyncWorkingShiftAsync(SyncWorkingShiftCommand command)
        {
            var commandResult = new CommandResult { Code = 0, Message = string.Empty };
            for (var date = command.FromDate.Date; date.Date <= command.ToDate.Date; date = date.AddDays(1))
            {
                try
                {
                    commandResult = await SyncWorkingShiftAsync(command, date);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message, ex);
                }
            }

            return commandResult;
        }

        private async Task<CommandResult> SyncWorkingShiftAsync(SyncWorkingShiftCommand command, DateTime date)
        {
            var commandResult = new CommandResult { Code = 0, Message = string.Empty };

            var builder = new StringBuilder(ConfigSettings.SyncWorkingShift_Url);
            builder.AppendFormat("?locationCode={0}&fromDate={1}&toDate={2}&timezone={3}",
                ConfigSettings.SyncWorkingShift_LocationCode,
                date.ToString("yyyy-MM-dd"),
                date.ToString("yyyy-MM-dd"),
                ConfigSettings.SyncWorkingShift_Timezone);

            var url = new Uri(builder.ToString());
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var rawData = await response.Content.ReadAsStringAsync();
                var workingShifts = JsonConvert.DeserializeObject<IList<WorkingShiftSyncModel>>(rawData);
                workingShifts = workingShifts?.Where(x => !string.IsNullOrWhiteSpace(x.EmployeeID)).ToList();
                if (workingShifts != null && workingShifts.Any())
                {
                    foreach (var shift in workingShifts)
                    {
                        ProcessData(shift);
                    }

                    int totalBatch = (int)Math.Ceiling((double)workingShifts.Count() / BatchSize);
                    for (int i = 0; i < totalBatch; i++)
                    {
                        commandResult = baseService.ExecuteNonQuery(new SyncWorkingShiftCommand
                        {
                            ActorId = command.ActorId,
                            EmployeeIds = command.EmployeeIds,
                            FromDate = date,
                            ToDate = date,
                            WorkingShiftXml = workingShifts.Skip(i * BatchSize).Take(BatchSize).ToXML().ToString()
                        });
                    }
                }
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                LogService.Error(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));
                commandResult.Code = (int)response.StatusCode;
                commandResult.Message = message;
            }

            return commandResult;
        }

        private static void ProcessData(WorkingShiftSyncModel shift)
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

        private static void CheckOffDay(WorkingShiftSyncModel shift)
        {
            if (shift.ShiftCode?.IndexOf("OFF", StringComparison.OrdinalIgnoreCase) >= 0
                && !shift.IsOffDay)
            {
                shift.IsOffDay = true;
            }
        }

        private static void CheckPublicHoliday(WorkingShiftSyncModel shift)
        {
            if (!string.IsNullOrWhiteSpace(shift.ShiftCode)
                && shift.ShiftCode.Equals("PH", StringComparison.OrdinalIgnoreCase)
                && !shift.IsPublicHoliday)
            {
                shift.IsPublicHoliday = true;
            }
        }

        private static void GetTotalHours(WorkingShiftSyncModel shift)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _httpClient.Dispose();
            }

            isDisposed = true;
        }

        ~SyncWorkingShiftService()
        {
            Dispose(false);
        }
    }
}