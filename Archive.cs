using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace SpisSekcjiManager
{
    public partial class Archive
    {
        public List<int> History { get; set; }
        public int Members { get; set; }
        public string Name { get; set; }
    }

    public partial class Archive
    {
        public static List<Archive> FromJson(string fileName) => JsonSerializer.Deserialize<List<Archive>>(File.ReadAllText($"{Directory.GetCurrentDirectory()}/data/{fileName}.json"), new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public static class SerializeArchive
    {
        public static void ToJson(this List<Archive> archive, string fileName) => File.WriteAllText($"{Directory.GetCurrentDirectory()}/data/{fileName}", JsonSerializer.Serialize(archive, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }));

        public static string ToString(this List<Archive> archive) => JsonSerializer.Serialize(archive, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}