using DEA.Database.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public class BaseRepository<T> where T : Model
    {
        public IMongoCollection<T> Collection { get; }

        public BaseRepository(IMongoCollection<T> collection)
        {
            Collection = collection;
        }

        public Task InsertAsync(T entity)
        {
            return Collection.InsertOneAsync(entity, null, default(CancellationToken));
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(filter).Limit(1).FirstOrDefaultAsync();
        }

        public async Task<List<T>> AllAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
            {
                return await (await Collection.FindAsync(filter)).ToListAsync();
            }
            else
            {
                return await(await Collection.FindAsync(Builders<T>.Filter.Empty)).ToListAsync();
            }
        }

        public Task UpdateAsync(T entity)
        {
            return Collection.ReplaceOneAsync(y => y.Id == entity.Id, entity);
        }
        
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(filter).Limit(1).AnyAsync();
        }

        public Task ModifyAsync(T entity, Action<T> function)
        {
            function(entity);
            return UpdateAsync(entity);
        }

        public async Task ModifyAsync(Expression<Func<T, bool>> filter, Action<T> function)
        {
            var entity = await GetAsync(filter);
            function(entity);
            await UpdateAsync(entity);
        }

        public Task DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return Collection.DeleteOneAsync(filter);
        }
    }
}
