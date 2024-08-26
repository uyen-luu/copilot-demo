using Api.Features.HybridRag.Commands;
using Api.Features.HybridRag.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.HybridRag.Extensions
{
    internal static class HybridRagExtensions
    {
        public static IServiceCollection AddHybridRag(this IServiceCollection services)
        {
            services.AddSingleton<PromptFactory>();
            return services;
        }

        public static IEndpointRouteBuilder MapHybridRagEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost(
                    "/hybrid-rag",
                    ([FromBody] SearchRequest request, [FromServices] IMediator mediator) =>
                    {
                        var command = new SearchCommand(request);
                        return mediator.Send(command);
                    })
                .Produces<SearchResponse>()
                .ProducesValidationProblem()
                .WithName("GetResultUsingHybridRag")
                .WithOpenApi();

            return app;
        }
    }
}
