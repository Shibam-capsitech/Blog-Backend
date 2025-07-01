using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Practice_project1.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public required string Username { get; set; }

        [BsonElement("email")]
        public required string Email { get; set; }

        [BsonElement("password")]
        public required string Password { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("age")]
        public int Age { get; set; }

        [BsonElement("city")]
        public string? City { get; set; }

        [BsonElement("likedPosts")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> LikedPosts { get; set; }


    }
}
