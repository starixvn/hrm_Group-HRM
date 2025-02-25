using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System.IO;
    using GO.UserControl.Client.Core.Session;
    using NPOI.HSSF.UserModel;

    public class BaseExportController : BaseController
    {
        public BaseExportController(IUserControlSession userControlSession) : base(userControlSession)
        {
        }

        protected ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            using (var output = new MemoryStream())
            {
                hssfworkbook.Write(output);

                return File(output.ToArray(), "application/vnd.ms-excel", fileName);
            }
        }
    }
}