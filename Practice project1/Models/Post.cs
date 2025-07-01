using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Practice_project1.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public required string title { get; set; }

        [BsonElement("description")]
        public required string description { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? userId { get; set; }

        [BsonElement("imgUrl")]
        public string? imgUrl { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
