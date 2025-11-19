# API Reference

All endpoints serve/accept JSON. The server exposes the following endpoints:

- GET /
  - Purpose: Health/info
  - Response: { "message": "Movie Reviews API (MongoDB) - GET /reviews, POST /reviews" }

- GET /reviews
  - Purpose: Return all reviews (most-recent first)
  - Response: 200 OK, JSON array of Review objects

- POST /reviews
  - Purpose: Create a review
  - Request JSON body (example):
    {
      "displayName": "Alice",
      "title": "Great sci-fi",
      "text": "Loved the visuals and story.",
      "tmdbId": 12345,
      "rating": 9
    }
  - Validation: displayName and title required; tmdbId must be > 0; rating must be 1..10
  - Responses:
    - 201 Created: created review JSON (includes `id` and `createdAt`)
    - 400 Bad Request: { "error": "..." } on validation failure

- GET /tmdb/{id}
  - Purpose: Look up movie metadata from TMDB (title and banner/poster).
  - Path param: `id` (TMDB movie id, integer)
  - Notes: Requires the `TMDBKey` environment variable to be set to a TMDB v4 access token. The server sends TMDB requests using the header `Authorization: Bearer <token>`.
  - Response: 200 OK with JSON { "movieName": "...", "posterUrl": "https://..." } (posterUrl may be null if no image available).
  - Failure codes: 404 when TMDB reports NotFound; 502 for upstream TMDB failures (including unauthorized or rate-limited responses); 500 if `TMDBKey` is missing or the placeholder is still present.

Review object shape (returned by GET /reviews and POST response):
{
  "id": "<objectId>",
  "displayName": "Alice",
  "title": "Great sci-fi",
  "text": "...",
  "tmdbId": 12345,
  "rating": 9,
  "createdAt": "2025-11-11T...Z"
}

Notes:
- **MongoDB connection:** The current server code uses hardcoded defaults rather than reading environment variables. The defaults are:
  - connection string: `mongodb://localhost:27017`
  - database: `movie_reviews_db`
  - collection: `reviews`
  To change these values, edit `Program.cs` where the `MongoClient` and database/collection names are set.
- The server listens on the URL(s) shown when you run it; examples used in tests: http://localhost:5096

- TMDB API: configure your TMDB v4 access token in the `TMDBKey` environment variable. The server sends TMDB requests using the header `Authorization: Bearer <token>` (no api_key query param).
