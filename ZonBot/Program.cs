using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ZonBot.Services;
using Fergun.Interactive;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ZonBot.TypeConverters;

namespace ZonBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }

        private async Task MainAsync(string[] args)
        {
            using var host = MakeHost();
            await InitializeConverters(host);
            await InitializeHandlers(host);
            await host.RunAsync();
        }

        private async Task InitializeConverters(IHost host)
        {
            host.Services.GetRequiredService<InteractionService>().AddTypeConverter<IEmote>(new EmoteConverter());
        }

        private IHost MakeHost()
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            hostBuilder.UseContentRoot(Helpers.ConfigPath); // uses appsettings.json as well
            hostBuilder.ConfigureDiscordHost(ConfigureDiscordHost);
            hostBuilder.UseInteractionService(ConfigureInteractionService); // Optionally wire up the interactions service
            hostBuilder.ConfigureServices(ConfigureServices);

            return hostBuilder.Build();
        }

        private void ConfigureDiscordHost(HostBuilderContext context, DiscordHostConfiguration config)
        {
            GatewayIntents intents = GatewayIntents.AllUnprivileged
                                        & ~GatewayIntents.GuildScheduledEvents // remove [GuildScheduledEvents] from base flag
                                        & ~GatewayIntents.GuildInvites; // remove [GuildInvites] from base flag

            config.SocketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 200,
                GatewayIntents = intents
            };

            config.Token = context.Configuration["token"];
        }
        
        private void ConfigureInteractionService(HostBuilderContext _, InteractionServiceConfig config)
        {
            config.LogLevel = LogSeverity.Info;
            config.UseCompiledLambda = true;
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Add any other services here
            services.AddSingleton<IConfiguration>(context.Configuration);
            services.AddSingleton<InteractionService>();
            services.AddSingleton<InteractiveService>();
            services.AddSingleton<IHandler, ReadyHandler>();
            services.AddSingleton<IHandler, CommandHandler>();
            services.AddSingleton<IHandler, MessageReceivedHandler>();
        }
        
        private async Task InitializeHandlers(IHost host)
        {
            // [DiscordSocketClient] and [InteractionService] already get their log from [.ConfigureDiscordHost] (?)
            var interactive = host.Services.GetRequiredService<InteractiveService>();
            interactive.Log += async message => Console.WriteLine(message.Message);

            foreach (var handler in host.Services.GetServices<IHandler>())
            {
                await handler.InitializeAsync();
            }
        }

    }
}