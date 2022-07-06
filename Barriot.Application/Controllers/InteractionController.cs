using Barriot.Application.Controllers.Builders;
using Barriot.Application.Interactions;
using Microsoft.AspNetCore.Mvc;

namespace Barriot.Application.Controllers
{
    [ApiController]
    [Route("interactions")]
    public class InteractionController : ControllerBase
    {
        const string _contentType = "application/json";

        private readonly ILogger<InteractionController> _logger;
        private readonly DiscordRestClient _client;
        private readonly InteractionService _service;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly InteractionApiManager _apiManager;
        private readonly PostExecutionManager _postExecManager;

        public InteractionController(
            DiscordRestClient client, 
            InteractionService service, 
            ILogger<InteractionController> logger,
            IServiceProvider provider,
            IConfiguration config,
            InteractionApiManager apiManager,
            PostExecutionManager postExecManager)
        {
            _logger = logger;
            _client = client;
            _service = service;
            _serviceProvider = provider;
            _configuration = config;
            _apiManager = apiManager;
            _postExecManager = postExecManager;
        }

        [HttpGet]
        public IActionResult GetAsync()
            => Ok(Content("Interaction endpoint available.", _contentType));

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            var signature = HttpContext.Request.Headers["X-Signature-Ed25519"];
            var timestamp = HttpContext.Request.Headers["X-Signature-Timestamp"];
            using var sr = new StreamReader(HttpContext.Request.Body);
            var body = await sr.ReadToEndAsync();

            if (!_client.IsValidHttpInteraction(_configuration["PublicToken"], signature, timestamp, body))
            {
                _logger.LogError("Failure (Invalid interaction signature)");

                return new ContentResultBuilder(401)
                    .WithPayload("Failed to verify interaction!")
                    .Build();
            }

            RestInteraction interaction = await _client.ParseHttpInteractionAsync(_configuration["PublicToken"], signature, timestamp, body, _apiManager.Predicate);

            if (interaction is RestPingInteraction pingInteraction)
            {
                _logger.LogInformation("Successful (Ping)");

                return new ContentResultBuilder(200)
                    .WithPayload(pingInteraction.AcknowledgePing())
                    .Build();
            }

            string payload = string.Empty;

            var context = new BarriotInteractionContext(await UserEntity.GetAsync(interaction.User.Id), _client, interaction, (str) =>
            {
                payload = str;
                return Task.CompletedTask;
            });

            var result = await _service.ExecuteCommandAsync(context, _serviceProvider);

            await _postExecManager.RunAsync(result, context);

            return new ContentResultBuilder(200)
                .WithPayload(payload)
                .Build();
        }
    }
}