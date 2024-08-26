using Api.Features.HybridRag.Extensions;
using Api.Features.Rag.Extensions;

namespace Api.Features.Extensions
{
    internal static class FeatureExtensions
    {
        public static IServiceCollection AddFeatures(this IServiceCollection services)
        {
            services.AddMediatR(
                cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(FeatureExtensions).Assembly);
                    cfg.Lifetime = ServiceLifetime.Scoped;
                });
            services.AddRag();
            services.AddHybridRag();
            return services;
        }
    }
}
