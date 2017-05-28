using DEA.Common.Items;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Common.Preconditions
{
    public class Own : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider serviceProvider)
        {
            if (!(value is Item item))
            {
                return Task.FromResult(PreconditionResult.FromError("ERROR: The own attribute may not be used on a parameter that is not an item."));
            }
            else if ((context as DEAContext).DbUser.Inventory.Elements.Any(x => x.Name == item.Name))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError($"You do not own the following item: {item.Name}."));
            }
        }
    }
}
