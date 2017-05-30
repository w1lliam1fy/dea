using Discord.Commands;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DEA.Modules.NSFW
{
    public partial class NSFW
    {
        [Command("Ass")]
        [Alias("butt", "butts", "booty")]
        [Summary("Sauce me some booty how about that.")]
        public async Task Ass()
        {
            using (var http = new HttpClient())
            {
                var obj = JArray.Parse(await http.GetStringAsync($"http://api.obutts.ru/butts/{Config.Random.Next(4731)}"))[0];
                await Context.Channel.SendMessageAsync($"http://media.obutts.ru/{obj["preview"]}");
            }
        }
    }
}
