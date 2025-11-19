
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Net.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register an HttpClient for TMDB calls
builder.Services.AddHttpClient("tmdb", c =>
{
    c.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});

var app = builder.Build();

string TMDBKey = Environment.GetEnvironmentVariable("TMDBKey") ?? "your_tmdb_key_here";

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

app.MapPost("/reviews", async (CreateReview myReview) =>
{
    if (string.IsNullOrWhiteSpace(myReview.DisplayName))
        return Results.BadRequest(new { error = "Name is required" });
    if (string.IsNullOrWhiteSpace(myReview.Title))
        return Results.BadRequest(new { error = "Title is required" });
    if (myReview.Rating < 1 || myReview.Rating > 10)
        return Results.BadRequest(new { error = "Rating must be between 1 and 10" });
    if (myReview.TmdbId <= 0)
        return Results.BadRequest(new { error = "TMDB ID must be a positive integer" });

    var review = new Review
    {
        Id = ObjectId.GenerateNewId().ToString(),
        DisplayName = myReview.DisplayName.Trim(),
        Title = myReview.Title.Trim(),
        Text = myReview.Text.Trim(),
        TmdbId = myReview.TmdbId,
        Rating = myReview.Rating,
        CreatedAt = DateTime.UtcNow
    };

    await reviewsCollection.InsertOneAsync(review);

    return Results.Created($"/reviews/{review.Id}", review);
});

// TMDB lookup endpoint - returns movie title and banner (backdrop) URL
app.MapGet("/tmdb/{id:int}", async (int id, IHttpClientFactory httpFactory) =>
{
    if (string.IsNullOrWhiteSpace(TMDBKey) || TMDBKey == "your_tmdb_key_here")
        return Results.StatusCode(500);

    var client = httpFactory.CreateClient("tmdb");

    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TMDBKey);

    var resp = await client.GetAsync($"movie/{id}");
    if (!resp.IsSuccessStatusCode)
    {
        // Read TMDB response body (if any) for additional detail
        string tmdbBody = string.Empty;
        try
        {
            tmdbBody = await resp.Content.ReadAsStringAsync();
        }
        catch { /* ignore read errors */ }

        // Return more descriptive errors depending on TMDB status code
        var statusCode = resp.StatusCode;
        if (statusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Results.NotFound(new { error = "TMDB: movie not found", tmdbStatus = (int)statusCode });
        }
        if (statusCode == System.Net.HttpStatusCode.Unauthorized || statusCode == System.Net.HttpStatusCode.Forbidden)
        {
            return Results.Json(new { error = "TMDB: unauthorized - invalid or missing TMDB token", tmdbStatus = (int)statusCode, details = tmdbBody }, statusCode: 502);
        }
        if ((int)statusCode == 429)
        {
            return Results.Json(new { error = "TMDB: rate limit exceeded", tmdbStatus = (int)statusCode, details = tmdbBody }, statusCode: 502);
        }

        // Generic TMDB failure
        return Results.Json(new { error = $"TMDB error: {(int)statusCode} {statusCode}", tmdbStatus = (int)statusCode, details = tmdbBody }, statusCode: 502);
    }

    using var stream = await resp.Content.ReadAsStreamAsync();
    using var doc = await JsonDocument.ParseAsync(stream);

    var root = doc.RootElement;
    string title = root.GetProperty("title").GetString() ?? string.Empty;

    // Prefer backdrop_path; fall back to poster_path
    string? path = null;
    if (root.TryGetProperty("backdrop_path", out var backdrop) && backdrop.ValueKind != JsonValueKind.Null)
        path = backdrop.GetString();
    else if (root.TryGetProperty("poster_path", out var poster) && poster.ValueKind != JsonValueKind.Null)
        path = poster.GetString();

    string? bannerUrl = null;
    if (!string.IsNullOrWhiteSpace(path))
    {
        // Use TMDB image base URL with a reasonable size for banners
        bannerUrl = $"https://image.tmdb.org/t/p/w780{path}";
    }

    return Results.Ok(new { MovieName = title, BannerUrl = bannerUrl });
});

app.Run();

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

public class CreateReview
{
    public string DisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int TmdbId { get; set; } = 0;
    public int Rating { get; set; } = 0;
}



