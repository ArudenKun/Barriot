using Barriot.Interaction;

namespace Barriot
{
    public sealed class InteractionMiddleware
    {
        private readonly DiscordRestClient _client;
        private readonly PostExecutionHandler _resultHandler;
        private readonly ILogger<InteractionMiddleware> _logger;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestDelegate _next;
        private readonly string _pbk;

        public InteractionMiddleware(DiscordRestClient client, InteractionService interactionService, string pbk,
            ILogger<InteractionMiddleware> logger, RequestDelegate next, IServiceProvider serviceProvider, PostExecutionHandler postExec)
        {
            _logger = logger;
            _client = client;
            _resultHandler = postExec;
            _interactions = interactionService;
            _pbk = pbk;
            _serviceProvider = serviceProvider;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Internal respond task so calls arent repeated.
            async Task RespondAsync(int statusCode, string responseBody)
            {
                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(responseBody).ConfigureAwait(false);
                await httpContext.Response.CompleteAsync().ConfigureAwait(false);
            }

            if (httpContext.Request.Method != "POST")
            {
                _logger.LogError("Failure (Invalid REST method)");
                await RespondAsync(StatusCodes.Status200OK, "Success!");
                return;
            }

            // Read out the stream and parse the signatures.
            var signature = httpContext.Request.Headers["X-Signature-Ed25519"];
            var timestamp = httpContext.Request.Headers["X-Signature-Timestamp"];
            using var sr = new StreamReader(httpContext.Request.Body);
            var body = await sr.ReadToEndAsync();

            await _next(httpContext);

            // If the interaction is invalid, return here.
            if (!_client.IsValidHttpInteraction(_pbk, signature, timestamp, body))
            {
                _logger.LogError("Failure (Invalid interaction signature)");
                await RespondAsync(StatusCodes.Status401Unauthorized, "Invalid Interaction Signature!");
                return;
            }

            RestInteraction interaction = await _client.ParseHttpInteractionAsync(_pbk, signature, timestamp, body, x =>
            {
                if (!string.IsNullOrEmpty(x.Name))
                    return x.Name switch
                    {
                        "challenge" => true,
                        "channel" => true,
                        _ => false
                    };

                var range = x.CustomId.Split('-');
                if (range.Any())
                    return range.First() switch
                    {
                        "sar" => true,
                        "channel" => true,
                        _ => false
                    };
                return false;
            });

            // Recognize a ping interaction from Discord to check if our receiving end functions properly
            if (interaction is RestPingInteraction pingInteraction)
            {
                _logger.LogInformation("Successful (Ping)");
                await RespondAsync(StatusCodes.Status200OK, pingInteraction.AcknowledgePing());
                return;
            }

            // Create the context to pass into execution.
            var context = new BarriotInteractionContext(_client, interaction, (str) => RespondAsync(StatusCodes.Status200OK, str));

            // Execute the command.
            var result = await _interactions.ExecuteCommandAsync(context, _serviceProvider);

            await _resultHandler.RunAsync(result, context).ConfigureAwait(false);
        }
    }
}
