using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Practice_project1.Models
{
    public class CommentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string postId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? userId { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
