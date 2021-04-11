using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SpisSekcjiManager
{
    public partial class Dataset
    {
        [JsonProperty("groups", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public List<Group> Groups { get; set; }

        [JsonProperty("lastUpdateDate", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string LastUpdateDate { get; set; }
        [JsonProperty("name", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public partial class Group
    {
        [JsonProperty("category", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Category? Category { get; set; }

        [JsonProperty("link", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri Link { get; set; }

        [JsonProperty("members", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Members { get; set; }

        [JsonProperty("membersGrowth", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? MembersGrowth { get; set; }

        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("isSection", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSection { get; set; }
        [JsonProperty("isOpen", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsOpen { get; set; }

        [JsonProperty("keywords", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Keywords { get; set; }
    }

    public partial struct Category
    {
        public string String;
        public List<string> StringArray;

        public static implicit operator Category(string String) => new Category { String = String };
        public static implicit operator Category(List<string> StringArray) => new Category { StringArray = StringArray };
    }

    public partial class Dataset
    {
        public static Dataset FromJson(string json) => JsonConvert.DeserializeObject<Dataset>(json, SpisSekcjiManager.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Dataset self) => JsonConvert.SerializeObject(self, SpisSekcjiManager.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                CategoryConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class CategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Category) || t == typeof(Category?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Category { String = stringValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<string>>(reader);
                    return new Category { StringArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type Category");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Category)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray);
                return;
            }
            throw new Exception("Cannot marshal type Category");
        }

        public static readonly CategoryConverter Singleton = new CategoryConverter();
    }
}
