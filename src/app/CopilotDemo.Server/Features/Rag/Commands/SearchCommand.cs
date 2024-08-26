using Api.Features.Core;
using Api.Features.Core.VectorDb;
using Api.Features.Rag.Models;
using MediatR;

namespace Api.Features.Rag.Commands
{
    public record SearchCommand(SearchRequest Request) : IRequest<SearchResponse>
    {
        private class SearchCommandHandler : IRequestHandler<SearchCommand, SearchResponse>
        {
            private readonly IEmbeddingModel _embeddingModel;
            private readonly IVectorDb _vectorDb;
            private readonly PromptFactory _promptFactory;
            private readonly ILlmProvider _llmProvider;

            public SearchCommandHandler(
                IEmbeddingModel embeddingModel,
                IVectorDb vectorDb,
                PromptFactory promptFactory,
                ILlmProvider llmProvider)
            {
                _embeddingModel = embeddingModel;
                _vectorDb = vectorDb;
                _llmProvider = llmProvider;
                _promptFactory = promptFactory;
            }

            public async Task<SearchResponse> Handle(SearchCommand request, CancellationToken cancellationToken)
            {
                var embeddings = await _embeddingModel.GetEmbeddingsForTextAsync(
                    request.Request.Prompt,
                    cancellationToken);
                if (embeddings is null) return new SearchResponse { Response = string.Empty };
                var searchResults = await _vectorDb.GetByVectorSimilarityAsync(embeddings, cancellationToken);
                var bestMatch = searchResults.FirstOrDefault();
                var prompt = _promptFactory.CreateFromSearchResults(request.Request.Prompt, bestMatch);
                var response = await _llmProvider.GetResponseAsync(prompt, cancellationToken);
                return new SearchResponse { Response = response };
            }
        }
    }
}
