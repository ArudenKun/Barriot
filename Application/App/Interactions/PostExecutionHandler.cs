﻿using Barriot.Interactions.Attributes;
using IResult = Discord.Interactions.IResult; // Ambiguous reference to Microsoft.Extensions

namespace Barriot.Interactions
{
    public class PostExecutionHandler
    {
        private readonly Dictionary<InteractionCommandError, Func<IResult, BarriotInteractionContext, Task>> _callBack = new();

        private readonly ILogger<PostExecutionHandler> _logger;

        public PostExecutionHandler(ILogger<PostExecutionHandler> logger)
        {
            _logger = logger;

            _callBack[InteractionCommandError.UnknownCommand] = UnknownCommandHandler;
            _callBack[InteractionCommandError.UnmetPrecondition] = UnmetPreconditionHandler;
        }

        /// <summary>
        ///     Runs post execution logic on an interaction.
        /// </summary>
        /// <param name="result">The result of the interaction execution.</param>
        /// <param name="context">The <see cref="BarriotInteractionContext"/> created in the middleware.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task RunAsync(IResult result, BarriotInteractionContext context)
        {
            if (result.Error.HasValue)
            {
                if (_callBack.TryGetValue(result.Error.Value, out var value))
                    await value(result, context);
                else
                    await FallbackHandler(result, context);
            }
        }

        private async Task FallbackHandler(IResult result, BarriotInteractionContext context)
        {
            if (result is ExecuteResult execResult)
                _logger.LogError("Runtime exception for '{}' ({}): {} ", context.User.Id, context.User, execResult.Exception);
            else
                _logger.LogError("Unknown command failure for '{}' ({}): {}", context.User.Id, context.User, result.ErrorReason);
            await Task.CompletedTask;
        }

        private async Task UnknownCommandHandler(IResult result, BarriotInteractionContext context)
        {
            await context.InteractionResponseCallback(
                context.Interaction.Respond(
                    text: ":white_check_mark: **Received interaction!**",
                    ephemeral: true));
            _logger.LogError("Command not found for '{}' ({}): {}", context.User.Id, context.User, result.ErrorReason);
        }

        private async Task UnmetPreconditionHandler(IResult result, BarriotInteractionContext context)
        {
            if (result is BarriotPreconditionResult preconResult)
            {
                if (preconResult.DisplayReason is not null)
                {
                    await context.InteractionResponseCallback(
                        context.Interaction.Respond(
                            text: $":x: {preconResult.DisplayReason}",
                            ephemeral: true));
                }
                _logger.LogError("Precondition unmet for '{}' ({}): {}", context.User.Id, context.User, result.ErrorReason);
            }
            else
                _logger.LogError("PreconditionResult in invalid state. Expected {}, Got {}", nameof(BarriotPreconditionResult), nameof(PreconditionResult));
        }
    }
}
