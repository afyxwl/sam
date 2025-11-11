
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

string mongoConn = "mongodb://localhost:27017";
string mongoDbName = "movie_reviews_db";
string mongoCollectionName = "reviews";

var mongoClient = new MongoClient(mongoConn);
var database = mongoClient.GetDatabase(mongoDbName);
var reviewsCollection = database.GetCollection<Review>(mongoCollectionName);

app.MapGet("/", () => Results.Ok(new { message = "Movie Reviews API (MongoDB) - GET /reviews, POST /reviews" }));

app.MapGet("/reviews", async () =>
{
    var list = await reviewsCollection.Find(Builders<Review>.Filter.Empty)
                                      .SortByDescending(r => r.CreatedAt)
                                      .ToListAsync();
    return Results.Ok(list);
});

app.MapPost("/reviews", async (ReviewCreate dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.DisplayName))
        return Results.BadRequest(new { error = "DisplayName is required" });
    if (string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest(new { error = "Title is required" });
    if (dto.Rating < 1 || dto.Rating > 10)
        return Results.BadRequest(new { error = "Rating must be between 1 and 10" });
    if (dto.TmdbId <= 0)
        return Results.BadRequest(new { error = "TMDB/IMDB ID must be a positive integer" });

    var review = new Review
    {
        Id = ObjectId.GenerateNewId().ToString(),
        DisplayName = dto.DisplayName.Trim(),
        Title = dto.Title.Trim(),
        Text = dto.Text.Trim(),
        TmdbId = dto.TmdbId,
        Rating = dto.Rating,
        CreatedAt = DateTime.UtcNow
    };

    await reviewsCollection.InsertOneAsync(review);

    return Results.Created($"/reviews/{review.Id}", review);
});

app.Run();

// Models
public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int TmdbId { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReviewCreate
{
    public string DisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int TmdbId { get; set; } = 0;
    public int Rating { get; set; } = 0;
}



