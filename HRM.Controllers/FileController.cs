namespace HRM.Controllers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Constants;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class FileController(
    IUserControlSession session,
    IMinIOService minIOService) : BaseController(session)
{
    private const int MaxSizeFileUpload = 5 * 1024 * 1024;

    [HttpPost]
    public async Task<string> UploadFile(IList<IFormFile> files, int categoryId, bool isOverride)
    {
        var pathAttachFiles = string.Empty;
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var pathAttach = await SaveFileToServer(file, categoryId, isOverride);
                pathAttachFiles = string.IsNullOrEmpty(pathAttachFiles) ? pathAttach : string.Concat(pathAttachFiles, ";/", pathAttach);
            }
        }

        return pathAttachFiles;
    }

    private async Task<string> SaveFileToServer(IFormFile file, int categoryId, bool isOverride)
    {
        var fileName = Path.GetFileName(file.FileName);
        var pathAttach = $"{CurrentCompanyId}/memo/{categoryId}/{fileName}";
        if (isOverride)
        {
            var count = 1;
            while (await minIOService.IsFileExistsAsync(Bucket.File, pathAttach))
            {
                pathAttach = pathAttach.Insert(pathAttach.LastIndexOf('.'), $"({count})");
                if (pathAttach.Contains($"({count - 1})"))
                {
                    pathAttach = pathAttach.Replace($"({count - 1})", "");
                }

                count++;
            }
        }

        await minIOService.UploadFileAsync(file, Bucket.File, pathAttach);

        return pathAttach;
    }

    public async Task<bool> Delete(string fileName)
    {
        return await minIOService.DeleteFileAsync(Bucket.File, fileName);
    }

    [HttpPost]
    public async Task<IActionResult> MyProfileUploadFile(IEnumerable<IFormFile> files)
    {
        var formFiles = files.ToList();
        if (!formFiles.Exists(file => file.Length <= 0))
        {
            if (!IsValidFileUpload(formFiles))
            {
                return Json(new { isSuccessful = false, message = "The file(s) uploaded invalid" });
            }

            foreach (var file in formFiles)
            {
                var fileName = Path.GetFileName(file.FileName);

                var pathAttach = $"{CurrentCompanyId}/EmployeeProfiles/{EmployeeSession.Code}/{fileName}";

                await minIOService.UploadFileAsync(file, Bucket.File, pathAttach);
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
    public async Task<IActionResult> DeleteMyProfileUploadedFile(string fileName)
    {
        var message = string.Empty;
        var pathAttach = $"{CurrentCompanyId}/EmployeeProfiles/{EmployeeSession.Code}/{fileName}";
        var isSuccessful = await minIOService.DeleteFileAsync(Bucket.File, pathAttach);

        return Json(new { isSuccessful, message });
    }
}