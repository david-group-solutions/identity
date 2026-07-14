using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DavidGroup.Core.Identity.ResultHandlers;

/// <summary>
/// Provides detailed authorization failure responses using the RFC 7807
/// <see cref="ProblemDetails"/> format.
/// </summary>
/// <remarks>
/// This handler extends the default authorization middleware behavior by
/// including information about failed requirements and authorization failure
/// reasons in forbidden responses. Successful authorization results and
/// non-forbidden responses are delegated to the default handler.
/// </remarks>
public sealed class DetailedAuthorizationResultHandler
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    /// <inheritdoc />
    public async Task HandleAsync(RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Forbidden)
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }

        ProblemDetails problem = new()
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = "Authorization policy failed."
        };

        AuthorizationFailure? failure = authorizeResult.AuthorizationFailure;
        if (failure is not null)
        {
            string[] failedRequirements = failure.FailedRequirements
                .Select(static r => r.GetType().Name)
                .ToArray();

            if (failedRequirements.Length > 0)
                problem.Extensions[nameof(failedRequirements)] = failedRequirements;

            string[] reasons = failure.FailureReasons
                .Select(static r => r.Message)
                .Distinct()
                .ToArray();

            if (reasons.Length > 0)
                problem.Extensions[nameof(reasons)] = reasons;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }
}
