using System.IO;

namespace HRM.Core.Models;

public class S3FileObject
{
    public string FileName { get; set; }

    public string FilePath { get; set; }

    public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);

    public string Extension => Path.GetExtension(FileName);
}