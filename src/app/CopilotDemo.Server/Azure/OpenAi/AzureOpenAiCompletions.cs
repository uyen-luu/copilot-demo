using Api.Azure.OpenAi.Configuration;
using Api.Features.Core;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace Api.Azure.OpenAi
{
    internal class AzureOpenAiCompletions : ILlmProvider
    {
        private readonly IOptionsMonitor<OpenAiOptions> _configuration;
        private readonly OpenAIClient _client;

        public AzureOpenAiCompletions(IOptionsMonitor<OpenAiOptions> configuration, OpenAIClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public async Task<string> GetResponseAsync(string prompt, CancellationToken cancellationToken)
        {
            var configuration = _configuration.CurrentValue;
            var completion = configuration.Completion ??
                throw new InvalidOperationException("Completion options are not configured");
            var options = new ChatCompletionsOptions
            {
                DeploymentName = completion.DeploymentName,
                Temperature = completion.Temperature,
                MaxTokens = completion.MaxTokens,
                NucleusSamplingFactor = completion.TopP,
                FrequencyPenalty = completion.FrequencyPenalty,
                PresencePenalty = completion.PresencePenalty,
                Messages = { new ChatRequestSystemMessage(prompt) }
            };
            ChatCompletions response = await _client.GetChatCompletionsAsync(options, cancellationToken);
            var choices = response.Choices;
            return choices.Count > 0 ? choices[0].Message.Content : string.Empty;
        }
    }
}
