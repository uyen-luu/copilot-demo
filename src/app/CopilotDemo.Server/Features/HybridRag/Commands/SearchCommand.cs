using Api.Features.Core;
using Api.Features.Core.Domain;
using Api.Features.Core.VectorDb;
using Api.Features.HybridRag.Models;
using MediatR;

namespace Api.Features.HybridRag.Commands
{
    public record SearchCommand(SearchRequest Request) : IRequest<SearchResponse>
    {
        private class SearchCommandHandler : IRequestHandler<SearchCommand, SearchResponse>
        {
            private readonly IEmbeddingModel _embeddingModel;
            private readonly IVectorDb _vectorDb;
            private readonly IKeywordSearch _keywordSearch;
            private readonly PromptFactory _promptFactory;
            private readonly ILlmProvider _llmProvider;

            public SearchCommandHandler(
                IEmbeddingModel embeddingModel,
                IVectorDb vectorDb,
                PromptFactory promptFactory,
                ILlmProvider llmProvider,
                IKeywordSearch keywordSearch)
            {
                _embeddingModel = embeddingModel;
                _vectorDb = vectorDb;
                _llmProvider = llmProvider;
                _keywordSearch = keywordSearch;
                _promptFactory = promptFactory;
            }

            public async Task<SearchResponse> Handle(SearchCommand request, CancellationToken cancellationToken)
            {
                var searchResultsByKeyword =
                    await _keywordSearch.GetByKeywordAsync(request.Request.Prompt, cancellationToken);
                var embeddings = await _embeddingModel.GetEmbeddingsForTextAsync(
                    request.Request.Prompt,
                    cancellationToken);
                if (embeddings is null) return new SearchResponse { Response = string.Empty };
                var searchResultsByVector = await _vectorDb.GetByVectorSimilarityAsync(embeddings, cancellationToken);
                HashSet<Book> searchResults = [];
                foreach (var result in searchResultsByVector) searchResults.Add(result);
                foreach (var result in searchResultsByKeyword) searchResults.Add(result);
                var prompt = _promptFactory.CreateFromSearchResults(request.Request.Prompt, searchResults);
                var response = await _llmProvider.GetResponseAsync(prompt, cancellationToken);
                return new SearchResponse { Response = response };
            }
        }
    }
}
