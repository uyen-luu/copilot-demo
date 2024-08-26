using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Api.Azure.OpenAi.Configuration
{
    [DataContract]
    public class CompletionOptions
    {
        [DataMember(Name = "deploymentName")]
        [Required]
        public required string DeploymentName { get; set; }

        [DataMember(Name = "temperature")]
        public float Temperature { get; set; } = 0.7f;

        [DataMember(Name = "maxTokens")]
        public int MaxTokens { get; set; } = 800;

        [DataMember(Name = "topP")]
        public float TopP { get; set; } = 0.95f;

        [DataMember(Name = "frequencyPenalty")]
        public float FrequencyPenalty { get; set; }

        [DataMember(Name = "presencePenalty")]
        public float PresencePenalty { get; set; }
    }
}
