using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Muddi.ShiftPlanner.Client;

public static class MuddiConnectExtension
{
    public static void AddMuddiConnect(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireClaim("role", "admin"));
        });
        services.AddOidcAuthentication(options =>
        {
            options.UserOptions.NameClaim = "preferred_username";

            configuration.Bind(options.ProviderOptions);
            options.ProviderOptions.ResponseType = "code";
            options.ProviderOptions.DefaultScopes.Add("roles");
            options.ProviderOptions.DefaultScopes.Add("email");
        });

    }
}