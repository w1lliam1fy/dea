using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Services.Static;

namespace DEA.Common.TypeReaders
{
    internal sealed class FoodTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                var food = Data.Food.FirstOrDefault(x => x.Name.ToLower() == input);

                if (food != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(food));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not edible."));
            });
        }
    }
}