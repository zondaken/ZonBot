using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ZonBot.Services
{
    public class ReadyHandler : IHandler
    {
        private IConfiguration _config;
        private InteractionService _interactions;
        private DiscordSocketClient _client;

        public ReadyHandler(IConfiguration config, InteractionService interactions, DiscordSocketClient client)
        {
            _config = config;
            _interactions = interactions;
            _client = client;
        }

        public async Task InitializeAsync()
        {
            _client.Ready += OnReady;
        }

        private async Task OnReady()
        {
#if DEBUG
            await _interactions.RegisterCommandsToGuildAsync(ulong.Parse(_config["debugGuild"]), true);
#else
            await _interactions.RegisterCommandsGloballyAsync(deleteMissing: true);
#endif
        }
    }
}