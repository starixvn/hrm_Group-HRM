namespace HRM.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

public class Memo : EntityBase<int>
{
    [Required]
    public string Contents { get; set; }

    [Required]
    public string RefNo { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string SentTo { get; set; }

    [Required]
    public string SentFrom { get; set; }

    public string ForInfo { get; set; }

    public int TotalPages { get; set; }

    [DataType(DataType.Date)]
    public DateTime SentDate { get; set; }

    public int Category { get; set; }

    public int MemoType { get; set; }

    public string AttachFile { get; set; }

    public string CategoryName { get; set; }

    public int CheckForEdit { get; set; }

    public bool SendMail { get; set; }
    public int CompanyId { get; set; }

    public List<string> MemoDetailLink
    {
        get
        {
            var allLinks = new List<string>();
            if (MemoType == 1)
            {
                var links = Contents.Split(';');
                if (links.Length > 0)
                {
                    foreach (var link in links)
                    {
                        var item =
                            "<a href='{UploadBasePath}"
                            + HttpUtility.HtmlEncode(link)
                            + "'> "
                            + HttpUtility.HtmlEncode(link.Split('/')[link.Split('/').Length - 1])
                            + "</a> </br>";
                        allLinks.Add(item);
                    }
                }
            }
            else
            {
                allLinks.Add(Contents);
            }

            return allLinks;
        }
    }
}