using System.Text.Json.Serialization;

namespace Api.Features.HybridRag.Models
{
    public class SearchResponse
    {
        [JsonRequired]
        [JsonPropertyName("response")]
        public required string Response { get; set; }
    }
}
