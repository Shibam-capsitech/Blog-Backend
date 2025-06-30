using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Practice_project1.Dto
{
    public class CommentDto
    {
        public string postId { get; set; }
        public string content {  get; set; }
    }
    public class CommentForPostDetailsDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [BsonElement("user")]
        public UserDto? User { get; set; }
    }
}
