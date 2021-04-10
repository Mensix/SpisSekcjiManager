using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SpisSekcjiManager
{
    public partial class Setup
    {
        [JsonProperty("login", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string Login { get; set; }

        [JsonProperty("password", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty("files", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public List<Files> Files { get; set; }

        [JsonProperty("settings", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public Settings Settings { get; set; }
    }

    public partial class Files
    {
        [JsonProperty("input", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string Input { get; set; }

        [JsonProperty("output", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string Output { get; set; }

        [JsonProperty("path", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }
    }

    public partial class Settings
    {
        [JsonProperty("firebaseLink", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string FirebaseLink { get; set; }

        [JsonProperty("autoFix", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutoFix { get; set; }

        [JsonProperty("autoCompare", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutoCompare { get; set; }

        [JsonProperty("autoUpdate", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutoUpdate { get; set; }
    }

    public partial class Setup
    {
        public static Setup FromJson(string json) => JsonConvert.DeserializeObject<Setup>(json, SpisSekcjiManager.Converter.Settings);
    }

    public static class SerializeSetup
    {
        public static string ToJson(this Setup self) => JsonConvert.SerializeObject(self, SpisSekcjiManager.Converter.Settings);
    }

    internal static class ConverterSetup
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
