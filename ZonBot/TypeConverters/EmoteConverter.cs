using Discord;
using Discord.Interactions;

namespace ZonBot.TypeConverters
{
    public class EmoteConverter : TypeConverter<IEmote>
    {
        public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            var input = (string) option.Value;

            if (Emoji.TryParse(input, out Emoji emoji))
            {
                return Task.FromResult(TypeConverterResult.FromSuccess(emoji));
            }
            else if(Emote.TryParse(input, out Emote emote))
            {
                return Task.FromResult(TypeConverterResult.FromSuccess(emote));
            }
            else
            {
                return Task.FromResult(TypeConverterResult.FromSuccess(null));
            }
        }
    }
}