using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Data;
using System.Xml;
using System.Net.Http;

namespace DEA.Modules.NSFW
{
    public partial class NSFW
    {
        [Command("Hentai")]
        [Remarks("boob")]
        [Summary("The real shit goes down with custom hentai tags.")]
        public async Task Gelbooru([Remainder] string tag = "")
        {
            tag = tag?.Replace(" ", "_");

            using (var http = new HttpClient())
            {
                var data = await http.GetStreamAsync($"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}");
                var doc = new XmlDocument();
                doc.Load(data);

                var node = doc.LastChild.ChildNodes[Config.RAND.Next(0, doc.LastChild.ChildNodes.Count)];
                if (node == null)
                {
                    ReplyError("No result found.");
                }

                var url = node.Attributes["file_url"].Value;

                if (!url.StartsWith("http"))
                {
                    url = "https:" + url;
                }

                await Context.Channel.SendMessageAsync(url);
            }
        }
    }
}
