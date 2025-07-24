using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.Discipline;
using HRM.Models.Discipline;

namespace HRM.Services.Interfaces;

public interface IDisciplineService
{
    Task<IList<DisciplineSummaryModel>> GetDisciplineSummaryAsync(GetDisciplineSummaryCriteria criteria);

    Task<IList<DisciplineDetailModel>> GetDisciplineDetailAsync(GetDisciplineDetailCriteria criteria);

    Task<IList<OtherDisciplineModel>> GetOtherDisciplineAsync(GetOtherDisciplineOfEmployeeCriteria criteria);

    Task<IList<DisciplineTypeModel>> GetDisciplineTypeAsync(GetDisciplineTypeCriteria criteria);

    Task<IList<DisciplineTypeModel>> GetOtherDisciplineTypeAsync(GetOtherDisciplineTypeCriteria criteria);

    Task<IEnumerable<EmployeeInfoModel>> GetMemberToAddOtherDisciplineAsync(int supervisorId, string query = null, short? companyId = null);
}