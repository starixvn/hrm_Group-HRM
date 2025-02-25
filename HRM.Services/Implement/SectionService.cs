namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Section;
    using HRM.Models;
    using HRM.Repositories;

    public class SectionService : ISectionService
    {
        private readonly ISectionRepository sectionRepository;
        private readonly IBaseService baseService;

        public SectionService(
            ISectionRepository sectionRepository,
            IBaseService baseService)
        {
            this.sectionRepository = sectionRepository;
            this.baseService = baseService;
        }

        public Section GetById(int id)
        {
            var criteria = new GetSectionByIdCriteria
            {
                SectionId = id
            };
            return sectionRepository.GetById(criteria);
        }

        public IList<Section> GetSection(bool? isActive = true)
        {
            var criteria = new GetSectionCriteria
            {
                IsActive = isActive
            };
            IList<Section> sections = sectionRepository.GetSection(criteria);

            return sections ?? new List<Section>();
        }

        public CommandResult Delete(int actorId, int id)
        {
            var criteria = new DeleteSectionCriteria
            {
                sectionId = id,
                ModifiedBy = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}