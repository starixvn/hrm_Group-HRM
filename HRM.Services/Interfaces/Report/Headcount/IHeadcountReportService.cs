using System.Threading.Tasks;
using HRM.Models.Report.HeadcountReport;

namespace HRM.Services.Interfaces.Report.Headcount
{
    public interface IHeadcountReportService
    {
        Task<HeadcountReportModel> GetReportAsync(int year);
    }
}