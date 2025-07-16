namespace HRM.Models.LetsTalk
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class ReplyMarkup
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "button";

        [JsonProperty(PropertyName = "actions")]
        public List<ReplyAction> Actions { get; set; } = [];
    }

    public class ReplyAction
    {
        [JsonIgnore]
        public ReplyActionType ActionType { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type
        {
            get
            {
                return ActionType switch
                {
                    ReplyActionType.PostBack => ReplyActionTypeString.Postback,
                    ReplyActionType.Uri => ReplyActionTypeString.Uri,
                    ReplyActionType.OpenWeb => ReplyActionTypeString.OpenWeb,
                    ReplyActionType.OpenTextInput => ReplyActionTypeString.OpenTextInput,
                    _ => ReplyActionTypeString.Postback
                };
            }
        }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }
    }

    internal static class ReplyActionTypeString
    {
        public const string Postback = "postback";
        public const string Uri = "uri";
        public const string OpenWeb = "open_web";
        public const string OpenTextInput = "open_text_input";
    }
}