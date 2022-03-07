using Discord.WebSocket;

namespace ZonBot.Services
{
    public class MessageReceivedHandler : IHandler
    {
        private readonly DiscordSocketClient _client;

        public MessageReceivedHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task InitializeAsync()
        {
            _client.MessageReceived += OnMessageReceived;
            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            var prefix = "!";
            
            if (arg.Content.StartsWith(prefix) && arg.Content.Length > prefix.Length)
            {
                await arg.Channel.SendMessageAsync("command detected");
            }
        }
    }
}