using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DEA.Database.Models
{
    public partial class Model
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
