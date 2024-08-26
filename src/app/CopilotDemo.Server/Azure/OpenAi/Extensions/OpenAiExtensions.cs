using Api.Azure.OpenAi.Configuration;
using Api.Features.Core;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace Api.Azure.OpenAi.Extensions
{
    public static class OpenAiExtensions
    {
        public static IServiceCollection AddOpenAi(this IServiceCollection services)
        {
            services.AddOptions<OpenAiOptions>()
                .BindConfiguration(OpenAiOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddScoped<OpenAIClient>(
                sp =>
                {
                    var options = sp.GetRequiredService<IOptions<OpenAiOptions>>();
                    var configuration = options.Value;
                    var endpoint = new Uri(configuration.Endpoint);
                    var keyCredential = new AzureKeyCredential(configuration.ApiKey);
                    return new OpenAIClient(endpoint, keyCredential);
                });
            services.AddScoped<IEmbeddingModel, AzureOpenAiEmbeddings>();
            services.AddScoped<ILlmProvider, AzureOpenAiCompletions>();
            return services;
        }
    }
}
