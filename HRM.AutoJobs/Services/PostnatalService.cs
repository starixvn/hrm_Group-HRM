namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;
    using HRM.AutoJobs.Criteria.Postnatal;

    public class PostnatalService : IPostnatalService
    {
        private readonly IBaseService baseService;

        public PostnatalService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task UpdateWorkingHourPerDayAsync()
        {
            var criteria = new UpdateWorkingHourPerDayCommand();
            await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}