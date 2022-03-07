using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace ZonBot.Modules.ContextCommands
{
    public class ContextCommandModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        [MessageCommand("Repeat")]
        public async Task RepeatMessage(IMessage msg)
        {
            var content = new StringBuilder();
            content.Append("With reference to ");
            content.Append(msg.GetJumpUrl());
            content.Append("\n\n");
            content.Append(msg.Content);
            
            await RespondAsync(content.ToString());
        }

        [UserCommand("SayHello")]
        public async Task SayHello(IUser user)
        {
            await RespondAsync($"Hello, {user.Mention}");
        }

        [UserCommand("kick-vc")]
        [RequireContext(ContextType.Guild)]
        public async Task KickVc(SocketGuildUser user)
        {
            if (user.VoiceChannel is not null)
            {
                await user.ModifyAsync(u => u.Channel = null);
            }

            await DeferAsync(ephemeral: true);
        }
    }
}