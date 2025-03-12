using System.IO;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;

namespace HRM.Controllers.Export;

public class BaseExportController(IUserControlSession userControlSession) : BaseController(userControlSession)
{
    protected IActionResult ExcelFile(HSSFWorkbook workbook, string fileName)
    {
        using (var output = new MemoryStream())
        {
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}