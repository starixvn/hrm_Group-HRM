using System.Collections.Generic;
using System.IO;
using System.Linq;
using HRM.Core.Services;
using System.Threading.Tasks;
using HRM.Models;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using HRM.Core.Constants;
using HRM.Core.Models;
using NPOI.HPSF;

namespace HRM.Services.Implement;

public class AttachFileService(IWebHostEnvironment hostingEnvironment, IMinIOService minIOService) : IAttachFileService
{
    public void UploadFile(IList<IFormFile> files, long fileId, string pathFolder)
    {
        if (files != null && files.Any(x => x.Length > 0))
        {
            var dir = Path.Combine(hostingEnvironment.ContentRootPath, RemoveFirstSlash(pathFolder) + fileId);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(dir, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    file.CopyTo(stream);
                }
            }
        }
    }

    public async Task UploadFileAsync(IList<IFormFile> files, long requestId, string pathFolder)
    {
        if (files != null && files.Any(x => x.Length > 0))
        {
            foreach (var file in files)
            {
                var pathFile = $"{pathFolder}/{requestId}/{file.FileName}";
                await minIOService.UploadFileAsync(file, Bucket.File, pathFile);
            }
        }
    }

    public List<string> GetAttachFile(long fileId, string pathFolder)
    {
        var files = new List<string>();
        var dir = Path.Combine(hostingEnvironment.ContentRootPath, RemoveFirstSlash(pathFolder) + fileId);
        if (Directory.Exists(dir))
        {
            var filePaths = Directory.GetFiles(dir);
            foreach (var filePath in filePaths)
            {
                files.Add(Path.GetFileName(filePath));
            }
        }

        return files;
    }

    public async Task<List<string>> GetAttachFileAsync(long requestId, string pathFolder)
    {
        var pathFile = $"{pathFolder}/{requestId}";
        return (await minIOService.GetFileAsync(Bucket.File, pathFile))
               .Select(x => x.FileName)
               .ToList();
    }

    public void DeleteFile(List<string> files, long fileId, string pathFolder)
    {
        foreach (var file in files)
        {
            var dir = Path.Combine(hostingEnvironment.ContentRootPath, RemoveFirstSlash(pathFolder) + fileId + '/' + file);
            File.Delete(dir);
        }
    }

    public async Task DeleteFileAsync(List<string> files, long fileId, string pathFolder)
    {
        foreach (var file in files)
        {
            var pathFile = $"{pathFolder}/{fileId}/{file}";
            await minIOService.DeleteFileAsync(Bucket.File, pathFile);
        }
    }

    public IList<AttachFile> GetInfoAttachFile(long fileId, string pathFolder)
    {
        var attachFiles = new List<AttachFile>();
        var path = RemoveFirstSlash(pathFolder) + fileId;
        var dir = Path.Combine(hostingEnvironment.ContentRootPath, path);
        if (Directory.Exists(dir))
        {
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                attachFiles.Add(new AttachFile
                {
                    Path = $"{pathFolder + fileId}/{Path.GetFileName(file)}",
                    FileName = Path.GetFileName(file),
                    FileNameWithoutExtension = Path.GetFileNameWithoutExtension(file),
                    Extension = Path.GetExtension(file)
                });
            }
        }

        return attachFiles;
    }

    public async Task<List<S3FileObject>> GetInfoAttachFileAsync(long fileId, string pathFolder)
    {
        var pathFile = $"{pathFolder}/{fileId}";
        return (await minIOService.GetFileAsync(Bucket.File, pathFile)).ToList();
    }

    public void DeleteFolderAttachFile(long id, string pathFolder)
    {
        var dir = Path.Combine(hostingEnvironment.ContentRootPath, RemoveFirstSlash(pathFolder) + id);

        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }
    }

    private static string RemoveFirstSlash(string path)
    {
        if (path.StartsWith('/'))
        {
            path = path[1..];
        }
        return path;
    }
}