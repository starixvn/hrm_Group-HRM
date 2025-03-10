namespace HRM.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Services;
using Criteria.Discipline;
using HRM.Repositories;
using Models.Discipline;

public class DisciplineService(
    IDisciplineRepository disciplineRepository,
    IBaseService baseService) : IDisciplineService
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
        }

        return disciplines;
    }

    public async Task<IEnumerable<EmployeeInfoModel>> GetMemberToAddOtherDisciplineAsync(int supervisorId, string query = null)
    {
        var criteria = new GetMemberToAddOtherDiscipineBySupervisorIdCriteria
        {
            SupervisorId = supervisorId,
            Query = query
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