namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Discipline;

    public interface IDisciplineRepository
    {
        IList<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria);

        IList<DisciplineDetailModel> GetDisciplineDetail(ICriteria criteria);
    }
}