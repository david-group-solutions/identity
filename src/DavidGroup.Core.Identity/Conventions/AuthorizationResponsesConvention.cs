using DavidGroup.Core.Identity.Attributes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DavidGroup.Core.Identity.Conventions;

/// <summary>
/// Adds standard authorization-related response types to API actions.
/// </summary>
/// <remarks>
/// This convention inspects controller actions during application startup and
/// automatically adds <c>401 Unauthorized</c> and <c>403 Forbidden</c> response
/// types to actions that require authorization. Actions marked with
/// <see cref="AllowAnonymousAttribute"/> are excluded, and existing
/// <see cref="ProducesResponseTypeAttribute"/> instances are preserved.
/// </remarks>
public sealed class AuthorizationResponsesConvention : IApplicationModelConvention
{
    /// <inheritdoc />
    public void Apply(ApplicationModel application)
    {
        foreach (ControllerModel controller in application.Controllers)
        foreach (ActionModel action in controller.Actions)
        {
            if (action.Attributes.OfType<AllowAnonymousAttribute>().Any())
                continue;

            if (!RequiresAuthorization(controller, action))
                continue;

            AddProducesResponse(action, StatusCodes.Status401Unauthorized);
            AddProducesResponse(action, StatusCodes.Status403Forbidden);
        }
    }

    private static bool RequiresAuthorization(ControllerModel controller, ActionModel action) =>
        HasAuthorization(controller.Attributes) || HasAuthorization(action.Attributes);

    private static bool HasAuthorization(IEnumerable<object> attributes) =>
        attributes.Any(a => a is AuthorizeAttribute or HasPermissionAttribute);

    private static void AddProducesResponse(ActionModel action, int statusCode)
    {
        if (action.Filters.OfType<ProducesResponseTypeAttribute>().Any(f => f.StatusCode == statusCode))
            return;

        action.Filters.Add(new ProducesResponseTypeAttribute(statusCode));
    }
}
