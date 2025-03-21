using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models.FnB
{
    public class SectionModel
    {
        public int SectionId{ get; set; }

        public int CompanyId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string DepartmentId { get; set; }

        public string IsActive { get; set; }

        public string ModifiedDate { get; set; }

        public string IsDeleted { get; set; }
    }
}
