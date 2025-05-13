using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Muddi.ShiftPlanner.Server.Api.Filters;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Extensions;

public static class ServiceCollectionExtensions
{
	/*	To make this work you need to configure Keycloak first:

		*** Create muddi Realm ***
		1. Create Muddi Realm
		2. Create Client 'shift-planner-backend'
		   - Set client authentication to ON (confidential)
		   - Generate a client secret (this is the client secret that is used to authenticate the client)
		   - Configure valid redirect URIs and web origins as needed
		3. Create Client 'shift-planner-frontend'
		   - Set client authentication to OFF (public)
		   - Configure valid redirect URIs and web origins as needed
		
		*** Add Protocol Mappers ***
		1. Go to Clients > shift-planner-frontend > Client Scopes > Dedicated Scopes > shift-planner-frontend-dedicated
		2. Go to Mappers tab
		3. Add mappers:
		   - Add "Subject (sub)" mapper to include user ID in token
		   - Add "User Property" mapper for email, firstName, lastName
		   - Add "User's full name" mapper for name claim
		
		*** Add Role Mappings ***
		1. Go to Clients > shift-planner-frontend > Client Scopes > Dedicated Scopes > shift-planner-frontend-dedicated > Mappers
		2. Add new mapper:
		   - Name: "client roles"
		   - Mapper Type: "User Client Role"
		   - Client ID: shift-planner-backend
		   - Client Role prefix: (leave empty)
		   - Multivalued: ON
		   - Token Claim Name: roles
		   - Claim JSON Type: String
		   - Add to ID token: ON
		   - Add to access token: ON
		   - Add to userinfo: ON
		
		*** Configure Client Roles ***
		1. Go to Clients > shift-planner-backend > Roles
		2. Add roles:
		   - super-admin: full access including deletions
		   - admin: create/update access, can only delete shifts
		   - editor: create and edit own shifts
		   - viewer: read-only access
		
		3. Go to Roles > default-roles-muddi > Client Roles
		4. Add 'editor' and 'viewer' as default roles

		*** Configure Service Account ***
		1. Enable service account in shift-planner-backend client settings
		2. Go to Service Account Roles tab
		3. Add realm-management roles:
		   - view-users: required for user lookup
		   - view-clients: required for client info
		   - view-realm: required for realm settings
		   - query-users: required for user search
		   - query-groups: required for group operations
		   - query-clients: required for client queries
		
		Note: The service account needs these roles to perform administrative tasks
		through the Keycloak Admin API. Adjust the roles based on your security requirements.
	*/
	public static void AddAuthenticationMuddiConnect(this IServiceCollection services, IConfiguration configuration)
	{
		var muddiConfig = configuration.GetSection("MuddiConnect");
		var authority = muddiConfig["Authority"];
		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
		services.AddSingleton<IKeycloakService, KeycloakService>();
		services.AddAuthorization();
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(o =>
			{
				o.MetadataAddress = authority.TrimEnd('/') + "/.well-known/openid-configuration";
				o.RequireHttpsMetadata = false;
				o.SaveToken = true;
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = authority,
					RoleClaimType = "roles",
					NameClaimType = "preferred_username",
					ValidAudience = muddiConfig["Audience"],
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.FromMinutes(1)
				};
				o.MapInboundClaims = false;

				o.Events = new JwtBearerEvents()
				{
					OnForbidden = c =>
					{
						return c.Response.WriteAsync("You do not have the rights to access this entity");
					},
					OnAuthenticationFailed = c =>
					{
						return Task.CompletedTask;
					}
				};
			});
	}

	public static void AddDatabaseMigrations(this IServiceCollection services)
	{
		services.AddTransient<IStartupFilter, DatabaseMigrationStartupFilter<ShiftPlannerContext>>();
	}
}