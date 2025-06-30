using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Practice_project1.Models;

namespace Practice_project1.Service
{
    public class CityUserCount
    {
        public string City { get; set; } = null!;
        public int UserCount { get; set; }
    }
    public class UserPostSummary
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int PostCount { get; set; }
    }
    public class UserService
    {
        private readonly IMongoCollection<UserModel> _user;
        private readonly IMongoCollection<Post> _post;
        public UserService(IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = new MongoClient(settings?.ConnectionString);
            var database = client.GetDatabase(settings?.DatabaseName);
            _user = database.GetCollection<UserModel>(settings?.PersonCollectionName);
            _post = database.GetCollection<Post>(settings?.PostsCollectionName);
        }

        //public List<UserModel> Get() => _user.~;

        //public UserModel Get(string id) => _user.Find(p => p.Id == id).FirstOrDefault();

        //public void Create(UserModel person)
        //{
        //    var user = _user.Find(p => p.Username == person.Username).FirstOrDefault();
        //    if (user!=null)
        //    {
        //        return;
        //    }

        //    const int workFactor = 10;
        //    person.Password = BCrypt.Net.BCrypt.HashPassword(person.Password, workFactor);
        //    _user.InsertOne(person);
        //}

        //public void Login(string username , string password)
        //{
        //    var user
        //}

        public async Task<UserModel> GetByUserNameAsync(string username)
        {
            return await _user.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(UserModel user)
        {
            await _user.InsertOneAsync(user);
        }

        public async Task<UserModel> getUserByUserIDAsync(string id)
        {
            return await _user.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task updateUserAsync(UserModel updatedUser)
        {
            await _user.ReplaceOneAsync(u => u.Id == updatedUser.Id, updatedUser);
        }

        public async Task<UserModel> getUserDetails(string id)
        {
            return await _user.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        // Aggregation practice 
        public async Task<int> CountUsersInNewYorkOver25Async()
        {
            var pipeline = new[]
            {
                new BsonDocument("$match",
                new BsonDocument {
                { "city", "New York" },
                { "age", new BsonDocument("$gt", 25) }
                }),
                new BsonDocument("$count","count")
            };

            var result = await _user.AggregateAsync<BsonDocument>(pipeline);

            var count = await result.FirstOrDefaultAsync();
            Console.WriteLine(count);
            return count["count"].AsInt32;
        }

        public async Task<List<CityUserCount>> GroupUserBasedOnCity()
        {
            var pipeline = new[]
            {
             new BsonDocument("$group", new BsonDocument
             {
             { "_id", "$city" },
             { "userCount", new BsonDocument("$sum", 1) }
             })
            };
            var result = await _user.AggregateAsync<BsonDocument>(pipeline);
            var documents = await result.ToListAsync();

            var cityCounts = new List<CityUserCount>();

            foreach (var doc in documents)
            {
                cityCounts.Add(new CityUserCount
                {
                    City = doc["_id"].AsString,
                    UserCount = doc["userCount"].AsInt32
                });
            }

            return cityCounts;
        }

        public async Task<List<UserModel>> GetUserByDecAge()
        {
            var pipeline = new[]
            {
                new BsonDocument("$sort",
                new BsonDocument("age", -1)
                )
            };
            var result = await _user.AggregateAsync<UserModel>(pipeline);
            return await result.ToListAsync();
        }

        public async Task<List<UserPostSummary>> GetActiveUsersFromNewYork()
        {
            //var result = await _user.Aggregate()
            //    .Match(u => u.City == "New York" && u.Age > 25)
            //    .Lookup(
            //        foreignCollection: _post,
            //        localField: "_id",
            //        foreignField: "userId",
            //        @as: "posts"
            //    )
            //    .Unwind<UserModel, BsonDocument>("posts", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            //    .Group(
            //        key => new { Id = key["_id"], Name = key["name"] },
            //        g => new
            //        {
            //            UserId = g.Key.Id,
            //            Name = g.Key.Name,
            //            PostCount = g.Count()
            //        }
            //    )
            //    .SortByDescending(u => u.PostCount)
            //    .ToListAsync();
            var pipeline = new[]
            {
                   new BsonDocument("$match", new BsonDocument
                   {
                       { "city", "New York" },
                       { "age", new BsonDocument("$gt", 20) }
                   }),
                   new BsonDocument("$lookup", new BsonDocument
                   {
                       { "from", "posts" },
                       { "localField", "_id" },
                       { "foreignField", "userId" },
                       { "as", "posts" }
                   }),
                   new BsonDocument("$unwind", new BsonDocument
                   {
                       { "path", "$posts" },
                       { "preserveNullAndEmptyArrays", true }
                   }),
                   new BsonDocument("$group", new BsonDocument
                   {
                       { "_id", new BsonDocument {
                           { "userId", "$_id" },
                           { "name", "$name" }
                       }},
                       { "postCount", new BsonDocument("$sum", 1) }
                   }),
                   new BsonDocument("$sort", new BsonDocument("postCount", -1))
            };                 

            var result = await _user.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var mapped = result.Select(doc => new UserPostSummary
            {
                UserId = doc["_id"]["userId"].ToString(),
                Name = doc["_id"]["name"].ToString(),
                PostCount = doc["postCount"].AsInt32
            }).ToList();
            Console.WriteLine(mapped);
            return mapped;
        }


    }
}
