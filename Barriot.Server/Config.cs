using Newtonsoft.Json;

namespace Server;

public class Config
{
    public static Settings Settings { get; private set; }
        = Settings.Read();
}

public class Settings
{
    public string Token { get; set; } = "";

    public ulong VoteChannel { get; set; }

    public ulong VoteRole { get; set; }

    public string[] Emotes { get; set; } = Array.Empty<string>();

    public Dictionary<ulong, string[]> ChannelHandleCallback { get; set; } = new();

    public static Settings Read()
    {
        string path = "Config.json";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(new Settings(), Formatting.Indented));
            return new Settings();
        }
        return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path))
            ?? throw new NullReferenceException();
    }
}
