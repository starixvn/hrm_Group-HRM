namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria.Discipline;
    using HRM.Repositories;
    using Models.Discipline;

    public class DisciplineService : IDisciplineService
    {
        private readonly IDisciplineRepository disciplineRepository;
        private readonly IBaseService baseService;

        public DisciplineService(
            IDisciplineRepository disciplineRepository,
            IBaseService baseService)
        {
            this.disciplineRepository = disciplineRepository;
            this.baseService = baseService;
        }

        public IList<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            IList<DisciplineSummaryModel> disciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                disciplines = disciplineRepository.GetDisciplineSummary(criteria);
            }

            return disciplines;
        }

        public IList<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria)
        {
            IList<DisciplineDetailModel> disciplines = new List<DisciplineDetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                disciplines = disciplineRepository.GetDisciplineDetail(criteria);
            }

            return disciplines;
        }

        public IEnumerable<EmployeeInfoModel> GetMemberToAddOtherDiscipline(int supervisorId, string query = null)
        {
            var criteria = new GetMemberToAddOtherDiscipineBySupervisorIdCriteria
            {
                SupervisorId = supervisorId,
                Query = query
            };
            return baseService.GetData<EmployeeInfoModel>(criteria);
        }

        public IList<OtherDisciplineModel> GetOtherDiscipline(GetOtherDisciplineOfEmployeeCriteria criteria)
        {
            return baseService.GetData<OtherDisciplineModel>(criteria).ToList();
        }

        public IList<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria)
        {
            return baseService.GetData<DisciplineTypeModel>(criteria).ToList();
        }

        public IList<DisciplineTypeModel> GetOtherDisciplineType(GetOtherDisciplineTypeCriteria criteria)
        {
            return baseService.GetData<DisciplineTypeModel>(criteria).ToList();
        }
    }
}