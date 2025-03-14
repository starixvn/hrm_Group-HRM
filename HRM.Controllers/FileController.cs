namespace HRM.Controllers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using GO.UserControl.Client.Core.Session;
using Core.Constants;
using Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class FileController(
    IWebHostEnvironment env,
    IUserControlSession userControlSession,
    IWebHostEnvironment hostingEnvironment) : BaseController(userControlSession)
{
    private const int MaxSizeFileUpload = 5 * 1024 * 1024;

    [HttpPost]
    public string UploadFile(IList<IFormFile> files, int categoryId, bool isOverride)
    {
        var pathAttachFiles = string.Empty;
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var pathAttach = SaveFileToServer(file, categoryId, isOverride);
                pathAttachFiles = (string.IsNullOrEmpty(pathAttachFiles)) ? pathAttach : string.Concat(pathAttachFiles, ";/", pathAttach);
            }
        }

        return pathAttachFiles;
    }

    private string SaveFileToServer(IFormFile file, int categoryId, bool isOverride)
    {
        var fileName = Path.GetFileName(file.FileName);
        var dir = "file/uploads/" + categoryId;
        var pathAttach = $"{dir}/{fileName}";
        var basePath = Path.Combine(hostingEnvironment.ContentRootPath, dir);
        if (isOverride)
        {
            while (System.IO.File.Exists(basePath + "/" + fileName))
            {
                fileName.Insert(0, "(1)");
                pathAttach = $"{basePath}/{fileName}";
            }
        }

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        var path = Path.Combine(basePath, fileName);
        using var stream = new FileStream(path, FileMode.Create);
        file.CopyTo(stream);

        return pathAttach;
    }

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
        if (path.StartsWith('/'))
        {
            path = path.Substring(1);
        }

        return path;
    }

    [HttpPost]
    public ActionResult MyProfileUploadFile(IEnumerable<IFormFile> files)
    {
        if (!files.Any(file => file.Length <= 0))
        {
            if (!IsValidFileUpload(files))
            {
                return Json(new { isSuccessful = false, message = "The file(s) uploaded invalid" });
            }

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file.FileName);
                var basePath = Path.Combine(hostingEnvironment.ContentRootPath, Constants.ProfileUploadPath + EmployeeSession.Code);
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                if (System.IO.File.Exists(basePath + "/" + file.FileName))
                {
                    return Json(new { isSucessful = false, message = string.Format(Labels.UploadFileExists, file.FileName) });
                }
                else
                {
                    var path = Path.Combine(basePath, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    file.CopyTo(stream);
                }
            }

            return Json(new { isSuccessful = true, message = "File(s) uploaded successful." });
        }

        return Json(new { isSuccessful = false, message = "File(s) uploaded failed." });
    }

    private static bool IsValidFileUpload(IEnumerable<IFormFile> files)
    {
        var allowedExtenstion = new[] { ".jpg", ".png", ".jpeg", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };
        foreach (var file in files)
        {
            if (file.Length > MaxSizeFileUpload)
            {
                return false;
            }
            var extenstion = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtenstion.Contains(extenstion))
            {
                return false;
            }
        }

        return true;
    }

    [HttpPost]
    public ActionResult DeleteMyProfileUploadedFile(string fileName)
    {
        var isSuccessful = false;
        var message = string.Empty;
        var path = Path.Combine(hostingEnvironment.ContentRootPath, Constants.ProfileUploadPath + EmployeeSession.Code + "/" + fileName);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            isSuccessful = true;
        }
        else
        {
            message = Labels.FileDoesNotExists;
        }

        return Json(new { isSuccessful, message });
    }
}
