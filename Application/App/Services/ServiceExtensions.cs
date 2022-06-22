﻿using Barriot.Application.API;
using Barriot.Application.Interactions;
using Barriot.Data;
using MongoDB.Driver;

namespace Barriot.Application.Services
{
    public static class ServiceExtensions
    {
        public static async Task<DiscordRestClient> CreateClientAsync(string token)
        {
            var config = new DiscordRestConfig()
            {
                APIOnRestInteractionCreation = false,
                FormatUsersInBidirectionalUnicode = false,
                LogLevel = LogSeverity.Verbose,
            };

            var client = new DiscordRestClient(config);

            await client.LoginAsync(TokenType.Bot, token);
            return client;
        }

        public static IServiceCollection AddInteractions(this IServiceCollection provider)
        {
            var config = new InteractionServiceConfig()
            {
                DefaultRunMode = RunMode.Sync,
                UseCompiledLambda = true,
                RestResponseCallback = new(async (ctx, str) =>
                {
                    if (ctx is not BarriotInteractionContext context)
                        throw new InvalidOperationException();

                    await context.InteractionResponseCallback(str);
                }),
                LogLevel = LogSeverity.Verbose,
            };

            provider.AddSingleton(config);
            provider.AddSingleton<InteractionService>();
            provider.AddSingleton<PostExecutionManager>();
            provider.AddSingleton<InteractionApiManager>();

            return provider;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection provider, string token)
        {
            var client = new MongoClient(new MongoUrlBuilder(token).ToMongoUrl());

            provider.AddSingleton(client);
            provider.AddSingleton<DatabaseManager>();
            return provider;
        }

        public static IServiceCollection AddImplementations(this IServiceCollection provider)
        {
            //provider.AddSingleton<UserService>();
            //provider.AddSingleton<TranslateService>();
            //provider.AddSingleton<InfoService>();

            //return provider;

            var interfaceType = typeof(IService);

            foreach (var type in typeof(Program).Assembly.GetTypes())
            {
                if (interfaceType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    provider.AddSingleton(type);
                }
            }

            return provider;
        }

        public static IServiceCollection AddHttp(this IServiceCollection provider, IConfigurationSection section)
        {
            provider.AddHttpClient<ITranslateClient, TranslateClient>(client =>
            {
                client.BaseAddress = new Uri(section["Translation"]);
            });

            return provider;
        }
    }
}
