using Api.Features.Core.Domain;
using Api.Features.Rag.Models;

namespace Api.Features.Core.VectorDb
{
    internal interface IVectorDb
    {
        Task<IReadOnlyCollection<Book>> GetByVectorSimilarityAsync(
            float[] vectors,
            CancellationToken cancellationToken);
    }
}
