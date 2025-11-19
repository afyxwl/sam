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
      "text": "Loved the visuals and story.",
      "tmdbId": 12345,
      "rating": 9
    }
  - Validation: `displayName` required; `tmdbId` must be > 0; `rating` must be 1..10
  - Behavior: When a client POSTs a review the server will first attempt to fetch movie metadata from TMDB using the provided `tmdbId`. If the TMDB lookup succeeds the server stores the movie title and a poster URL on the created review. If the TMDB lookup fails the server will NOT create the review and will return an error.
  - Responses:
    - 201 Created: created review JSON (includes `id`, `createdAt`, `movieName`, and `posterUrl`)
    - 400 Bad Request: { "error": "..." } on validation failure or if TMDB reports NotFound for the provided `tmdbId`.
    - 502 Bad Gateway: returned when TMDB returns an upstream failure (unauthorized, rate-limited, or other errors). The response body includes `tmdbStatus` and optional `details` from TMDB.
    - 500 Internal Server Error: returned when the server is not configured with a valid `TMDBKey` environment variable.

- GET /tmdb/{id}
  - NOTE: This endpoint has been removed. TMDB lookups are performed automatically when creating a review with `POST /reviews`.

Review object shape (returned by GET /reviews and POST response):
{
  "id": "<objectId>",
  "displayName": "Alice",
  "text": "...",
  "tmdbId": 12345,
  "rating": 9,
  "movieName": "Movie Title",
  "posterUrl": "https://image.tmdb.org/t/p/w500/....",
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
- TMDB lookups are performed during `POST /reviews` and the standalone `/tmdb/{id}` endpoint is no longer available.
