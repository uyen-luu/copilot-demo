using Api.Features.Core.Domain;

namespace Api.Features.Core
{
    internal interface IKeywordSearch
    {
        public Task<IReadOnlyCollection<Book>> GetByKeywordAsync(
            string query,
            CancellationToken cancellationToken);
    }
}
