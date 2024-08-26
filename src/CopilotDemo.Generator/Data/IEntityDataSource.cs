namespace Generator.Data
{
    internal interface IEntityDataSource<T>
    {
        Task<IReadOnlyCollection<T>> GetAsync(CancellationToken cancel);
    }
}
