using DEA.Common.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

namespace DEA.Common.TypeReaders
{
    internal sealed class GunTypeReader : TypeReader
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Gun[] _guns;

        public GunTypeReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _guns = _serviceProvider.GetService<Gun[]>();
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                var gun = _guns.FirstOrDefault(x => x.Name.ToLower() == input);

                if (gun != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(gun));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not a gun."));
            });
        }
    }
}