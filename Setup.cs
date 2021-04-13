using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SpisSekcjiManager
{
    public partial class Setup
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public List<string> Files { get; set; }

        public Settings Settings { get; set; }
    }

    public partial class Settings
    {
        public string FirebaseLink { get; set; }

        public bool? AutoFix { get; set; }

        public bool? AutoCompare { get; set; }

        public bool? AutoUpdate { get; set; }
    }

    public partial class Setup
    {
        public static Setup FromJson() => JsonSerializer.Deserialize<Setup>(File.ReadAllText($"{Directory.GetCurrentDirectory()}/data/settings.json"), new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
