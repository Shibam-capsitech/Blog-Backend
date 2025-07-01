using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Practice_project1.Dto
{
    public class CreatePostDto
    {
        public required string title { get; set; }
        public required string description { get; set; }
    }
    public class UpdatePostDto
    {
        public required string title { get; set; }
        public required string description { get; set; }
    }

    public class GetPOstByIdDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("imgUrl")]
        public string? ImageUrl { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; } 

        [BsonElement("user")]
        public UserDto? User { get; set; }

        [BsonElement("comment")]
        public List<CommentForPostDetailsDto> Comment { get; set; }
    }

}

