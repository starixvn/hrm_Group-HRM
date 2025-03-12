namespace HRM.ViewModels.Employee;

using Models.Log;
using System.Collections.Generic;
using Models;

public class AccountViewModel 
{
    public Employee Employee { get; set; }

    public IEnumerable<LogObjectDetail> Logs { get; set; }
}