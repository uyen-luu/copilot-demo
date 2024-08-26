using System.Runtime.CompilerServices;
using Api.Azure.Search;
using Api.Features.Core;
using Api.Features.Core.VectorDb.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Generator.Data;
using Generator.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Generator
{
    internal class VectorDb : BackgroundService
    {
        private const string IndexName = "books";
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<VectorDb> _logger;

        public VectorDb(
            IServiceScopeFactory scopeFactory,
            ILogger<VectorDb> logger,
            IHostApplicationLifetime applicationLifetime)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
        }

        private async Task PopulateIndexAsync(CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            var embeddingModel = serviceProvider.GetRequiredService<IEmbeddingModel>();
            var searchIndexClient = serviceProvider.GetRequiredService<SearchIndexClient>();
            await CreateIndexAsync(embeddingModel, searchIndexClient, cancellationToken);
            var entityDataSource = serviceProvider.GetRequiredService<IEntityDataSource<Book>>();
            var searchClientFactory = serviceProvider.GetRequiredService<SearchClientFactory>();
            var searchClient = searchClientFactory.CreateForIndex(IndexName);
            var documentsEnumerable = GetEntityDocumentsAsync(entityDataSource, embeddingModel, cancellationToken);
            await foreach (var documents in documentsEnumerable.Batch(100).WithCancellation(cancellationToken))
            {
                await searchClient.IndexDocumentsAsync(
                    IndexDocumentsBatch.Upload(documents),
                    new IndexDocumentsOptions { ThrowOnAnyError = true },
                    cancellationToken: cancellationToken);
            }
        }

        private static async Task CreateIndexAsync(
            IEmbeddingModel embeddingModel,
            SearchIndexClient searchIndexClient,
            CancellationToken cancellationToken)
        {
            const string VectorSearchProfile = "my-vector-profile";
            const string VectorSearchHnswConfig = "my-hsnw-vector-config";
            const string VectorSearchKnnConfig = "my-knn-vector-config";
            var modelDimensions = await embeddingModel.GetEmbeddingsDimensionsAsync(cancellationToken);
            SearchIndex searchIndex = new(IndexName)
            {
                Fields =
                {
                    new SimpleField(nameof(BookDbModel.Id), SearchFieldDataType.String)
                    {
                        IsKey = true, IsFilterable = true, IsSortable = true, IsFacetable = true
                    },
                    new SearchableField(nameof(BookDbModel.Name)) { IsFilterable = true, IsSortable = true },
                    new SearchableField(nameof(BookDbModel.Description)) { IsFilterable = true },
                    new SearchField(
                        nameof(BookDbModel.DescriptionVector),
                        SearchFieldDataType.Collection(SearchFieldDataType.Single))
                    {
                        IsSearchable = true,
                        VectorSearchDimensions = modelDimensions,
                        VectorSearchProfileName = VectorSearchProfile
                    },
                    new SearchableField(nameof(BookDbModel.Authors), collection: true)
                    {
                        IsFilterable = true, IsSortable = false
                    },
                    new SearchField(nameof(BookDbModel.Year), SearchFieldDataType.Int32)
                    {
                        IsFilterable = true, IsSortable = true
                    },
                    new SimpleField(nameof(BookDbModel.ThumbnailUrl), SearchFieldDataType.String)
                    {
                        IsFilterable = false, IsSortable = false, IsFacetable = false
                    }
                },
                VectorSearch = new VectorSearch
                {
                    Profiles = { new VectorSearchProfile(VectorSearchProfile, VectorSearchKnnConfig) },
                    Algorithms =
                    {
                        new HnswAlgorithmConfiguration(VectorSearchHnswConfig),
                        new ExhaustiveKnnAlgorithmConfiguration(VectorSearchKnnConfig)
                    }
                }
            };
            await searchIndexClient.CreateOrUpdateIndexAsync(
                index: searchIndex,
                allowIndexDowntime: false,
                onlyIfUnchanged: true,
                cancellationToken: cancellationToken);
        }

        private async IAsyncEnumerable<BookDbModel> GetEntityDocumentsAsync(
            IEntityDataSource<Book> entityData,
            IEmbeddingModel embeddingModel,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var models = await entityData.GetAsync(cancellationToken);
            var count = 0;
            foreach (var model in models)
            {
                var id = ++count + "";
                var descriptionVector = await embeddingModel.GetEmbeddingsForTextAsync(
                    model.Description,
                    cancellationToken);
                var entity = new BookDbModel
                {
                    Id = id,
                    Name = model.Name,
                    Description = model.Description,
                    DescriptionVector = descriptionVector,
                    Authors = model.Authors.Split(";", StringSplitOptions.RemoveEmptyEntries),
                    Year = model.Year,
                    ThumbnailUrl = model.ThumbnailUrl
                };
                yield return entity;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await PopulateIndexAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while populating index {IndexName}", IndexName);
            }
            finally
            {
                _applicationLifetime.StopApplication();
            }
        }
    }
}
