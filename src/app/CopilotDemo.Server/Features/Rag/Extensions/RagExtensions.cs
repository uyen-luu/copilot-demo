using Api.Features.Rag.Commands;
using Api.Features.Rag.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Rag.Extensions
{
    internal static class RagExtensions
    {
        public static IServiceCollection AddRag(this IServiceCollection services)
        {
            services.AddSingleton<PromptFactory>();
            return services;
        }

        public static IEndpointRouteBuilder MapRagEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(
                    "/rag",
                    ([FromBody] SearchRequest request, [FromServices] IMediator mediator) =>
                    {
                        var command = new SearchCommand(request);
                        return mediator.Send(command);
                    })
                .Produces<SearchResponse>()
                .ProducesValidationProblem()
                .WithName("GetResultUsingRag")
                .WithOpenApi();
            return app;
        }
    }
}
