using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models;
using Microsoft.AspNetCore.Http;

namespace HRM.Services.Interfaces;

public interface IAttachFileService
{
    void UploadFile(IList<IFormFile> files, long fileId, string pathFolder);

    Task UploadFileAsync(IList<IFormFile> files, long requestId, string pathFolder);

    List<string> GetAttachFile(long fileId, string pathFolder);

    Task<List<string>> GetAttachFileAsync(long requestId, string pathFolder);

    Task<List<S3FileObject>> GetInfoAttachFileAsync(long fileId, string pathFolder);

    void DeleteFile(List<string> files, long fileId, string pathFolder);

    Task DeleteFileAsync(List<string> files, long fileId, string pathFolder);

    IList<AttachFile> GetInfoAttachFile(long fileId, string pathFolder);

    void DeleteFolderAttachFile(long id, string pathFolder);
}