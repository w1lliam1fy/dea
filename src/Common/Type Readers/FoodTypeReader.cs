using DEA.Common.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

namespace DEA.Common.TypeReaders
{
    internal sealed class FoodTypeReader : TypeReader
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Food[] _food;

        public FoodTypeReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _food = _serviceProvider.GetService<Food[]>();
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                var food = _food.FirstOrDefault(x => x.Name.ToLower() == input);

                if (food != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(food));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not edible."));
            });
        }
    }
}