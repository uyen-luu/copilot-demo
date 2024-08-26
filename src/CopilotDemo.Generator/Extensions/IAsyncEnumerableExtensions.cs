namespace Generator.Extensions
{
    internal static class AsyncExtensions
    {
        public static async IAsyncEnumerable<List<T>> Batch<T>(this IAsyncEnumerable<T> source, int batchSize)
        {
            var batch = new List<T>(batchSize);
            await foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count < batchSize) continue;
                yield return batch;
                batch = new List<T>(batchSize);
            }
            if (batch.Count > 0)
            {
                yield return batch;
            }
        }
    }
}
