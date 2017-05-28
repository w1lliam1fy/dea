using DEA.Common.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

namespace DEA.Common.TypeReaders
{
    internal sealed class ItemTypeReader : TypeReader
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Item[] _items;

        public ItemTypeReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _items = _serviceProvider.GetService<Item[]>();
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            input = input.ToLower();

            var item = _items.FirstOrDefault(x => x.Name.ToLower() == input);

            if (item != null)
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(item));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item does not exist."));
        }
    }
}