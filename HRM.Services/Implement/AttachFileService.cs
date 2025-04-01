using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HRM.Services.Implement;

public class AttachFileService(IMinIOService minIOService) : IAttachFileService
{
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

    public async Task DeleteFileAsync(List<string> files, long fileId, string pathFolder)
    {
        foreach (var file in files)
        {
            var pathFile = $"{pathFolder}/{fileId}/{file}";
            await minIOService.DeleteFileAsync(Bucket.File, pathFile);
        }
    }

    public async Task<IEnumerable<S3FileObject>> GetInfoAttachFileAsync(long fileId, string pathFolder)
    {
        var pathFile = $"{pathFolder}/{fileId}";
        return await minIOService.GetFileAsync(Bucket.File, pathFile);
    }

    public async Task DeleteFolderAttachFileAsync(long id, string pathFolder)
    {
        await minIOService.DeleteDirectoryAsync(Bucket.File, $"{pathFolder}/{id}");
    }
}