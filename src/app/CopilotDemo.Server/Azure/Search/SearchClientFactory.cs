using Api.Azure.Search.Configuration;
using Azure;
using Azure.Search.Documents;
using Microsoft.Extensions.Options;

namespace Api.Azure.Search
{
    public class SearchClientFactory
    {
        private readonly IOptionsMonitor<AiSearchOptions> _configuration;

        public SearchClientFactory(IOptionsMonitor<AiSearchOptions> configuration)
        {
            _configuration = configuration;
        }

        public SearchClient CreateForIndex(string indexName)
        {
            var configuration = _configuration.CurrentValue;
            var endpoint = new Uri(configuration.Endpoint);
            var keyCredential = new AzureKeyCredential(configuration.ApiKey);
            return new SearchClient(endpoint, indexName, keyCredential);
        }
    }
}
