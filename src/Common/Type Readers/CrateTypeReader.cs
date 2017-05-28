using DEA.Common.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

namespace DEA.Common.TypeReaders
{
    internal sealed class CrateTypeReader : TypeReader
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Crate[] _crates;

        public CrateTypeReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _crates = _serviceProvider.GetService<Crate[]>();
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            input = input.ToLower();

            input = input.EndsWith("crate") ? input : input + " crate";

            var crate = _crates.FirstOrDefault(x => x.Name.ToLower() == input);

            if (crate != null)
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(crate));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not a crate."));
        }
    }
}