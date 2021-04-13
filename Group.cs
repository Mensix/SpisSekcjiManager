using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpisSekcjiManager
{
    public partial class Dataset
    {
        public List<Group> Groups { get; set; }
        public string LastUpdateDate { get; set; }
        public string Name { get; set; }
    }

    public class Group
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string> Category { get; set; }
        public string Link { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Members { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? MembersGrowth { get; set; }
        public string Name { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? IsSection { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? IsOpen { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string> Keywords { get; set; }
    }

    public partial class Dataset
    {
        public static Dataset FromJson(string fileName)
        {
            return JsonSerializer.Deserialize<Dataset>(File.ReadAllText($"{Directory.GetCurrentDirectory()}/data/{fileName}.json"), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    public static class SerializeGroups
    {
        public static void ToJson(this Dataset dataset, string fileName) => File.WriteAllText($"{Directory.GetCurrentDirectory()}/data/{fileName}", JsonSerializer.Serialize(dataset, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));

        public static string ToString(this Dataset dataset) => JsonSerializer.Serialize(dataset, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
