using DEA.Common.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

namespace DEA.Common.TypeReaders
{
    internal sealed class WeaponTypeReader : TypeReader
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Weapon[] _weapons;

        public WeaponTypeReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _weapons = _serviceProvider.GetService<Weapon[]>();
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            input = input.ToLower();

            var weapon = _weapons.FirstOrDefault(x => x.Name.ToLower() == input);

            if (weapon != null)
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(weapon));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not a weapon."));
        }
    }
}