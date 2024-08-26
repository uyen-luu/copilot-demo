namespace Api.Features.Core
{
    public interface IEmbeddingModel
    {
        Task<float[]?> GetEmbeddingsForTextAsync(string text, CancellationToken cancellationToken);
        Task<int> GetEmbeddingsDimensionsAsync(CancellationToken cancellationToken);
    }
}
