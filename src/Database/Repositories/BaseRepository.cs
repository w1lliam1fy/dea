using DEA.Database.Models;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public class BaseRepository<T> where T : Model
    {
        public IMongoCollection<T> _collection { get; }

        public BaseRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public async Task<T> FetchAsync(Expression<Func<T, bool>> filter)
        {
            return await (await _collection.FindAsync(filter)).SingleOrDefaultAsync();
        }

        public Task UpdateAsync(T entity)
        {
            return _collection.ReplaceOneAsync(y => y.Id == entity.Id, entity);
        }

        public async Task ModifyAysnc(Expression<Func<T, bool>> filter, Action<T> function)
        {
            var entity = await FetchAsync(filter);
            function(entity);
            await UpdateAsync(entity);
        }

        public Task DeleteAsync(T entity)
        {
            return _collection.DeleteOneAsync(y => y == entity);
        }
    }
}
