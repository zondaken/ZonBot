using Discord.WebSocket;

namespace ZonBot.Services
{
    public class MessageReceivedHandler
    {
        private readonly DiscordSocketClient _client;

        public MessageReceivedHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task InitializeAsync()
        {
            _client.MessageReceived += OnMessageReceived;
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