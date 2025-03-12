namespace HRM.Services;

using Criteria.Discipline;
using Models.Discipline;
using System.Collections.Generic;

public interface IExportDisciplineSummaryService
{
    byte[] Export(GetDisciplineSummaryCriteria criteria, IList<DisciplineSummaryModel> disciplines);
}