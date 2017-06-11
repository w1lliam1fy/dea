using Discord.Commands;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using DEA.Services.Static;

namespace DEA.Modules.NSFW
{
    public partial class NSFW
    {
        [Command("Tits")]
        [Alias("titties", "tities", "boobs", "boob")]
        [Summary("Motorboat that shit.")]
        public async Task Tits()
        {
            using (var http = new HttpClient())
            {
                var obj = JArray.Parse(await http.GetStringAsync($"http://api.oboobs.ru/boobs/{CryptoRandom.Next(10738)}"))[0];
                await Context.Channel.SendMessageAsync($"http://media.oboobs.ru/{obj["preview"]}");
            }
        }
    }
}
