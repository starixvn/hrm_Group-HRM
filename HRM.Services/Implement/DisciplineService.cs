using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.Discipline;
using HRM.Models.Discipline;
using HRM.Repositories.Interfaces;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class DisciplineService(
    IDisciplineRepository disciplineRepository,
    IBaseService baseService,
    IUserControlSession userControlSession) : IDisciplineService
{
    public async Task<IList<DisciplineSummaryModel>> GetDisciplineSummaryAsync(GetDisciplineSummaryCriteria criteria)
    {
        IList<DisciplineSummaryModel> disciplines = new List<DisciplineSummaryModel>();

        if (criteria != null && criteria.IsValid())
        {
            disciplines = await disciplineRepository.GetDisciplineSummaryAsync(criteria);
        }

        return disciplines;
    }

    public async Task<IList<DisciplineDetailModel>> GetDisciplineDetailAsync(GetDisciplineDetailCriteria criteria)
    {
        IList<DisciplineDetailModel> disciplines = new List<DisciplineDetailModel>();

        if (criteria != null && criteria.IsValid())
        {
            disciplines = await disciplineRepository.GetDisciplineDetailAsync(criteria);
            foreach (var discipline in disciplines)
            {
                discipline.CheckIn = discipline.CheckIn?.ToClientTimeZone() ?? discipline.CheckIn;
                discipline.CheckOut = discipline.CheckOut?.ToClientTimeZone() ?? discipline.CheckOut;
            }
        }

        return disciplines;
    }

    public async Task<IEnumerable<EmployeeInfoModel>> GetMemberToAddOtherDisciplineAsync(int supervisorId, string query = null, short? companyId = null)
    {
        var criteria = new GetMemberToAddOtherDisciplineBySupervisorIdCriteria
        {
            SupervisorId = supervisorId,
            Query = query,
            CompanyId = companyId ?? userControlSession.Get<short>(SessionKey.CurrentCompanyId)
        };
        return await baseService.GetDataAsync<EmployeeInfoModel>(criteria);
    }

    public async Task<IList<OtherDisciplineModel>> GetOtherDisciplineAsync(GetOtherDisciplineOfEmployeeCriteria criteria)
    {
        var data = await baseService.GetDataAsync<OtherDisciplineModel>(criteria);
        return data.ToList();
    }

    public async Task<IList<DisciplineTypeModel>> GetDisciplineTypeAsync(GetDisciplineTypeCriteria criteria)
    {
        var data = await baseService.GetDataAsync<DisciplineTypeModel>(criteria);
        return data.ToList();
    }

    public async Task<IList<DisciplineTypeModel>> GetOtherDisciplineTypeAsync(GetOtherDisciplineTypeCriteria criteria)
    {
        var data = await baseService.GetDataAsync<DisciplineTypeModel>(criteria);
        return data.ToList();
    }
}