using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Services.Static;

namespace DEA.Common.TypeReaders
{
    internal sealed class GunTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                var gun = Data.Guns.FirstOrDefault(x => x.Name.ToLower() == input);

                if (gun != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(gun));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not a gun."));
            });
        }
    }
}