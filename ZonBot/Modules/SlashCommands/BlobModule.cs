using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace ZonBot.Modules.SlashCommands
{
    public enum ButtonEnum
    {
        FooButton
    }

    [Group("blob", "all blob commands")]
    public class BlobModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        public InteractiveService Interactive { get; set; }

        [SlashCommand("message", "Make a delayed answer", runMode: RunMode.Async)]
        public async Task BlobMessageAsync()
        {
            await RespondAsync("Waiting for another message to repeat...");

            var filter = (SocketMessage r) => r.Author == Context.User && r.Channel.Id == Context.Channel.Id;
            SocketMessage? userResponse =
                (await Interactive.NextMessageAsync(filter, timeout: TimeSpan.FromSeconds(30))).Value;

            await Context.Channel.SendMessageAsync(userResponse?.Content);
        }

        [SlashCommand("reaction", "Make a delayed reaction", runMode: RunMode.Async)]
        public async Task BlobReactionAsync()
        {
            await RespondAsync("Waiting for reaction to say blob...");
            
            var message = await Context.Interaction.GetOriginalResponseAsync(); // api call unavoidable

            var emoji = new Emoji("😄");

            await message.AddReactionAsync(emoji);

            var filter = (SocketReaction r) =>
            {
                if (r.User.Value.IsBot)
                    return false;

                return Context.Interaction.User == r.User.Value && message.Id == r.MessageId && Equals(r.Emote, emoji);
            };
            
            SocketReaction? userResponse =
                (await Interactive.NextReactionAsync(filter, timeout: TimeSpan.FromSeconds(30))).Value;

            if (userResponse != null)
            {
                await Context.Channel.SendMessageAsync("yay");
            }
        }

        [SlashCommand("button", "Use buttons")]
        [RequireContext(ContextType.Guild)]
        public async Task BlobButtonAsync()
        {
            var builder = new ComponentBuilder()
                .WithButton("Primary", "AVeryUniqueNewAndBetterCustomId", style: ButtonStyle.Primary,
                    emote: Emoji.Parse(":smile:"))
                .WithButton("Secondary", "2", style: ButtonStyle.Secondary)
                .WithButton("Success", "3", style: ButtonStyle.Success)
                .WithButton("Danger", "4", style: ButtonStyle.Danger)
                .WithButton("Link", url: "https://www.github.com", style: ButtonStyle.Link);

            await RespondAsync("Response", components: builder.Build());


            //await InteractionUtility.ConfirmAsync(Context.Client, Context.Channel, TimeSpan.FromMinutes(10));
            //MentionUtils;

            if (await InteractionUtility.WaitForMessageComponentAsync(Context.Client, await GetOriginalResponseAsync(),
                    TimeSpan.FromMinutes(10)) is IComponentInteraction interaction)
            {
                await FollowupAsync(interaction.Data.CustomId);
            }

        }
        
        [ComponentInteraction("AVeryUniqueNewAndBetterCustomId", ignoreGroupNames: true)]
        public async Task PrimaryButton() => await RespondAsync("Primary");
        
        [ComponentInteraction("2", ignoreGroupNames: true)]
        public async Task SecondaryButton() => await RespondAsync("Secondary");

        [ComponentInteraction("3", ignoreGroupNames: true)]
        public async Task SuccessButton()
        {
            await DeferAsync();

            if (Context.Interaction is SocketMessageComponent interaction)
            {
                var message = interaction.Message;
                await message.ModifyAsync(msg => msg.Content = "Success");
            }
            else
            {
                throw new Exception("Context.Interaction is not SocketMessageComponent. Why?");
            }
        }
        
        [ComponentInteraction("4", ignoreGroupNames: true)]
        public async Task DangerButton() => await RespondAsync("Danger");
    }
}