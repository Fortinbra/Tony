using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public abstract class Base
    {
        [BsonId]
        Guid Id { get; set; } = Guid.NewGuid();
    }
}