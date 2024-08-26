using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Api.Azure.OpenAi.Configuration
{
    [DataContract]
    public class EmbeddingOptions
    {
        [DataMember(Name = "deploymentName")]
        [Required]
        public required string DeploymentName { get; set; }
    }
}
