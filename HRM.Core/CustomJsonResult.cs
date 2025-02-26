using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;

namespace HRM.Core;

public class CustomJsonResult(object data) : IActionResult
{
    private const string DateFormat = "dd MMM, yyyy HH:mm:ss";

    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        var response = context.HttpContext.Response;
        response.ContentType = "application/json";
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> { new IsoDateTimeConverter { DateTimeFormat = DateFormat } }
        };

        var json = JsonConvert.SerializeObject(data, settings);
        await response.WriteAsync(json);
    }
}