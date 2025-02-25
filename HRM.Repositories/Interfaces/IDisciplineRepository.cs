namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.Discipline;

    public interface IDisciplineRepository
    {
        IList<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria);

        IList<DisciplineDetailModel> GetDisciplineDetail(ICriteria criteria);
    }
}