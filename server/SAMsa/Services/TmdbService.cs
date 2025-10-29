using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SAMsa.Services
{
    public class TmdbService
    {
        private readonly HttpClient _http;
        private readonly string? _apiKey;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, GuestSessionResult> _localGuests = new();

        public TmdbService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Tmdb:ApiKey"];
            _http.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        }

        public async Task<object?> SearchAsync(string query, int page = 1, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_apiKey)) return null;
            var url = $"search/multi?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}";
            var res = await _http.GetFromJsonAsync<object>(url, ct);
            return res;
        }

        public async Task<object?> GetDetailsAsync(string mediaType, int id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_apiKey)) return null;
            var url = $"{mediaType}/{id}?api_key={_apiKey}";
            var res = await _http.GetFromJsonAsync<object>(url, ct);
            return res;
        }

        /// <summary>
        /// Create or return a guest session. If TMDB API key is available, request a real TMDB guest_session.
        /// Otherwise create a local ephemeral guest session id that can be used by clients.
        /// </summary>
        public async Task<GuestSessionResult> CreateGuestSessionAsync(CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                var url = $"authentication/guest_session/new?api_key={_apiKey}";
                var dto = await _http.GetFromJsonAsync<TmdbGuestSessionDto>(url, ct);
                if (dto != null && dto.Success && dto.GuestSessionId != null)
                {
                    DateTime expires = DateTime.UtcNow.AddHours(24);
                    if (!string.IsNullOrWhiteSpace(dto.ExpiresAt))
                    {
                        if (DateTime.TryParse(dto.ExpiresAt, out var parsed)) expires = parsed.ToUniversalTime();
                    }
                    return new GuestSessionResult { GuestSessionId = dto.GuestSessionId, ExpiresAt = expires, IsLocal = false };
                }
                // fallthrough to local if TMDB returned unexpected data
            }

            // create local ephemeral guest session
            var id = Guid.NewGuid().ToString("N");
            var expiresAt = DateTime.UtcNow.AddHours(24);
            var res = new GuestSessionResult { GuestSessionId = id, ExpiresAt = expiresAt, IsLocal = true };
            _localGuests[id] = res;
            return res;
        }
    }
}
