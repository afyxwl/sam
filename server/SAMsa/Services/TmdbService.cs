using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SAMsa.Services
{
    public class TmdbService
    {
        private readonly HttpClient _http;
        private readonly string? _apiKey;

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
    }
}
