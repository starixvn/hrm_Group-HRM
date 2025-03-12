namespace HRM.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Models.Discipline;

public interface IDisciplineRepository
{
    Task<IList<DisciplineSummaryModel>> GetDisciplineSummaryAsync(ICriteria criteria);

    Task<IList<DisciplineDetailModel>> GetDisciplineDetailAsync(ICriteria criteria);
}