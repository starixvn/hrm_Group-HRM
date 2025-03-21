using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models.FnB
{
    public class EmployeeModel
    {
        public int UserId { get; set; }

        public int EmpId { get; set; }

        public string Password { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string DomainName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int JobTitleId { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime JoinedDate { get; set; }

        public DateTime ConfirmedDate { get; set; }

        public bool IsLocal { get; set; }

        public int TeamId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string CostCenter { get; set; }

        public int JobLevelId { get; set; }

        public int CompanyId { get; set; }
    
        public int EmpCategoryID { get; set; }

        public int OldEmpId { get; set; }
    }
}
