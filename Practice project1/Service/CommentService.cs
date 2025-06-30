using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Practice_project1.Models;
using System.Runtime.InteropServices;

namespace Practice_project1.Service
{
    public class CommentService
    {
        private readonly IMongoCollection<CommentModel> _comment;

        public CommentService(IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = new MongoClient(settings?.ConnectionString);
            var database = client.GetDatabase(settings?.DatabaseName);
            _comment = database.GetCollection<CommentModel>(settings?.CommentsCollectionName);
        }

        public async Task CreateComment(CommentModel comment)
        {
            await _comment.InsertOneAsync(comment);
        }

        public async Task DeleteComment(string id)
        {
            await _comment.DeleteOneAsync(comment => comment.Id == id);
        }

        //public async Task<List<BsonDocument>> GetCommentsWithPostDetails(string postId)
        //{
        //    var objectId = new ObjectId(postId);
        //    var pipeline = new BsonDocument[]
        //    {
        //new BsonDocument("$match", new BsonDocument("postId", objectId)),
        //new BsonDocument("$lookup", new BsonDocument
        //{
        //    { "from", "posts" },
        //    { "localField", "postId" },
        //    { "foreignField", "_id" },
        //    { "as", "postDetails" }
        //})
        //    };

        //    return await _comment.Aggregate<BsonDocument>(pipeline).ToListAsync();
        //}

    }
}
