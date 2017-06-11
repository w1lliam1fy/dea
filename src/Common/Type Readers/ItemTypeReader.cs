using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Services.Static;

namespace DEA.Common.TypeReaders
{
    internal sealed class ItemTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                var item = Data.Items.FirstOrDefault(x => x.Name.ToLower() == input);

                if (item != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(item));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item does not exist."));
            });
        }
    }
}