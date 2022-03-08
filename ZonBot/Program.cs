using System.Collections.Concurrent;
using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ZonBot.Services;
using Fergun.Interactive;
using Microsoft.Extensions.Hosting;

namespace ZonBot
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = MakeHost();
            await InitializeHandlers(host);
            await host.RunAsync();
        }

        private static async Task InitializeHandlers(IHost host)
        {
            var interactive = host.Services.GetRequiredService<InteractiveService>();
            interactive.Log += async message => Console.WriteLine(message.Message);

            foreach (var handler in host.Services.GetServices<IHandler>())
            {
                await handler.InitializeAsync();
            }
        }

        private static IHost MakeHost()
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            hostBuilder.UseContentRoot(Helpers.ConfigPath); // uses appsettings.json as well
            hostBuilder.ConfigureDiscordHost(ConfigureDiscordHost);
            hostBuilder.UseInteractionService(ConfigureInteractionService); // Optionally wire up the interactions service
            hostBuilder.ConfigureServices(ConfigureServices);

            return hostBuilder.Build();
        }

        private static void ConfigureDiscordHost(HostBuilderContext context, DiscordHostConfiguration config)
        {
            config.SocketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 200,
                GatewayIntents = GatewayIntents.AllUnprivileged
            };

            config.Token = context.Configuration["token"];
        }
        
        private static void ConfigureInteractionService(HostBuilderContext _, InteractionServiceConfig config)
        {
            config.LogLevel = LogSeverity.Info;
            config.UseCompiledLambda = true;
        }

        private static void ConfigureServices(HostBuilderContext _, IServiceCollection services)
        {
            // Add any other services here
            services.AddSingleton<InteractionService>();
            services.AddSingleton<InteractiveService>();
            services.AddSingleton<IHandler, CommandHandler>();
            services.AddSingleton<IHandler, MessageReceivedHandler>();
        }
    }
}