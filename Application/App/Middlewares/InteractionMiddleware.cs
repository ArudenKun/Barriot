using Barriot.Interactions;

namespace Barriot
{
    public sealed class InteractionMiddleware
    {
        private readonly DiscordRestClient _client;
        private readonly PostExecutionHandler _resultHandler;
        private readonly ILogger<InteractionMiddleware> _logger;
        private readonly ApiController _controller;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestDelegate _next;
        private readonly string _pbk;

        public InteractionMiddleware(DiscordRestClient client, InteractionService interactionService, string pbk,
            ILogger<InteractionMiddleware> logger, RequestDelegate next, IServiceProvider serviceProvider, PostExecutionHandler postExec, ApiController controller)
        {
            _logger = logger;
            _client = client;
            _resultHandler = postExec;
            _interactions = interactionService;
            _controller = controller;
            _pbk = pbk;
            _serviceProvider = serviceProvider;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            async Task RespondAsync(int statusCode, string responseBody)
            {
                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(responseBody).ConfigureAwait(false);
                await httpContext.Response.CompleteAsync().ConfigureAwait(false);
            }

            if (httpContext.Request.Method is not "POST")
            {
                _logger.LogError("Failure (Invalid REST method)");
                await RespondAsync(StatusCodes.Status200OK, "Success!");
                return;
            }

            var signature = httpContext.Request.Headers["X-Signature-Ed25519"];
            var timestamp = httpContext.Request.Headers["X-Signature-Timestamp"];
            using var sr = new StreamReader(httpContext.Request.Body);
            var body = await sr.ReadToEndAsync();

            await _next(httpContext);

            if (!_client.IsValidHttpInteraction(_pbk, signature, timestamp, body))
            {
                _logger.LogError("Failure (Invalid interaction signature)");
                await RespondAsync(StatusCodes.Status401Unauthorized, "Invalid Interaction Signature!");
                return;
            }

            RestInteraction interaction = await _client.ParseHttpInteractionAsync(_pbk, signature, timestamp, body, _controller.Predicate);
            if (interaction is RestPingInteraction pingInteraction)
            {
                _logger.LogInformation("Successful (Ping)");
                await RespondAsync(StatusCodes.Status200OK, pingInteraction.AcknowledgePing());
                return;
            }

            var context = new BarriotInteractionContext(_client, interaction, (str) => RespondAsync(StatusCodes.Status200OK, str));
            var result = await _interactions.ExecuteCommandAsync(context, _serviceProvider);

            await _resultHandler.RunAsync(result, context).ConfigureAwait(false);
        }
    }
}
