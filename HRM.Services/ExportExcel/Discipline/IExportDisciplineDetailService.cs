namespace HRM.Services;

using Criteria.Discipline;
using Models.Discipline;
using System.Collections.Generic;

public interface IExportDisciplineDetailService
{
    byte[] Export(GetDisciplineDetailCriteria criteria, IList<DisciplineDetailModel> disciplines);
}