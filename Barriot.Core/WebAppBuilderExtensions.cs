using Barriot.Interaction;
using Barriot.Models;

namespace Barriot.Extensions
{
    internal static class WebAppBuilderExtensions
    {
        /// <summary>
        ///     Configures the middleware path for the <see cref="InteractionService"/> to take.
        /// </summary>
        /// <param name="builder">The currrent <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="path">The API path for this middleware to listen to.</param>
        /// <param name="pbk">The public key for this application.</param>
        /// <returns>The currrent <see cref="IApplicationBuilder"/> instance with the middleware configuration set.</returns>
        public static IApplicationBuilder RedirectInteractions(this IApplicationBuilder builder, string path, string pbk)
            => builder.MapWhen(ctx => ctx.Request.Path == path, app => app.UseMiddleware<InteractionMiddleware>(pbk));

        /// <summary>
        ///     Configures the middleware path for the voting API to take.
        /// </summary>
        /// <param name="builder">The currrent <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="path">The API path for this middleware to listen to.</param>
        /// <returns>The currrent <see cref="IApplicationBuilder"/> instance with the middleware configuration set.</returns>
        public static IApplicationBuilder RedirectVotes(this IApplicationBuilder builder, string path, string vak)
            => builder.MapWhen(ctx => ctx.Request.Path == path && ctx.Request.Method == "POST", app => app.UseMiddleware<VotingMiddleware>(vak));

        /// <summary>
        ///     Adds the interaction service alongside its respective configuration to the service collection.
        /// </summary>
        /// <param name="services">The current <see cref="IServiceCollection"/> instance.</param>
        /// <returns>The current <see cref="IServiceCollection"/> with the <see cref="InteractionService"/> included.</returns>
        public static IServiceCollection AddInteractionService(this IServiceCollection services)
        {
            var config = new InteractionServiceConfig()
            {
                RestResponseCallback = ResponseCallback,
                DefaultRunMode = RunMode.Sync,
                UseCompiledLambda = true
            };
            services.AddSingleton(config);
            services.AddSingleton<InteractionService>();
            return services;
        }

        private static async Task ResponseCallback(IInteractionContext context, string body)
        {
            if (context is not BarriotInteractionContext intContext)
                throw new InvalidOperationException($"Provided context isn't a type of {nameof(BarriotInteractionContext)}");

            await intContext.InteractionResponseCallback(body);
        }
    }
}
