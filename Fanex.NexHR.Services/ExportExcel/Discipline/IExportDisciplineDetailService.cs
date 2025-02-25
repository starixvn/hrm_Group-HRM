namespace Fanex.NexHR.Services
{
    using Criteria.Discipline;
    using Models.Discipline;
    using System.Collections.Generic;

    public interface IExportDisciplineDetailService
    {
        void Export(GetDisciplineDetailCriteria criteria, IList<DisciplineDetailModel> disciplines);
    }
}