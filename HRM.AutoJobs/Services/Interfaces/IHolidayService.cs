using System.Threading.Tasks;

namespace HRM.AutoJobs.Services.Interfaces;

public interface IHolidayService
{
    Task InsertHolidayInYearAsync();
}