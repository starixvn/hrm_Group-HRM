namespace HRM.AutoJobs.Services;

using System.Threading.Tasks;

public interface IHolidayService
{
    Task InsertHolidayInYearAsync();
}