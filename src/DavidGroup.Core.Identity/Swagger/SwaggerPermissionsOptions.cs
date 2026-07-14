using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidGroup.Core.Identity.Swagger;

/// <summary>
/// Configures Swagger to include permissions information in the API documentation.
/// </summary>
/// <remarks>
/// This class implements <see cref="IConfigureOptions{SwaggerGenOptions}"/> and adds
/// the <see cref="SwaggerPermissionsOperationFilter"/> to Swagger's operation filters.
/// </remarks>
public class SwaggerPermissionsOptions : IConfigureOptions<SwaggerGenOptions>
{
    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        options.OperationFilter<SwaggerPermissionsOperationFilter>();
    }
}
