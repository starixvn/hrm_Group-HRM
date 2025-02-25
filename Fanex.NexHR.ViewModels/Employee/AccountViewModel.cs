namespace Fanex.NexHR.ViewModels.Employee
{
    using Fanex.NexHR.Models.Log;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class AccountViewModel 
    {
        public Employee Employee { get; set; }

        public IEnumerable<LogObjectDetail> Logs { get; set; }
    }
}
