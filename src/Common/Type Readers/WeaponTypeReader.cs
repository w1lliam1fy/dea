using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Services.Static;

namespace DEA.Common.TypeReaders
{
    internal sealed class WeaponTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                var weapon = Data.Weapons.FirstOrDefault(x => x.Name.ToLower() == input);

                if (weapon != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(weapon));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not a weapon."));
            });
        }
    }
}