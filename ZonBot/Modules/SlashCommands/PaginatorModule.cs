using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using Fergun.Interactive.Selection;
using RunMode = Discord.Interactions.RunMode;

namespace ZonBot.Modules.SlashCommands
{
    [Discord.Interactions.Group("paginator", "Paginator commands")]
    public class PaginatorModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        public InteractiveService Interactive { get; set; }
        
        [SlashCommand("static", "Static paginator", runMode: RunMode.Async)]
        public async Task PaginatorAsync()
        {
            var pages = new[]
            {
                new PageBuilder().WithDescription("Lorem ipsum dolor sit amet, consectetur adipiscing elit."),
                new PageBuilder().WithDescription("Praesent eu est vitae dui sollicitudin volutpat."),
                new PageBuilder().WithDescription("Etiam in ex sed turpis imperdiet viverra id eget nunc."),
                new PageBuilder().WithDescription("Donec eget feugiat nisi. Praesent faucibus malesuada nulla, a vulputate velit eleifend ut.")
            };

            var paginator = new StaticPaginatorBuilder()
                .AddUser(Context.User) // Only allow the user that executed the command to interact with the selection.
                .WithPages(pages) // Set the pages the paginator will use. This is the only required component.
                .Build();
            
            // Send the paginator to the source channel and wait until it times out after 10 minutes.
            await Interactive.SendPaginatorAsync(paginator, Context.Interaction, timeout: TimeSpan.FromMinutes(10), resetTimeoutOnInput: true);
        }

        [SlashCommand("lazy", "Lazy paginator", runMode: RunMode.Async)]
        public async Task LazyPaginatorAsync()
        {
            var paginator = new LazyPaginatorBuilder()
                .AddUser(Context.User)
                .WithPageFactory(GeneratePage) // The pages are now generated on demand using a local method.
                .WithMaxPageIndex(9) // You must specify the max. index the page factory can go. max. index 9 = 10 pages
                .Build();

            await Interactive.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(10));

            static PageBuilder GeneratePage(int index)
            {
                return new PageBuilder()
                    .WithDescription($"This is page {index + 1}.")
                    .WithRandomColor();
            }
        }
        
        
    }

    public static class PageBuilderExtensions
    {
        public static PageBuilder WithRandomColor(this PageBuilder builder) 
            => builder.WithColor(GetRandomColor());
        
        public static Color GetRandomColor()
            => new(Random.Shared.Next(0, 256), Random.Shared.Next(0, 256), Random.Shared.Next(0, 256));
    }
    
    [Discord.Commands.Group("custom")]
    public partial class CustomModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
    public InteractiveService Interactive { get; set; }

    // Sends a selection of buttons, where each option has its own button style/color.
    [SlashCommand("button", "custom button", runMode: RunMode.Async)]
    public async Task CustomButtonSelectionAsync()
    {
        // To be able to create buttons with custom colors, we need to create a custom selection and a builder for that new selection.
        // See ButtonSelectionBuilder<T> and ButtonSelection<T> (below) for more information.

        // A ButtonSelection uses ButtonOption<T>s, specifically created for this custom selection.
        var options = new ButtonOption<string>[]
        {
            new("Primary", ButtonStyle.Primary),
            new("Secondary", ButtonStyle.Secondary),
            new("Success", ButtonStyle.Success),
            new("Danger", ButtonStyle.Danger)
        };

        var pageBuilder = new PageBuilder()
            .WithDescription("Button selection")
            .WithRandomColor();

        var buttonSelection = new ButtonSelectionBuilder<string>()
            .WithOptions(options)
            .WithStringConverter(x => x.Option)
            .WithSelectionPage(pageBuilder)
            .AddUser(Context.User)
            .Build();
        
        await Interactive.SendSelectionAsync(buttonSelection, Context.Channel);
    }

    // Custom selection builder for ButtonSelections
    public class ButtonSelectionBuilder<T> : BaseSelectionBuilder<ButtonSelection<T>, ButtonOption<T>, ButtonSelectionBuilder<T>>
    {
        // Since this selection specifically created for buttons, it makes sense to make this option the default.
        public override InputType InputType => InputType.Buttons;

        // We must override the Build method
        public override ButtonSelection<T> Build() => new(this);
    }

    // Custom selection where you can override the default button style/color
    public class ButtonSelection<T> : BaseSelection<ButtonOption<T>>
    {
        public ButtonSelection(ButtonSelectionBuilder<T> builder)
            : base(builder)
        {
        }

        // This method needs to be overriden to build our own component the way we want.
        public override ComponentBuilder GetOrAddComponents(bool disableAll, ComponentBuilder builder = null)
        {
            builder ??= new ComponentBuilder();
            foreach (var option in Options)
            {
                var emote = EmoteConverter?.Invoke(option);
                string label = StringConverter?.Invoke(option);
                if (emote is null && label is null)
                {
                    throw new InvalidOperationException($"Neither {nameof(EmoteConverter)} nor {nameof(StringConverter)} returned a valid emote or string.");
                }

                var button = new ButtonBuilder()
                    .WithCustomId(emote?.ToString() ?? label)
                    .WithStyle(option.Style) // Use the style of the option
                    .WithEmote(emote)
                    .WithDisabled(disableAll);

                if (label is not null)
                    button.Label = label;

                builder.WithButton(button);
            }

            return builder;
        }
    }

    public record ButtonOption<T>(T Option, ButtonStyle Style); // An option with an style
}
}