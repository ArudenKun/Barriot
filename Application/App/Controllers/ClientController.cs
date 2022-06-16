using System.Timers;

namespace Barriot
{
    public class ClientController
    {
        private readonly System.Timers.Timer _timer;
        private readonly string _path;

        private readonly DiscordRestClient _client;
        private readonly InteractionService _service;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientController> _logger;

        public ClientController(DiscordRestClient client, InteractionService service, IConfiguration config, ILogger<ClientController> logger)
        {
            _client = client;
            _logger = logger;
            _service = service;
            _configuration = config;

            _path = Path.Combine("Logs", string.Format(
                "{0:yy-MM-dd_HH-mm-ss}.log",
                DateTime.UtcNow));

            _timer = new(15000)
            {
                AutoReset = true,
                Enabled = true,
            };
        }

        public async Task<bool> ConfigureAsync()
        {
            _client.Log += LogAsync;
            _timer.Elapsed += TimerElapsed;

            if (_configuration.GetValue<bool>("BuildCommands"))
            {
                await _service.RegisterCommandsGloballyAsync();
                _logger.LogInformation("Registered commands!");
            }

            List<string> messages = new();
            foreach (var file in Directory.GetFiles(Path.Combine("Files", "Messages")))
            {
                messages.Add(string.Join("\n", File.ReadAllLines(file)));

                File.Delete(file);
            }

            if (!messages.Any())
                return false;

            var users = await UserEntity.GetAllAsync();

            foreach (var user in users)
                user.Inbox = new(user.Inbox.Concat(messages));

            return true;
        }

        private async void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            var now = DateTime.UtcNow;
            var reminders = await RemindEntity.GetManyAsync(now);

            foreach (var reminder in reminders) // comes in at 1
            {
                reminder.Frequency--; // decrease frequency, now 0
                reminder.Expiration += reminder.SpanToRepeat; // irrelevant to later check.

                var user = await _client.GetUserAsync(reminder.UserId);

                try
                {
                    await user.SendMessageAsync($":alarm_clock: **Reminder!**\n\n> {reminder.Message}");
                }
                catch
                {
                    _logger.LogDebug("Failed to send reminder for: {} ({}) due to sharing no mutual guilds to send messages from.", user.Id, user.ToString());
                }

                if (reminder.Frequency <= 0) // if 0 or lower, delete.
                    await reminder.DeleteAsync();
            }

            await PollEntity.DeleteManyAsync(now);
            await TicTacToeEntity.DeleteManyAsync(now);
            await ConnectEntity.DeleteManyAsync(now);
        }

        private async Task LogAsync(LogMessage arg)
        {
            LogLevel loglevel = arg.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Verbose => LogLevel.Debug,
                _ => throw new ArgumentOutOfRangeException(nameof(arg))
            };
            _logger.Log(loglevel, arg.ToString());

            using StreamWriter sw = File.AppendText(_path);
            await sw.WriteLineAsync(arg.ToString());
        }
    }
}
