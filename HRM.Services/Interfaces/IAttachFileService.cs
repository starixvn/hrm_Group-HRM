using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using Microsoft.AspNetCore.Http;

namespace HRM.Services.Interfaces;

public interface IAttachFileService
{
    Task UploadFileAsync(IList<IFormFile> files, long requestId, string pathFolder);

    Task<IEnumerable<S3FileObject>> GetInfoAttachFileAsync(long fileId, string pathFolder);

    Task DeleteFileAsync(List<string> files, long fileId, string pathFolder);

    Task DeleteFolderAttachFileAsync(long id, string pathFolder);
}