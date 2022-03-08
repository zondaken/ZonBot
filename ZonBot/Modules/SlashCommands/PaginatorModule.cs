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
}