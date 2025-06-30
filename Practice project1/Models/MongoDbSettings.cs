namespace Practice_project1.Models;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string PersonCollectionName { get; set; } = null!;
    public string PostsCollectionName { get; set; }= null!;
    public string CommentsCollectionName { get; set; } = null!;
}

