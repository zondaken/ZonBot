using System.Diagnostics;
using System.Net;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZonBot.Modules.SlashCommands
{
    public class GifModule : InteractionModuleBase<SocketInteractionContext<SocketInteraction>>
    {
        private string _uri;
        private string _key;

        public GifModule()
        {
            _uri = "https://g.tenor.com/v1/search?q={0}&key={1}&limit={2}";

            var file = Path.Combine(Helpers.ConfigPath, "gifsettings.json");
            var json = File.ReadAllText(file);
            //var definition = new {tenor_key = ""};
            //_key = (JsonConvert.DeserializeAnonymousType(json, definition) ?? definition).tenor_key;
            _key = (string?)JObject.Parse(json)["tenor_key"] ?? string.Empty;
        }
        
        private Uri MakeUri(string query, int limit)
        {
            return new Uri(string.Format(_uri, query, _key, limit.ToString()));
        }

        private async Task<JObject> MakeJson(string query, int limit)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(MakeUri(query, limit));
            
            return JObject.Parse(response);
        }

        private async Task<List<string>?> MakeGifList(string query, int limit)
        {
            List<string> gifList = new List<string>();
            
            var json = await MakeJson(query, limit);

            var allEntries = json["results"];
            if (allEntries == null) return null;

            foreach (var entry in json["results"]!)
            {
                gifList.Add((string?)entry["url"] ?? string.Empty);
            }

            return gifList;
        }
        
        [SlashCommand("gif", "Search gif on tenor")]
        public async Task GifAsync(string query)
        {
            var limit = 50;

            List<string>? gifList = await MakeGifList(query, limit);
            if (gifList == null) return;

            var index = new Random().Next(0, limit);
            await RespondAsync(gifList[index]);
        }
    }
}