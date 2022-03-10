using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace ZonBot.Modules.SlashCommands.Utility
{
    public class CharinfoModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        [SlashCommand("charinfo", "Utility for developers", runMode: RunMode.Async)]
        public async Task CharinfoAsync(IEmote? input)
        {
            string printableEmote = "";
            string response = "";
            
            if (input is null)
            {
                response += "Invalid emoji";
            }
            else if (input is Emoji emoji)
            {
                printableEmote = emoji.Name;

                int unicodeInt = printableEmote[0];
                string unicodeStr = unicodeInt.ToString("X4");

                response += $"Emote: \\{printableEmote}\n" +
                            $"Unicode: U+{unicodeStr}";
            }
            else if(input is Emote emote)
            {
                string a = emote.Animated ? "a" : "";
                string name = emote.Name;
                string id = emote.Id.ToString();
                
                printableEmote = $"<{a}:{name}:{id}>";

                response += $"\\{printableEmote}";
            }

            await RespondAsync(response, ephemeral: true);
        }
    }
}