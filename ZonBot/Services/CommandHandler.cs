using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace ZonBot.Services
{
    // Ideen vom dpy-Bot
    // TO-DO: [Admin]Purge messages
    // TO-DO: User: gif, embed, messagecheck
    // TO-DO: AFK
    // TO-DO: reaction role adding
    // TO-DO: tags
    // TO-DO: react with emote/message in 30 seconds (command name: blop)
    
    // Kais Ideen
    // TO-DO: welcome message upon joining guild
    // TO-DO: reaction roles
    // TO-DO: auto role (f.e. tracker -> rank role)
    // TO-DO: embeds for announcements
    // TO-DO: minigames?
    // TO-DO: voice channel creation (after joining default channel?)
    // TO-DO: twitch notifications
    // TO-DO: automod (spam protection, blacklisting (per word), whitelisting (per user))
    // TO-DO: xp system
    
    // Meine Ideen
    // TO-DO: [Utility]charinfo
    // TO-DO: [QoL]Reminder(Kalendererinnerung)
    
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;
        private readonly InteractiveService _interactive;

        public CommandHandler(DiscordSocketClient client, InteractionService interactions, IServiceProvider services, InteractiveService interactive)
        {
            _client = client;
            _interactions = interactions;
            _services = services;
            _interactive = interactive;
        }

        public async Task InitializeAsync()
        {
            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += HandleInteraction;

            // Process the command execution results 
            _interactions.SlashCommandExecuted += SlashInteractionExecuted;
            _interactions.ContextCommandExecuted += ContextInteractionExecuted;
            _interactions.ComponentCommandExecuted += ComponentInteractionExecuted;
        }

        #region Error Handling
        private async Task ComponentInteractionExecuted (ComponentCommandInfo commandInfo, IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess)
            {
                await ctx.Interaction.RespondAsync($"Command failed for the following reason:\n{result.ErrorReason}");

                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                }
            }
        }

        private async Task ContextInteractionExecuted (ContextCommandInfo commandInfo, IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess)
            {
                await ctx.Interaction.RespondAsync($"Command failed for the following reason:\n{result.ErrorReason}");

                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task SlashInteractionExecuted (SlashCommandInfo slashInfo, Discord.IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess)
            {
                await ctx.Interaction.RespondAsync($"Command failed for the following reason:\n{result.ErrorReason}");
                
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Execution
        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                if (arg is SocketMessageComponent messageComponent &&
                    _interactive.Callbacks.ContainsKey(messageComponent.Message.Id))
                {
                    return;
                }
                else
                {
                    // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                    var ctx = new SocketInteractionContext(_client, arg);
                    await _interactions.ExecuteCommandAsync(ctx, _services);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }
        # endregion
    }
}