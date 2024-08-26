using System.Text.Json.Serialization;

namespace Api.Features.Rag.Models
{
    public class SearchResponse
    {
        [JsonRequired]
        [JsonPropertyName("response")]
        public required string Response { get; set; }
    }
}