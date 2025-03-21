using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models.FnB
{
    public class UnitModel
    {
        public int UnitId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int CompanyId { get; set; }
    }
}
