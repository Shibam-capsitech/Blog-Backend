using Microsoft.OpenApi.Any;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Practice_project1.Dto;
using Practice_project1.Models;
using System.ComponentModel;

namespace Practice_project1.Service
{
    public class PostService
    {
        private readonly IMongoCollection<Post> _post;
        private readonly IMongoCollection<CommentModel> _comment;

        public PostService(IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = new MongoClient(settings?.ConnectionString);
            var database = client.GetDatabase(settings?.DatabaseName);
            _post = database.GetCollection<Post>(settings?.PostsCollectionName);
            _comment = database.GetCollection<CommentModel>(settings?.CommentsCollectionName);
        }

        public async Task CreateAsyncPost (Post post)
        {
            await _post.InsertOneAsync(post);
        }

        public async Task<List<object>> GetAllPostAsync()
        {

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "CRUD" },
            { "localField", "userId" },
            { "foreignField", "_id" },
            { "as", "user" }
        }),
        new BsonDocument("$addFields", new BsonDocument
        {
            { "user", new BsonDocument("$arrayElemAt", new BsonArray { "$user", 0 }) }
        }),
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 1 },
            { "title", 1 },
            { "description", 1 },
            { "CreatedAt", 1 },
            { "user._id", 1 },
            { "user.username", 1 },
            { "user.email", 1 },
            { "user.name", 1 },
            { "user.age", 1 },
            { "user.city", 1 }
        })
            };

            var results = await _post.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var postList = new List<object>();

            foreach (var doc in results)
            {
                var user = doc["user"].AsBsonDocument;
                var post = new
                {
                    _id = doc["_id"].AsObjectId.ToString(),
                    title = doc["title"].AsString,
                    description = doc["description"].AsString,
                    CreatedAt = doc["CreatedAt"].ToUniversalTime(),
                    user = new
                    {
                        _id = user["_id"].AsObjectId.ToString(),
                        username = user["username"].AsString,
                        email = user["email"].AsString,
                        name = user["name"].AsString,
                        age = user["age"].AsInt32,
                        city = user.Contains("city") ? user["city"].AsString : null
                    }
                };

                postList.Add(post);
            }

            return postList;
        }

        public async Task<Object> GetPostById(string postId)
        {
            var objectId = ObjectId.Parse(postId);

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match", new BsonDocument("_id", objectId)),

        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "CRUD" },
            { "localField", "userId" },
            { "foreignField", "_id" },
            { "as", "user" }
        }),
        new BsonDocument("$addFields", new BsonDocument
        {
            { "user", new BsonDocument("$arrayElemAt", new BsonArray { "$user", 0 }) }
        }),

        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "COMMENT" },
            { "let", new BsonDocument("postId", "$_id") },
            { "pipeline", new BsonArray
                {
                    new BsonDocument("$match", new BsonDocument
                    {
                        { "$expr", new BsonDocument("$eq", new BsonArray { "$postId", "$$postId" }) }
                    }),
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "CRUD" },
                        { "localField", "userId" },
                        { "foreignField", "_id" },
                        { "as", "user" }
                    }),
                    new BsonDocument("$addFields", new BsonDocument
                    {
                        { "user", new BsonDocument("$arrayElemAt", new BsonArray { "$user", 0 }) }
                    })
                }
            },
            { "as", "comment" }
        }),

        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 1 },
            { "title", 1 },
            { "description", 1 },
            { "CreatedAt", 1 },
            { "user._id", 1 },
            { "user.username", 1 },
            { "user.email", 1 },
            { "user.name", 1 },
            { "user.age", 1 },
            { "user.city", 1 },
            { "comment._id", 1 },
            { "comment.content", 1 },          
            { "comment.CreatedAt", 1 },    
            { "comment.user._id", 1 },
            { "comment.user.username", 1 },
            { "comment.user.email", 1 },
            { "comment.user.name", 1 },
            { "comment.user.age", 1 },
            { "comment.user.city", 1 }
        })
            };

            var doc = await _post.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            var postDetails = BsonSerializer.Deserialize<GetPOstByIdDto>(doc);
            //if (doc == null) return null;

            //var user = doc.GetValue("user", null)?.AsBsonDocument;
            //var comments = doc.GetValue("comment", new BsonArray()).AsBsonArray;

            //var post = new
            //{
            //    _id = doc["_id"].AsObjectId.ToString(),
            //    title = doc.GetValue("title", "").AsString,
            //    description = doc.GetValue("description", "").AsString,
            //    CreatedAt = doc.GetValue("CreatedAt", BsonNull.Value).IsBsonNull ? (DateTime?)null : doc["CreatedAt"].ToUniversalTime(),
            //    user = user == null ? null : new
            //    {
            //        _id = user["_id"].AsObjectId.ToString(),
            //        username = user.GetValue("username", "").AsString,
            //        email = user.GetValue("email", "").AsString,
            //        name = user.GetValue("name", "").AsString,
            //        age = user.GetValue("age", 0).ToInt32(),
            //        city = user.Contains("city") ? user["city"].AsString : null
            //    },
            //    comment = comments.Select(c =>
            //    {
            //        var cDoc = c.AsBsonDocument;
            //        var cUser = cDoc.GetValue("user", null)?.AsBsonDocument;
            //        return new
            //        {
            //            _id = cDoc["_id"].AsObjectId.ToString(),
            //            text = cDoc.GetValue("content", "").AsString,
            //            CreatedAt = cDoc.GetValue("CreatedAt", BsonNull.Value).IsBsonNull ? (DateTime?)null : cDoc["CreatedAt"].ToUniversalTime(),
            //            user = cUser == null ? null : new
            //            {
            //                _id = cUser["_id"].AsObjectId.ToString(),
            //                username = cUser.GetValue("username", "").AsString,
            //                email = cUser.GetValue("email", "").AsString,
            //                name = cUser.GetValue("name", "").AsString,
            //                age = cUser.GetValue("age", 0).ToInt32(),
            //                city = cUser.Contains("city") ? cUser["city"].AsString : null
            //            }
            //        };
            //    }).ToList()
            //};




            return postDetails;
        }


        public async Task UpdatePostAsync(string postId, Post updatedPost)
        {
            await _post.FindOneAndReplaceAsync(post => post.Id == postId, updatedPost);
        }

        public async Task DeletePostAsync (string postId)
        {
            await _post.DeleteOneAsync(post => post.Id == postId);
        }

        public async Task<List<object>> GetAllComments(string postId)
        {
            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match", new BsonDocument("postId", new ObjectId(postId))),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "COMMENT" },
            { "let", new BsonDocument("postId", "$_id") },
            { "pipeline", new BsonArray
                {
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "CRUD" },
                        { "localField", "userId" },
                        { "foreignField", "_id" },
                        { "as", "user" }
                    }),
                    new BsonDocument("$addFields", new BsonDocument
                    {
                        { "user", new BsonDocument("$arrayElemAt", new BsonArray { "$user", 0 }) }
                    }),
                    new BsonDocument("$project", new BsonDocument
                    {
                        { "_id", 1 },
                        { "content", 1 },
                        { "CreatedAt", 1 },
                        { "user._id", 1 },
                        { "user.username", 1 },
                        { "user.email", 1 },
                        { "user.name", 1 },
                        { "user.age", 1 },
                        { "user.city", 1 }
                    })
                }
            },
            { "as", "comments" }
        })
            };

            var result = await _post.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            var commentsList = new List<object>();

            if (result != null && result.Contains("comments"))
            {
                foreach (var comment in result["comments"].AsBsonArray)
                {
                    var c = comment.AsBsonDocument;

                    var user = c["user"].AsBsonDocument;

                    var commentObj = new
                    {
                        _id = c["_id"].AsObjectId.ToString(),
                        content = c["content"].AsString,
                        CreatedAt = c["CreatedAt"].ToUniversalTime(),
                        user = new
                        {
                            _id = user["_id"].AsObjectId.ToString(),
                            username = user["username"].AsString,
                            email = user["email"].AsString,
                            name = user["name"].AsString,
                            age = user["age"].AsInt32,
                            city = user.Contains("city") ? user["city"].AsString : null
                        }
                    };

                    commentsList.Add(commentObj);
                }
            }

            return commentsList;
        }


    }
}



//using Practice_project1.Models;

//public void Update(string id, UserModel personIn) => _user.ReplaceOne(p => p.Id == id, personIn);
//public void Remove(string id) => _user.DeleteOne(p => p.Id == id);\

