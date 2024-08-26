using System.Globalization;
using CsvHelper;

namespace Generator.Data
{
    internal class BookDataSource : IEntityDataSource<Book>
    {
        public async Task<IReadOnlyCollection<Book>> GetAsync(CancellationToken cancel)
        {
            using var reader = new StreamReader("Data/books.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            List<Book> books = [];
            await foreach (var model in csv.GetRecordsAsync<BookCsvModel>(cancel))
            {
                if (model.Name is null || model.Description is null || model.Authors is null || model.Year is null)
                {
                    continue;
                }
                var book = new Book(model.Name, model.Description, model.Authors, model.Year.Value, model.Thumbnail);
                books.Add(book);
            }
            return books;
        }
    }
}
