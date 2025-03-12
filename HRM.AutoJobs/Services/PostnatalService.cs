namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;
    using Criteria.Postnatal;

    public class PostnatalService(IBaseService baseService) : IPostnatalService
    {
        public async Task UpdateWorkingHourPerDayAsync()
        {
            var criteria = new UpdateWorkingHourPerDayCommand();
            await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}