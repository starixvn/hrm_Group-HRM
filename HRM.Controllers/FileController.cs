namespace HRM.Controllers;

using System.IO;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Hosting;

public class FileController(
    IWebHostEnvironment env,
    IUserControlSession userControlSession) : BaseController(userControlSession)
{
    private const int MaxSizeFileUpload = 5 * 1024 * 1024;

    //[HttpPost]
    //public string UploadFile(IList<IFormFile> files, int categoryId, bool isOverride)
    //{
    //    var pathAttachFiles = string.Empty;
    //    foreach (var file in files)
    //    {
    //        if (file.Length > 0)
    //        {
    //            var pathAttach = SaveFileToServer(file, categoryId, isOverride);

    //            pathAttachFiles = (string.IsNullOrEmpty(pathAttachFiles)) ? pathAttach : string.Concat(pathAttachFiles, ";/", pathAttach);
    //        }
    //    }

    //    return pathAttachFiles;
    //}

    //private string SaveFileToServer(IFormFile file, int categoryId, bool isOverride)
    //{
    //    var fileName = new StringBuilder(Path.GetFileName(file.FileName));
    //    var dir = "~/file/uploads/" + categoryId;
    //    var pathAttach = $"{dir}/{fileName}";

    //    if (isOverride)
    //    {
    //        while (System.IO.File.Exists(Server.MapPath(pathAttach)))
    //        {
    //            fileName.Insert(0, "(1)");
    //            pathAttach = $"{dir}/{fileName}";
    //        }
    //    }

    //    if (!Directory.Exists(Server.MapPath(dir)))
    //    {
    //        Directory.CreateDirectory(Server.MapPath(dir));
    //    }

    //    var path = Path.Combine(Server.MapPath(dir), fileName.ToString());
    //    file.SaveAs(path);
    //    const byte StartIndex = 2;

    //    return pathAttach.Substring(StartIndex);
    //}

    public bool Delete(string fileName)
    {
        var filePath = Path.Combine(env.ContentRootPath, RemoveFirstSlash(fileName));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        return true;
    }

    private static string RemoveFirstSlash(string path)
    {
        if (path.StartsWith("/"))
        {
            path = path.Substring(1);
        }

        return path;
    }

    //[HttpPost]
    //public ActionResult MyProfileUploadFile(IEnumerable<HttpPostedFileBase> files)
    //{
    //    if (!files.Any(file => file.ContentLength <= 0))
    //    {
    //        if (!IsValidFileUpload(files))
    //        {
    //            return Json(new { isSuccessful = false, message = "The file(s) uploaded invalid" });
    //        }

    //        foreach (var file in files)
    //        {
    //            var fileName = Path.GetFileName(file.FileName);
    //            string basePath = HostingEnvironment.ApplicationPhysicalPath + Constants.ProfileUploadPath + EmployeeSession.Code;
    //            if (!Directory.Exists(basePath))
    //            {
    //                Directory.CreateDirectory(basePath);
    //            }

    //            if (System.IO.File.Exists(basePath + "/" + file.FileName))
    //            {
    //                return Json(new { isSucessful = false, message = string.Format(Labels.UploadFileExists, file.FileName) });
    //            }
    //            else
    //            {
    //                var path = Path.Combine(Server.MapPath("~/file/uploads/profile/"), EmployeeSession.Code, fileName);
    //                file.SaveAs(path);
    //            }
    //        }

    //        return Json(new { isSuccessful = true, message = "File(s) uploaded sucessful." });
    //    }

    //    return Json(new { isSuccessful = false, message = "File(s) uploaded failed." });
    //}

    //private static bool IsValidFileUpload(IEnumerable<HttpPostedFileBase> files)
    //{
    //    string[] allowedExtentsions = new[] { ".jpg", ".png", ".jpeg", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };
    //    foreach (var file in files)
    //    {
    //        if (file.ContentLength > MaxSizeFileUpload)
    //        {
    //            return false;
    //        }
    //        string extentsion = Path.GetExtension(file.FileName).ToLowerInvariant();
    //        if (!allowedExtentsions.Contains(extentsion))
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}

    //[HttpPost]
    //public ActionResult DeleteMyProfileUploadedFile(string fileName)
    //{
    //    bool isSuccessful = false;
    //    string message = string.Empty;
    //    string path = HostingEnvironment.ApplicationPhysicalPath + Constants.ProfileUploadPath + EmployeeSession.Code + "/" + fileName;
    //    if (System.IO.File.Exists(path))
    //    {
    //        System.IO.File.Delete(path);
    //        isSuccessful = true;
    //    }
    //    else
    //    {
    //        message = Labels.FileDoesNotExists;
    //    }

    //    return Json(new { isSuccessful, message });
    //}
}