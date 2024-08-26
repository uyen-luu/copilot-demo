using Api.Features.Core;
using Api.Features.Core.Domain;
using Api.Features.Core.VectorDb;
using Api.Features.Core.VectorDb.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace Api.Azure.Search
{
    internal class AzureAiSearch : IVectorDb, IKeywordSearch
    {
        private const string IndexName = "books";
        private readonly SearchClientFactory _searchClientFactory;

        public AzureAiSearch(SearchClientFactory searchClientFactory)
        {
            _searchClientFactory = searchClientFactory;
        }

        public async Task<IReadOnlyCollection<Book>> GetByKeywordAsync(
            string query,
            CancellationToken cancellationToken)
        {
            var searchClient = _searchClientFactory.CreateForIndex(IndexName);
            var searchOptions = new SearchOptions
            {
                QueryType = SearchQueryType.Simple, IncludeTotalCount = false, SearchMode = SearchMode.Any, Size = 5
            };
            SearchResults<BookDbModel> response = await searchClient.SearchAsync<BookDbModel>(
                searchText: query,
                options: searchOptions,
                cancellationToken: cancellationToken);
            var searchResults = new List<Book>();
            await foreach (var result in response.GetResultsAsync())
            {
                var doc = result.Document;
                var entity = new Book(
                    doc.Id,
                    doc.Name,
                    doc.Description,
                    doc.Authors.ToArray(),
                    doc.Year,
                    doc.ThumbnailUrl);
                searchResults.Add(entity);
            }
            return searchResults;
        }

        public async Task<IReadOnlyCollection<Book>> GetByVectorSimilarityAsync(
            float[] vectors,
            CancellationToken cancellationToken)
        {
            var searchClient = _searchClientFactory.CreateForIndex(IndexName);
            var searchOptions = new SearchOptions
            {
                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizedQuery(vectors)
                        {
                            KNearestNeighborsCount = 5, Fields = { nameof(BookDbModel.DescriptionVector) }
                        }
                    }
                }
            };
            SearchResults<BookDbModel> response = await searchClient.SearchAsync<BookDbModel>(
                searchText: null,
                options: searchOptions,
                cancellationToken: cancellationToken);
            var searchResults = new List<Book>();
            await foreach (var result in response.GetResultsAsync())
            {
                var doc = result.Document;
                var entity = new Book(
                    doc.Id,
                    doc.Name,
                    doc.Description,
                    doc.Authors.ToArray(),
                    doc.Year,
                    doc.ThumbnailUrl);
                searchResults.Add(entity);
            }
            return searchResults;
        }
    }
}
