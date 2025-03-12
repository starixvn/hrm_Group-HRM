namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;
    using Criteria;

    public class TimeRecordService : ITimeRecordService
    {
        private readonly IBaseService baseService;

        public TimeRecordService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task ImportTimeRecordAsync()
        {
            var command = new ImportTimeRecordCommand();
            await baseService.ExecuteNonQueryAsync(command);
        }
    }
}