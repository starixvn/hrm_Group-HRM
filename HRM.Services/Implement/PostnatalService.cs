namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Models;
    using Core.Services;
    using Criteria.Postnatal;
    using Models;

    public class PostnatalService : IPostnatalService
    {
        private readonly IBaseService baseService;

        public PostnatalService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<EmployeePostnatal> GetAll(int year)
        {
            var criteria = new GetPostnatalCriteria { Year = year };
            return baseService.GetData<EmployeePostnatal>(criteria).ToList();
        }

        public EmployeePostnatal GetById(int id)
        {
            var criteria = new GetPostnatalByIdCriteria { PostnatalId = id };
            return baseService.GetDataEntity<EmployeePostnatal>(criteria);
        }

        public CommandResult Insert(InsertPostnatalCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdatePostnatalCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeletePostnatalCriteria { PostnatalId = id };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}