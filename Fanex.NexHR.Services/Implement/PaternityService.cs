namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Models;
    using Core.Services;
    using Criteria.Paternity;
    using Models;

    public class PaternityService : IPaternityService
    {
        private readonly IBaseService baseService;

        public PaternityService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeletePaternityCriteria { PaternityId = id };

            return baseService.ExecuteNonQuery(criteria);
        }

        public IList<EmployeePaternity> GetAll(int year)
        {
            var criteria = new GetPaternityCriteria { Year = year };
            IList<EmployeePaternity> paternities = null;
            if (criteria.IsValid())
            {
                paternities = baseService.GetData<EmployeePaternity>(criteria).ToList();
            }

            return paternities ?? new List<EmployeePaternity>();
        }

        public EmployeePaternity GetById(int id)
        {
            var criteria = new GetPaternityByIdCriteria { PaternityId = id };

            EmployeePaternity postnatal = null;
            if (criteria.IsValid())
            {
                postnatal = baseService.GetDataEntity<EmployeePaternity>(criteria);
            }
            return postnatal ?? new EmployeePaternity();
        }

        public CommandResult Insert(InsertPaternityCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdatePaternityCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}