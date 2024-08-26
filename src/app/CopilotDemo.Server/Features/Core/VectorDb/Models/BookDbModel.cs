#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Api.Features.Core.VectorDb.Models
{
    public class BookDbModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ReadOnlyMemory<string> Authors { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public ReadOnlyMemory<float>? DescriptionVector { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
