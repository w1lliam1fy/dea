using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;
using DEA.Common.Items;
using System.Reflection;
using System;
using DEA.Services.Static;

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
                var name = property.Name.SplitCamelCase();

                if (name == "Description" || name == "Name")
                {
                    continue;
                }

                var value = property.GetValue(item);

                if (value is decimal newValue)
                {
                    message += $"**{name}:** {newValue.USD()}\n";
                }
                else if (name == "Crate Odds")
                {
                    message += $"**{name}:** {(Convert.ToSingle(value) / Data.CrateItemOdds).ToString("P")}\n";
                }
                else if (name == "Acquire Odds" && property.PropertyType == typeof(Fish))
                {
                    message += $"**{name}:** {(Convert.ToSingle(value) / Data.FishAcquireOdds).ToString("P")}\n";
                }
                else if (name == "Acquire Odds" && property.PropertyType == typeof(Meat))
                {
                    message += $"**{name}:** {(Convert.ToSingle(value) / Data.MeatAcquireOdds).ToString("P")}\n";
                }
                else
                {
                    message += $"**{name}:** {value.ToString()}\n";
                } 
            }

            await SendAsync(message, item.Name);
        }
    }
}
