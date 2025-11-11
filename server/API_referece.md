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
- CORS is enabled (AllowAnyOrigin) for browser clients.
- MongoDB connection can be configured with environment variables:
  - `MONGO_CONNECTION` (default: mongodb://localhost:27017)
  - `MONGO_DB` (default: movie_reviews_db)
  - `MONGO_COLLECTION` (default: reviews)
- The server listens on the URL(s) shown when you run it; examples used in tests: http://localhost:5096
