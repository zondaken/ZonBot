using Discord.Interactions;
using Discord.WebSocket;

namespace ZonBot.Modules.SlashCommands
{
    public class TemplateModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        [SlashCommand("template", "template", runMode: RunMode.Async)]
        public async Task TemplateAsync()
        {
            await DeferAsync(ephemeral: true);
        }
    }
}