using DEA.Database.Models;
using MongoDB.Bson;
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
        private readonly IMongoCollection<T> _collection;

        public BaseRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public Task InsertAsync(T entity)
        {
            return _collection.InsertOneAsync(entity, null, default(CancellationToken));
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).Limit(1).FirstOrDefaultAsync();
        }

        public async Task<List<T>> AllAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
            {
                return await (await _collection.FindAsync(filter)).ToListAsync();
            }
            else
            {
                return await(await _collection.FindAsync(Builders<T>.Filter.Empty)).ToListAsync();
            }
        }

        public Task UpdateAsync(T entity)
        {
            return _collection.ReplaceOneAsync(y => y.Id == entity.Id, entity);
        }
        
        public Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).Limit(1).AnyAsync();
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

        public Task PushAsync(Expression<Func<T, bool>> filter, FieldDefinition<T> field, BsonValue value)
        {
            return _collection.UpdateOneAsync(filter, Builders<T>.Update.Push(field, value));
        }

        public Task PullAsync(Expression<Func<T, bool>> filter, FieldDefinition<T> field, BsonValue value)
        {
            return _collection.UpdateOneAsync(filter, Builders<T>.Update.Pull(field, value));
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null)
            {
                return _collection.CountAsync(Builders<T>.Filter.Empty);
            }
            else
            {
                return _collection.CountAsync(filter);
            }
        }

        public Task DeleteAsync(T entity)
        {
            return _collection.DeleteOneAsync(y => y.Id == entity.Id);
        }

        public Task DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return _collection.DeleteManyAsync(filter);
        }
    }
}
