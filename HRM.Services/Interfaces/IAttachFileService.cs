namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Models;
    using Microsoft.AspNetCore.Http;

    public interface IAttachFileService
    {
        void UploadFile(IList<IFormFile> files, long fileId, string pathFolder);

        List<string> GetAttachFile(long fileId, string pathFolder);

        void DeleteFile(List<string> files, long fileId, string pathFolder);

        IList<AttachFile> GetInfoAttachFile(long fileId, string pathFolder);

        void DeleteFolderAttachFile(long id, string pathFolder);
    }
}