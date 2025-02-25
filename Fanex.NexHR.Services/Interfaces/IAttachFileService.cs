namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Models;

    public interface IAttachFileService
    {
        void UploadFile(IList<HttpPostedFileBase> files, long fileId, string pathFolder);

        List<string> GetAttachFile(long fileId, string pathFolder);

        void DeleteFile(List<string> files, long fileId, string pathFolder);

        IList<AttachFile> GetInfoAttachFile(long fileId, string pathFolder);

        void DeleteFolderAttachFile(long id, string pathFolder);
    }
}