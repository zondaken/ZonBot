using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace ZonBot.Modules.SlashCommands
{
    public class TemplateModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        [SlashCommand("template", "template", runMode: RunMode.Async)]
        public async Task TemplateAsync()
        {
            await Task.Delay(0);
        }
    }
}