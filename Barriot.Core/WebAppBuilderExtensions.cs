using Barriot.Caching;
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
        ///     Generates the starting time of the application once the ASP instance is started, and is kept in services until the app shuts down.
        /// </summary>
        /// <param name="services">The current <see cref="IServiceCollection"/> instance.</param>
        /// <returns>The current <see cref="IServiceCollection"/> with the <see cref="UptimeTracker"/> included.</returns>
        public static IServiceCollection WithUptimeTracker(this IServiceCollection services)
        {
            var appStart = new UptimeTracker();

            return services.AddSingleton(appStart);
        }

        /// <summary>
        ///     Sets up caching for any guilds Barriot has access to and can converse with. 
        ///     Calls to the API shouldn't be made frequently, and this prevents just that.
        /// </summary>
        /// <param name="services">The current <see cref="IServiceCollection"/> instance.</param>
        /// <returns>The current <see cref="IServiceCollection"/> with the <see cref="GuildCache"/> included.</returns>
        public static IServiceCollection WithGuildCaching(this IServiceCollection services)
            => services.AddSingleton<GuildCache>();

        /// <summary>
        ///     Sets up caching for any users Barriot communicates with. 
        ///     Calls to the API shouldn't be made frequently, and this prevents just that.
        /// </summary>
        /// <param name="services">The current <see cref="IServiceCollection"/> instance.</param>
        /// <returns>The current <see cref="IServiceCollection"/> with the <see cref="UserCache"/> included.</returns>
        public static IServiceCollection WithUserCaching(this IServiceCollection services)
            => services.AddSingleton<UserCache>();

        /// <summary>
        ///     Sets up caching for all languages in the LibreLang translate book.
        ///     Calls to the API are reduced and languages are sorted at a lower level so all languages can be included in multiple dropdown menu's.
        /// </summary>
        /// <param name="services">The current <see cref="IServiceCollection"/> instance.</param>
        /// <returns>The current <see cref="IServiceCollection"/> with the <see cref="TranslationCache"/> included.</returns>
        public static IServiceCollection WithTranslationCaching(this IServiceCollection services)
            => services.AddSingleton<TranslationCache>();

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
