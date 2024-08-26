using System.Text.Json.Serialization;

namespace Api.Features.HybridRag.Models
{
    public class SearchRequest
    {
        [JsonRequired]
        [JsonPropertyName("prompt")]
        public required string Prompt { get; set; }
    }
}