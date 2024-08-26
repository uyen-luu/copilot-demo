using Api.Azure.Search.Configuration;
using Api.Features.Core;
using Api.Features.Core.VectorDb;
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Options;

namespace Api.Azure.Search.Extensions
{
    public static class AiSearchExtensions
    {
        public static IServiceCollection AddAiSearch(this IServiceCollection services)
        {
            services.AddOptions<AiSearchOptions>()
                .BindConfiguration(AiSearchOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddSingleton<SearchClientFactory>();
            services.AddScoped<SearchIndexClient>(
                sp =>
                {
                    var options = sp.GetRequiredService<IOptions<AiSearchOptions>>();
                    var configuration = options.Value;
                    var endpoint = new Uri(configuration.Endpoint);
                    var keyCredential = new AzureKeyCredential(configuration.ApiKey);
                    return new SearchIndexClient(endpoint, keyCredential);
                });
            services.AddScoped<IVectorDb, AzureAiSearch>();
            services.AddScoped<IKeywordSearch, AzureAiSearch>();
            return services;
        }
    }
}
