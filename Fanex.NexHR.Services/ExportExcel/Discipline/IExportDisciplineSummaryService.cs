namespace Fanex.NexHR.Services
{
    using Criteria.Discipline;
    using Models.Discipline;
    using System.Collections.Generic;

    public interface IExportDisciplineSummaryService
    {
        void Export(GetDisciplineSummaryCriteria criteria, IList<DisciplineSummaryModel> disciplines);
    }
}