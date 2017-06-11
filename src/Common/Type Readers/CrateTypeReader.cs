using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Services.Static;

namespace DEA.Common.TypeReaders
{
    internal sealed class CrateTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                input = input.EndsWith("crate") ? input : input + " crate";

                var crate = Data.Crates.FirstOrDefault(x => x.Name.ToLower() == input);

                if (crate != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(crate));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not a crate."));
            }); 
        }
    }
}