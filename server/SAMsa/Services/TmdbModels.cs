using System.Text.Json.Serialization;

namespace SAMsa.Services
{
    public class TmdbGuestSessionDto
    {
        [JsonPropertyName("guest_session_id")]
        public string? GuestSessionId { get; set; }

        [JsonPropertyName("expires_at")]
        public string? ExpiresAt { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class GuestSessionResult
    {
        public string GuestSessionId { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsLocal { get; set; }
    }
}
