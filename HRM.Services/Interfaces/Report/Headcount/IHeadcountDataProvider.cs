using System.Threading.Tasks;
using HRM.Models.Report.HeadcountReport;

namespace HRM.Services.Interfaces.Report.Headcount;

public interface IHeadcountDataProvider
{
    Task<HeadcountData> GetHeadcountDataAsync(int year);
}
