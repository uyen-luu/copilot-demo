using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Extensions.ExceptionHandler
{
    public class ExceptionToProblemDetailsHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly IHostEnvironment _hostEnvironment;

        public ExceptionToProblemDetailsHandler(
            IProblemDetailsService problemDetailsService,
            IHostEnvironment hostEnvironment)
        {
            _problemDetailsService = problemDetailsService;
            _hostEnvironment = hostEnvironment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (!httpContext.Response.HasStarted) httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails =
                {
                    Title = "An error occurred while processing your request.",
                    Detail = _hostEnvironment.IsDevelopment() ? exception.ToString() : null,
                    Type = exception.GetType().Name
                },
                Exception = exception
            });
        }
    }
}