using Discord.Interactions;
using Discord.WebSocket;

namespace ZonBot.Modules.SlashCommands
{
    public class SimpleResponseModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        [SlashCommand("foo", "Says bar.")]
        public async Task FooAsync()
        {
            await RespondAsync("bar");
        }
        
        [SlashCommand("hello", "Says 'Hello, world!'.")]
        public async Task HelloAsync()
        {
            await RespondAsync("Hello, world!");
        }
        
        [SlashCommand("ping", "Pings the bot and returns its latency.")]
        public async Task PingAsync()
        {
            int latency = Context.Client.Latency;
            await RespondAsync($":ping_pong: It took me {latency}ms to respond to you!", ephemeral: true);
        }
    }
}