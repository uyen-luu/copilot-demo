using CsvHelper.Configuration.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Generator.Data
{
    internal class BookCsvModel
    {
        [Name("title")]
        public string? Name { get; set; }

        [Name("description")]
        public string? Description { get; set; }

        [Name("authors")]
        public string? Authors { get; set; }

        [Name("published_year")]
        public int? Year { get; set; }

        [Name("thumbnail")]
        public string? Thumbnail { get; set; }
    }
}
