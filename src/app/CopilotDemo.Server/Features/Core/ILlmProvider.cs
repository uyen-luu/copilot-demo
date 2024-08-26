namespace Api.Features.Core
{
    internal interface ILlmProvider
    {
        Task<string> GetResponseAsync(string prompt, CancellationToken cancellationToken);
    }
}
