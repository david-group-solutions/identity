using DavidGroup.Core.Identity.Extensions;
using DavidGroup.Core.Identity.MessageHandlers;
using DavidGroup.Core.Identity.Samples.WebApi.HttpClients;
using DavidGroup.Core.SwaggerSetup.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddControllersWithIdentity();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtAuthenticationMessageHandler>();

builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
    {
        client.BaseAddress = new Uri("https://inventory-api");
    })
    .AddHttpMessageHandler<JwtAuthenticationMessageHandler>()
    .ConfigurePrimaryHttpMessageHandler(() => new MockInventoryApiHandler());

builder.Services.AddDefaultSwagger(swagger => swagger.WithBearerAuth());

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDefaultSwagger();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
