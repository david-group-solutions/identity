using DavidGroup.Core.Identity.Attributes;

using Microsoft.OpenApi;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidGroup.Core.Identity.Swagger;

/// <summary>
/// Adds a description of required permissions to Swagger/OpenAPI documentation for controller actions.
/// </summary>
/// <remarks>
/// This filter inspects the <see cref="HasPermissionAttribute"/> applied to actions or controllers,
/// extracts the required permissions, and appends them to the operation description in Swagger.
/// </remarks>
public class SwaggerPermissionsOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        string?[]? permissions = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<HasPermissionAttribute>()
            .Select(a => a.Policy)
            .Distinct()
            .ToArray();

        if (permissions.Length == 0)
            permissions = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<HasPermissionAttribute>()
                .Select(a => a.Policy)
                .Distinct()
                .ToArray();

        if (permissions is null || permissions.Length <= 0) return;

        string permissionsString = string.Join(", ", permissions);
        operation.Description += $"<p>Required Permissions ({permissionsString})</p>";
    }
}
