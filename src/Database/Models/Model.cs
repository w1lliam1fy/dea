using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DEA.Database.Models
{
    public abstract partial class Model
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
