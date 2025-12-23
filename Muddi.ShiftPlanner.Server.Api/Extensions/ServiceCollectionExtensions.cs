using System.IdentityModel.Tokens.Jwt;
using Duende.AccessTokenManagement;
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
		Create Muddi Realm
		Clients -> Create
		shift-planner
		Go to Mappers -> Create
			Name: role-mapper
			Mapper Type: User Client Role
			Client ID: shift-planner
			Token Claim Name: roles
			Claim JSON Type: String
			Add to ID: Off
			Add to access token: On
			Add to userinfo: On
		Mappers:
			Create new
			Select Audience
			Included Custom Audience: shift-planner


	    *** Add Roles Mapper ***
		Go to your Keycloak Admin Console > Client Scopes > roles > Mappers > client roles
		Add to userinfo: True


		*** Add default roles for shift-planner ***
		Go to shift-planner client > Roles
		Add four roles: super-admin, admin, editor, viewer
			viewer: only viewing allowed
			editor: allowed to create and edit own shifts
			admin: allowed to create or update mostly anything, only allowed to delete shifts
			super-admin: allowed to delete everything

		Go to Roles > default-roles-muddi > Client Roles: shift-planner
		Add 'editor' and 'viewer' to Client Default Roles


		*** Create Service Account Client for API access ***
		Clients -> Create Client
			Client ID: shift-planner-server-api
			Client authentication: ON (confidential)
			Service accounts roles: ON
		After creation, go to Credentials tab to get the Client Secret
		Go to Service Account Roles tab
		Assign 'view-users' role from 'realm-management' client

		*** Add User Registration ***
	    Realm Settings -> Login
	    Set the things you want (e.g. user registration)
	    Realm Settings -> Email
	    Set SMTP Host, Enable SSL and Enable Authentication
	*/
	public static void AddAuthenticationMuddiConnect(this IServiceCollection services, IConfiguration configuration)
	{
		var muddiConfig = configuration.GetSection("MuddiConnect");
		var authority = muddiConfig["Authority"];
		var authorityUri = new Uri(authority!);
		var baseUrl = authorityUri.GetLeftPart(UriPartial.Authority);

		var serviceClientId = muddiConfig["ClientId"];
		var serviceClientSecret = muddiConfig["ClientSecret"];
		if (string.IsNullOrEmpty(serviceClientId) || string.IsNullOrEmpty(serviceClientSecret))
			throw new InvalidOperationException("You need to specify ClientId and ClientSecret in MuddiConnect");

		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

		services.AddClientCredentialsTokenManagement()
			.AddClient(ClientCredentialsClientName.Parse("keycloak"), client =>
			{
				client.TokenEndpoint = new Uri($"{authority}/protocol/openid-connect/token");
				client.ClientId = ClientId.Parse(serviceClientId);
				client.ClientSecret = ClientSecret.Parse(serviceClientSecret);
			});

		services.AddRefitClient<IKeycloakApi>()
			.ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
			.AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse("keycloak"));

		services.AddScoped<IKeycloakService, KeycloakService>();
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
					// NOTE: Usually you don't need to set the issuer since the middleware will extract it 
					// from the .well-known endpoint provided above. but since I am using the container name in
					// the above URL which is not what is published issueer by the well-known, I'm setting it here.
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
					OnForbidden = c => c.Response.WriteAsJsonAsync("You do not have the rights to access this entity")
				};
			});
	}

	public static void AddDatabaseMigrations(this IServiceCollection services)
	{
		services.AddTransient<IStartupFilter, DatabaseMigrationStartupFilter<ShiftPlannerContext>>();
	}
}