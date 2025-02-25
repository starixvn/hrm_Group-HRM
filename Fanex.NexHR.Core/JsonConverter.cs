namespace Fanex.NexHR.Core
{
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    public static class JsonConverter
    {
        public static string ToJson(object o)
        {
            var jsonSerializer = new JavaScriptSerializer();

            return jsonSerializer.Serialize(o);
        }

        public static T FromJson<T>(string s)
        {
            var serializer = new JavaScriptSerializer();

            return serializer.Deserialize<T>(s);
        }

        public static List<T> FromJsonArray<T>(string s)
        {
            var javaScriptSerializer = new JavaScriptSerializer();

            return javaScriptSerializer.Deserialize<List<T>>(s);
        }
    }
}
