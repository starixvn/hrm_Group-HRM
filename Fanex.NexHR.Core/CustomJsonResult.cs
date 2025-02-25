namespace Fanex.NexHR.Core
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class CustomJsonResult : JsonResult
    {
        private const string DateFormat = "dd MMM, yyyy HH:mm:ss";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };
                settings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeFormat = DateFormat
                });

                response.Write(JsonConvert.SerializeObject(Data, settings));
            }
        }
    }
}