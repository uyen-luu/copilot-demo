using Api.Features.HybridRag.Extensions;
using Api.Features.Rag.Extensions;

namespace Api.Extensions.Endpoints
{
    internal static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapRagEndpoints();
            app.MapHybridRagEndpoints();
            return app;
        }
    }
}
