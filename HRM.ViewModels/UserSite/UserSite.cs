using System;

namespace HRM.ViewModels.UserSite
{
    [Serializable]
    public class UserSite
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string SSOUrl { get; set; }
    }
}