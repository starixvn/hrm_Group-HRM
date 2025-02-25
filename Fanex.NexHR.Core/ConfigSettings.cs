namespace Fanex.NexHR.Core
{
    using System;
    using System.Configuration;

    public static class ConfigSettings
    {
        public static string EmailSetting => ConfigurationManager.AppSettings["Email"];
        public static bool IsShowCloseBalance => Convert.ToBoolean(ConfigurationManager.AppSettings["IsCloseBalance"]);
        public static bool AllowAdminLogin => Convert.ToBoolean(ConfigurationManager.AppSettings["AllowAdminLogin"]);
        public static bool IsDisabledSendMail => Convert.ToBoolean(ConfigurationManager.AppSettings["IsDisabledSendMail"]);
        public static byte SiteId => Convert.ToByte(ConfigurationManager.AppSettings["SiteId"]);
        public static string ApplicationDomain => ConfigurationManager.AppSettings["ApplicationDomain"];

        public static string SyncWorkingShift_LocationCode => ConfigurationManager.AppSettings["SyncWorkingShift_LocationCode"];
        public static string SyncWorkingShift_Timezone => ConfigurationManager.AppSettings["SyncWorkingShift_Timezone"];
        public static string SyncWorkingShift_Url => ConfigurationManager.AppSettings["SyncWorkingShift_Url"];

        public static string WhitelistIPs => ConfigurationManager.AppSettings["WhitelistIPs"];
        public static bool EnableManualSyncWorkingShift
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableManualSyncWorkingShift"]);

        public static string API_Auth_Username => ConfigurationManager.AppSettings["API.Auth:Username"];
        public static string API_Auth_Password => ConfigurationManager.AppSettings["API.Auth:Password"];

        // Letstalk
        public static bool IsEnableSendLetstalk
            => Convert.ToBoolean(ConfigurationManager.AppSettings["Letstalk:Enable"]);
        public static string Letstalk_TestingAccountId => ConfigurationManager.AppSettings["Letstalk:TestingAccountId"];
        public static string Letstalk_API_BaseUrl => ConfigurationManager.AppSettings["Letstalk.API:BaseUrl"];
        public static string Letstalk_Token_Username => ConfigurationManager.AppSettings["Letstalk.API.Token:Username"];
        public static string Letstalk_Token_Password => ConfigurationManager.AppSettings["Letstalk.API.Token:Password"];
        public static int Letstalk_BotId => Convert.ToInt32(ConfigurationManager.AppSettings["Letstalk:BotId"]);

        // Proxy
        public static bool IsEnableProxy => Convert.ToBoolean(ConfigurationManager.AppSettings["Proxy:Enable"]);
        public static string Proxy_Address => ConfigurationManager.AppSettings["Proxy:Address"];
        public static bool Proxy_BypassOnLocal => Convert.ToBoolean(ConfigurationManager.AppSettings["Proxy:BypassOnLocal"]);
    }
}