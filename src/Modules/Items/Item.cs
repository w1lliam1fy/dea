using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;
using DEA.Common.Items;
using System.Reflection;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Item")]
        [Remarks("Kitchen Knife")]
        [Summary("Get all the information on any item.")]
        public async Task Item([Remainder] Item item)
        {
            var message = $"**Description:** {item.Description}\n";

            foreach (var property in item.GetType().GetProperties())
            {
                if (property.Name == "Description" || property.Name == "Name")
                {
                    continue;
                }

                var value =  property.GetValue(item);
                message += $"**{property.Name.SplitCamelCase()}:** {(value is decimal ? ((decimal)value).USD() : value.ToString())}\n";
            }

            await SendAsync(message, item.Name);
        }
    }
}
