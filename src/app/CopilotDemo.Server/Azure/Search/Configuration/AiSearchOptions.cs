using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Api.Azure.Search.Configuration
{
    public class AiSearchOptions
    {
        public static string SectionName => "Azure:AiSearch";

        [DataMember(Name = "endpoint")]
        [Required]
        public required string Endpoint { get; set; }

        [DataMember(Name = "apiKey")]
        [Required]
        public required string ApiKey { get; set; }
    }
}
