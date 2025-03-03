namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Discipline;
    using Models.Discipline;

    public interface IDisciplineService
    {
        Task<IList<DisciplineSummaryModel>> GetDisciplineSummaryAsync(GetDisciplineSummaryCriteria criteria);

        Task<IList<DisciplineDetailModel>> GetDisciplineDetailAsync(GetDisciplineDetailCriteria criteria);

        Task<IList<OtherDisciplineModel>> GetOtherDisciplineAsync(GetOtherDisciplineOfEmployeeCriteria criteria);

        Task<IList<DisciplineTypeModel>> GetDisciplineTypeAsync(GetDisciplineTypeCriteria criteria);

        Task<IList<DisciplineTypeModel>> GetOtherDisciplineTypeAsync(GetOtherDisciplineTypeCriteria criteria);

        Task<IEnumerable<EmployeeInfoModel>> GetMemberToAddOtherDisciplineAsync(int supervisorId, string query = null);
    }
}