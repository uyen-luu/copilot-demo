using System.Text;
using Api.Features.Core.Domain;

namespace Api.Features.Rag
{
    internal class PromptFactory
    {
        public string CreateFromSearchResults(string requestPrompt, Book? searchResult)
        {
            var sb = new StringBuilder("Act as a search copilot, be helpful and informative.");
            sb.AppendLine("--------------");
            sb.AppendLine("Based on the user's query below: ");
            sb.AppendLine($"'{requestPrompt}'");
            sb.AppendLine("Here is some information about the query. It has the following information:");
            if (searchResult is null)
            {
                sb.AppendLine("No information found");
            }
            else
            {
                var name = searchResult.Name;
                var description = searchResult.Description;
                var authors = string.Join(" and ", searchResult.Authors);
                var year = searchResult.Year;
                sb.AppendLine(
                    $"Name is '{name}', Description is '{description}', Authors are '{authors}',Year is '{year}'");
            }
            sb.AppendLine("--------------");
            sb.AppendLine("note: Be concise and dont add any other details if you don't know about it.");
            return sb.ToString();
        }
    }
}
