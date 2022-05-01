
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
            
            options.ProviderOptions.ClientId = configuration["MuddiConnect:Audience"];
            options.ProviderOptions.Authority = configuration["MuddiConnect:Authority"];
            options.ProviderOptions.ResponseType = "code";
            options.ProviderOptions.DefaultScopes.Add("roles");
            options.ProviderOptions.DefaultScopes.Add("email");
        });

    }
}