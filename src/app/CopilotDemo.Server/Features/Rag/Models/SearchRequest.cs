using System.Text.Json.Serialization;

namespace Api.Features.Rag.Models
{
    public class SearchRequest
    {
        [JsonRequired]
        [JsonPropertyName("prompt")]
        public required string Prompt { get; set; }
    }
}