using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public abstract class Base
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}