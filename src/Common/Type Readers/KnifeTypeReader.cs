using DEA.Common.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

namespace DEA.Common.TypeReaders
{
    internal sealed class KnifeTypeReader : TypeReader
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Knife[] _knives;

        public KnifeTypeReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _knives = _serviceProvider.GetService<Knife[]>();
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                var knife = _knives.FirstOrDefault(x => x.Name.ToLower() == input);

                if (knife != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(knife));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not a knife."));
            });
        }
    }
}