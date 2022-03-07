using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZonBot.Services;
using Fergun.Interactive;

namespace ZonBot
{
    class Program
    {
        private static DiscordSocketClient _client;
        private static InteractionService _interactions;
        private static IConfiguration _config;
        private static IServiceProvider _services;
        
        public static async Task Main(string[] args)
        {
            _config = MakeConfig();

            await using var services = ConfigureServices(_config);
            _services = services;
            
            _client = services.GetRequiredService<DiscordSocketClient>();
            _interactions = services.GetRequiredService<InteractionService>();

            _client.Log += OnLog;
            _interactions.Log += OnLog;
            _services.GetRequiredService<InteractiveService>().Log += OnLog;

            _client.Ready += OnReady;

            // Here we can initialize the service that will register and execute our commands
            await services.GetRequiredService<CommandHandler>().InitializeAsync();
            await services.GetRequiredService<MessageReceivedHandler>().InitializeAsync();

            // Bot token can be provided from the Configuration object we set up earlier
            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private static IConfiguration MakeConfig()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Helpers.ConfigPath);
            builder.AddJsonFile("appsettings.json", optional: true);
            
            return builder.Build();
        }
        
        private static ServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var intents = 0 
                          //| GatewayIntents.GuildScheduledEvents
                          /*| GatewayIntents.DirectMessageTyping
                          | GatewayIntents.DirectMessageReactions
                          | GatewayIntents.DirectMessages*/
                          | GatewayIntents.GuildMessageTyping
                          | GatewayIntents.GuildMessageReactions
                          | GatewayIntents.GuildMessages
                          | GatewayIntents.GuildVoiceStates
                          //| GatewayIntents.GuildInvites
                          | GatewayIntents.GuildWebhooks
                          | GatewayIntents.GuildIntegrations
                          | GatewayIntents.GuildEmojis
                          | GatewayIntents.GuildBans
                          | GatewayIntents.Guilds;
            
            var discordConfig = new DiscordSocketConfig
            {
                GatewayIntents = intents
            };

            var client = new DiscordSocketClient(discordConfig);
            
            var sc = new ServiceCollection();
            sc.AddSingleton<IConfiguration>(configuration);
            sc.AddSingleton<DiscordSocketClient>(client);
            sc.AddSingleton<InteractionService>(services => new InteractionService(services.GetRequiredService<DiscordSocketClient>()));
            sc.AddSingleton<InteractiveService>(services => new InteractiveService(services.GetRequiredService<DiscordSocketClient>()));
            sc.AddSingleton<CommandHandler>(services => new CommandHandler(client: services.GetRequiredService<DiscordSocketClient>(),
                                                                                        interactions: services.GetRequiredService<InteractionService>(), 
                                                                                        services: services,
                                                                                        interactive: services.GetRequiredService<InteractiveService>()));
            sc.AddSingleton<MessageReceivedHandler>(services => new MessageReceivedHandler(client: services.GetRequiredService<DiscordSocketClient>()));
            
            return sc.BuildServiceProvider();
        }

        private static Task OnLog(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }

        private static async Task OnReady()
        {
            // Slash Commands and Context Commands are can be automatically registered, but this process needs to happen after the client enters the READY state.
            // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands. To determine the method we should
            // register the commands with, we can check whether we are in a DEBUG environment and if we are, we can register the commands to a predetermined test guild.

            try
            {
#if DEBUG
                ulong guildId = ulong.Parse(_config["debugGuild"] ?? "0");
                await _interactions.RegisterCommandsToGuildAsync(guildId, deleteMissing: true);
#else
                foreach (var guild in _client.Guilds)
                {
                    await _interactions.RegisterCommandsToGuildAsync(guild.Id, deleteMissing: true);
                }
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}