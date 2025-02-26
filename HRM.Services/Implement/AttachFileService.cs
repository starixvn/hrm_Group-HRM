namespace HRM.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using HRM.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    public class AttachFileService : IAttachFileService
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        public AttachFileService(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public void UploadFile(IList<IFormFile> files, long fileId, string pathFolder)
        {
            if (files != null && files.Any(x => x.Length > 0))
            {
                string dir = Path.Combine(hostingEnvironment.ContentRootPath, RemoveFirstSlash(pathFolder) + fileId);
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
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                }
            }
        }

        public List<string> GetAttachFile(long fileId, string pathFolder)
        {
            var files = new List<string>();
            string dir = Path.Combine(hostingEnvironment.ContentRootPath, RemoveFirstSlash(pathFolder) + fileId);
            if (Directory.Exists(dir))
            {
                string[] filePaths = Directory.GetFiles(dir);
                foreach (string filePath in filePaths)
                {
                    files.Add(Path.GetFileName(filePath));
                }
            }

            return files;
        }

        public void DeleteFile(List<string> files, long fileId, string pathFolder)
        {
            foreach (string file in files)
            {
                string dir = Path.Combine(hostingEnvironment.ContentRootPath, RemoveFirstSlash(pathFolder) + fileId + '/' + file);
                File.Delete(dir);
            }
        }

        public IList<AttachFile> GetInfoAttachFile(long fileId, string pathFolder)
        {
            var attachFiles = new List<AttachFile>();
            string path = RemoveFirstSlash(pathFolder) + fileId;
            string dir = Path.Combine(hostingEnvironment.ContentRootPath, path);
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir);
                foreach (string file in files)
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

        public void DeleteFolderAttachFile(long id, string pathFolder)
        {
            string dir = Path.Combine(hostingEnvironment.ContentRootPath, pathFolder + id);

            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        private string RemoveFirstSlash(string path)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            return path;
        }
    }
}