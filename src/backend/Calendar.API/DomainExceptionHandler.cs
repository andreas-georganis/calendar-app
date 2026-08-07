using Calendar.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API;

public class DomainExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public DomainExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<DomainExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    private readonly ILogger<DomainExceptionHandler> _logger;

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not CalendarDomainException calendarDomainException)
        {
            return ValueTask.FromResult(false);
        }

        _logger.LogError(exception, "{Message}", exception.Message);

        var details = new ProblemDetails
        {
            Title = "A domain error occurred.",
            Status = StatusCodes.Status400BadRequest,
            Detail = calendarDomainException.Message,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
            }
        };

        httpContext.Response.StatusCode = details!.Status.Value;

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = details,
        };

        return _problemDetailsService.TryWriteAsync(context);
    }
}
