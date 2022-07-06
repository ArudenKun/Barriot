namespace Barriot.Application.Services.Client
{
    public class LogService : IConfigurableService
    {
        private readonly ILogger _logger;
        private readonly DiscordRestClient _client;
        private readonly InteractionService _service;

        public LogService(DiscordRestClient client, InteractionService service, ILoggerFactory factory)
        {
            _logger = factory.CreateLogger("Discord");
            _client = client;
            _service = service;
        }

        public async Task ConfigureAsync()
        {
            _client.Log += LogAsync;
            _service.Log += LogAsync;
            await Task.CompletedTask;
        }

        private async Task LogAsync(LogMessage arg)
        {
            var level = ConvertSeverity(arg.Severity);

            _logger.Log(level, arg.Message);
            await Task.CompletedTask;
        }

        private static LogLevel ConvertSeverity(LogSeverity severity)
            => severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Verbose => LogLevel.Debug,
                _ => throw new InvalidOperationException()
            };
    }
}
