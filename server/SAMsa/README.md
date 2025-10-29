# SAMsa - Simple Movie Social Backend

This is a minimal ASP.NET Core Web API backend for a simple social app centered on movies/TV shows. It provides:

- Posts (reviews) with optional TMDB linkage
- Replies to posts
- TMDB proxy endpoints for search and details (requires TMDB API key)

Configuration
-------------

Set your OMDB API key in configuration (appsettings.Development.json, appsettings.json, or user secrets) under:

```
{
  "Omdb": {
    "ApiKey": "YOUR_OMDB_API_KEY"
  }
}
```

If the API key is not configured, OMDB endpoints will return 400 with a message.

Run
---

From the project folder:

```bash
cd /home/jacky/Dev/sam/server/SAMsa
dotnet run --urls "http://localhost:5005"
```

API Endpoints
-------------

- GET /api/posts — list posts
- GET /api/posts/{id} — get single post with replies
- POST /api/posts — create a post (JSON body: Author, Title, Content, Rating, TmdbId)
- PUT /api/posts/{id} — update a post
- DELETE /api/posts/{id} — delete a post

- GET /api/posts/{postId}/replies — list replies
- POST /api/posts/{postId}/replies — create reply (JSON body: Author, Content)
- DELETE /api/posts/{postId}/replies/{id} — delete reply

- GET /api/omdb/search?q=some+query — search OMDB (requires API key)
- GET /api/omdb/details?id={imdbIdOrTitle} — get OMDB details by imdb id or title

Examples
--------

Create a post:

```bash
curl -X POST http://localhost:5005/api/posts \
  -H "Content-Type: application/json" \
  -d '{"author":"Alice","title":"Dune (2021)","content":"Great visuals!","rating":8}'
```

Search OMDB:

```bash
curl "http://localhost:5005/api/omdb/search?q=dune"
```

Notes
-----
- Uses EF Core InMemory for simplicity. For production, swap to a persistent provider such as PostgreSQL or SQL Server.
- Swagger is enabled in development.
