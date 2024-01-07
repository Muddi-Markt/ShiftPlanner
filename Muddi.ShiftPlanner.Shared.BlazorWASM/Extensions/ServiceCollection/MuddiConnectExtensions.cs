using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Muddi.ShiftPlanner.Shared.BlazorWASM;

public static partial class ServiceCollectionExtensions
{
    public static void AddOauthConnect(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(Policies.IsAdmin, policy => policy.RequireRole("admin"));
        });
        services.AddOidcAuthentication(options =>
        {
            options.UserOptions.NameClaim = "given_name";
            options.UserOptions.RoleClaim = "roles";

            options.ProviderOptions.ClientId = configuration["Audience"];
            options.ProviderOptions.Authority = configuration["Authority"];
            options.ProviderOptions.ResponseType = "code";
        }).AddAccountClaimsPrincipalFactory<AccountClaimsPrincipalFactoryEx>();

    }
}


//https://nahidfa.com/posts/blazor-webassembly-role-based-and-policy-based-authorization/
public class AccountClaimsPrincipalFactoryEx : AccountClaimsPrincipalFactory<RemoteUserAccount>
{

    public AccountClaimsPrincipalFactoryEx(IAccessTokenProviderAccessor accessor) : base(accessor)
    {
    }

    public async override ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return user;
        }

        var identity = (ClaimsIdentity)user.Identity;
        var roleClaims = identity.FindAll(identity.RoleClaimType);

        if (roleClaims == null || !roleClaims.Any())
        {
            return user;
        }
        
        var rolesElem = account.AdditionalProperties[identity.RoleClaimType];

        if (rolesElem is JsonElement roles)
        {
            if (roles.ValueKind == JsonValueKind.Array)
            {
                identity.RemoveClaim(identity.FindFirst(options.RoleClaim));
                foreach (var role in roles.EnumerateArray())
                {
                    identity.AddClaim(new Claim(options.RoleClaim, role.GetString()!));
                }
            }
        }

        return user;
    }
}