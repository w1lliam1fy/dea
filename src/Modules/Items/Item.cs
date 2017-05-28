using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;
using DEA.Common.Items;
using System.Reflection;
using System;

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

                var value = property.GetValue(item);

                if (value is decimal newValue)
                {
                    message += $"**{property.Name.SplitCamelCase()}:** {newValue.USD()}\n";
                }
                else if (property.Name == "CrateOdds")
                {
                    message += $"**{property.Name.SplitCamelCase()}:** {(Convert.ToSingle(value) / _crateItemOdds).ToString("P")}\n";
                }
                else if (property.Name == "AcquireOdds")
                {
                    message += $"**{property.Name.SplitCamelCase()}:** {(Convert.ToSingle(value) / _foodAcquireOdds).ToString("P")}\n";
                }
                else
                {
                    message += $"**{property.Name.SplitCamelCase()}:** {value.ToString()}\n";
                } 
            }

            await SendAsync(message, item.Name);
        }
    }
}
