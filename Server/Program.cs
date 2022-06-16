namespace Server;

public class Program
{
    private readonly DiscordSocketClient _client;

    private readonly DiscordSocketConfig _config = new()
    {
        AlwaysDownloadUsers = true,
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
        LogLevel = LogSeverity.Info,
    };

    public Program()
    {
        _client = new(_config);

        _client.Log += Log;
        _client.Ready += Ready;

        _ = new SocketOperator(_client);
    }

    static void Main(string[] args)
        => new Program()
            .RunAsync()
            .GetAwaiter()
            .GetResult();

    public async Task RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, Config.Settings.Token);

        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private async Task Ready()
        => await _client.SetGameAsync("the Barriot server", null, ActivityType.Watching);

    private async Task Log(LogMessage args)
    {
        Console.WriteLine(args);
        await Task.CompletedTask;
    }
}
