using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Data;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DEA.Modules.NSFW
{
    public partial class NSFW
    {
        [Command("Tits")]
        [Alias("titties", "tities", "boobs", "boob")]
        [RequireNsfw]
        [Summary("Motorboat that shit.")]
        public async Task Tits()
        {
            using (var http = new HttpClient())
            {
                var obj = JArray.Parse(await http.GetStringAsync($"http://api.oboobs.ru/boobs/{Config.RAND.Next(0, 10330)}"))[0];
                await Context.Channel.SendMessageAsync($"http://media.oboobs.ru/{obj["preview"]}");
            }
        }
    }
}
