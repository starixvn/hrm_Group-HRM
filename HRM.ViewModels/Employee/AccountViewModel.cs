namespace HRM.ViewModels.Employee
{
    using HRM.Models.Log;
    using System.Collections.Generic;
    using HRM.Models;

    public class AccountViewModel 
    {
        public Employee Employee { get; set; }

        public IEnumerable<LogObjectDetail> Logs { get; set; }
    }
}
