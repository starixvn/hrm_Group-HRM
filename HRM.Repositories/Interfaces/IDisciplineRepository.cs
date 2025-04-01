using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models.Discipline;

namespace HRM.Repositories.Interfaces;

public interface IDisciplineRepository
{
    Task<IList<DisciplineSummaryModel>> GetDisciplineSummaryAsync(ICriteria criteria);

    Task<IList<DisciplineDetailModel>> GetDisciplineDetailAsync(ICriteria criteria);
}