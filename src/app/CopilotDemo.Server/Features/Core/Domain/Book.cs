namespace Api.Features.Core.Domain
{
    public record Book(
        string Id,
        string Name,
        string Description,
        IReadOnlyCollection<string> Authors,
        int Year,
        string? ThumbnailUrl);
}
