using System.Net.Http.Json;

namespace SAMsa.Services
{
    public class OmdbService
    {
        private readonly HttpClient _http;
        private readonly string? _apiKey;

        public OmdbService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Omdb:ApiKey"]; // OMDB key should be set under Omdb:ApiKey
            _http.BaseAddress = new Uri("http://www.omdbapi.com/");
        }

        public async Task<object?> SearchAsync(string query, int page = 1, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_apiKey)) return null;
            var url = $"?apikey={_apiKey}&s={Uri.EscapeDataString(query)}&page={page}";
            var res = await _http.GetFromJsonAsync<object>(url, ct);
            return res;
        }

        public async Task<object?> GetDetailsAsync(string imdbIdOrTitle, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_apiKey)) return null;
            // OMDB supports i= (imdb id) or t= (title)
            var url = $"?apikey={_apiKey}&i={Uri.EscapeDataString(imdbIdOrTitle)}&plot=full";
            var res = await _http.GetFromJsonAsync<object>(url, ct);
            return res;
        }
    }
}
