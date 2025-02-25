namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Resources;
    using GO.UserControl.Client.Core.Session;

    public class FileController : BaseController
    {
        private const int MaxSizeFileUpload = 5 * 1024 * 1024;

        public FileController(IUserControlSession userControlSession) : base(userControlSession)
        {
        }

        [HttpPost]
        public string UploadFile(IEnumerable<HttpPostedFileBase> files, int categoryId, bool isOverride)
        {
            string pathAttachFiles = string.Empty;

            foreach (HttpPostedFileBase file in files)
            {
                if (file.ContentLength > 0)
                {
                    string pathAttach = SaveFileToServer(file, categoryId, isOverride);

                    pathAttachFiles = (string.IsNullOrEmpty(pathAttachFiles)) ? pathAttach : string.Concat(pathAttachFiles, ";/", pathAttach);
                }
            }

            return pathAttachFiles;
        }

        private string SaveFileToServer(HttpPostedFileBase file, int categoryId, bool isOverride)
        {
            var fileName = new StringBuilder(Path.GetFileName(file.FileName));
            var dir = "~/file/uploads/" + categoryId;
            var pathAttach = string.Format("{0}/{1}", dir, fileName);

            if (isOverride)
            {
                while (System.IO.File.Exists(Server.MapPath(pathAttach)))
                {
                    fileName.Insert(0, "(1)");
                    pathAttach = string.Format("{0}/{1}", dir, fileName);
                }
            }

            if (!Directory.Exists(Server.MapPath(dir)))
            {
                Directory.CreateDirectory(Server.MapPath(dir));
            }

            var path = Path.Combine(Server.MapPath(dir), fileName.ToString());
            file.SaveAs(path);
            const byte StartIndex = 2;

            return pathAttach.Substring(StartIndex);
        }

        public bool Delete(string fileName)
        {
            System.IO.File.Delete(Server.MapPath("~/" + fileName));
            return true;
        }

        [HttpPost]
        public ActionResult MyProfileUploadFile(IEnumerable<HttpPostedFileBase> files)
        {
            if (!files.Any(file => file.ContentLength <= 0))
            {
                if (!IsValidFileUpload(files))
                {
                    return Json(new { isSuccessful = false, message = "The file(s) uploaded invalid" });
                }

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string basePath = HostingEnvironment.ApplicationPhysicalPath + Constants.ProfileUploadPath + EmployeeSession.Code;
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
                        var path = Path.Combine(Server.MapPath("~/file/uploads/profile/"), EmployeeSession.Code, fileName);
                        file.SaveAs(path);
                    }
                }

                return Json(new { isSuccessful = true, message = "File(s) uploaded sucessful." });
            }

            return Json(new { isSuccessful = false, message = "File(s) uploaded failed." });
        }

        private static bool IsValidFileUpload(IEnumerable<HttpPostedFileBase> files)
        {
            string[] allowedExtentsions = new[] { ".jpg", ".png", ".jpeg", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };
            foreach (var file in files)
            {
                if (file.ContentLength > MaxSizeFileUpload)
                {
                    return false;
                }
                string extentsion = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtentsions.Contains(extentsion))
                {
                    return false;
                }
            }

            return true;
        }

        [HttpPost]
        public ActionResult DeleteMyProfileUploadedFile(string fileName)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            string path = HostingEnvironment.ApplicationPhysicalPath + Constants.ProfileUploadPath + EmployeeSession.Code + "/" + fileName;
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
}