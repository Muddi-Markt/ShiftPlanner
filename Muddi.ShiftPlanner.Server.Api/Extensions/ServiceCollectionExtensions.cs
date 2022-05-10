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
	 
	    *** Add Roles Mapper *** 
		Go to your Keycloak Admin Console > Client Scopes > roles > Mappers > client roles
		Change "Token Claim Name" as "roles"
		Multivalued: True
		Add to access token: True
		
		*** Add Audience ***
		Edit shift-planner Client
		Go to Mapper
		Create new
		Select Audience
		Included Custom Audience: shift-planner
		
		*** Add User with view-users roles ***
		btw: This is sooo stupid... I dont know why, but since Version 17 or so of fckn Keycloak
		it is not possible anymore to login with the admin user... nvm, here is how to solve it:
		Create User 
		Edit User and set password
		Go to Role Mapping
		Client Roles: realm-managment
		Add view-users
		This will be the AdminUser and AdminPassword
	*/
	public static void AddAuthenticationMuddiConnect(this IServiceCollection services, IConfiguration configuration)
	{
		var muddiConfig = configuration.GetSection("MuddiConnect");
		var authority = muddiConfig["Authority"];
		services.AddSingleton<KeycloakService>();

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

					NameClaimType = "preferred_username",
					ValidAudience = muddiConfig["Audience"],
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.FromMinutes(1)
				};

				o.Events = new JwtBearerEvents()
				{
					OnForbidden = c => { return c.Response.WriteAsync("You do not have the rights to access this entity"); },
					OnAuthenticationFailed = c =>
					{
						// if (Environment.IsDevelopment())
						// {
						// 	return c.Response.WriteAsync(c.Exception.ToString());
						// }

						return Task.CompletedTask;
					}
				};
			});
	}

	private static RefitSettings? KeycloakSettings(IServiceProvider arg)
	{
		var service = arg.GetService<KeycloakService>();
		return new RefitSettings { AuthorizationHeaderValueGetter = service.GetToken };
	}


	public static void AddDatabaseMigrations(this IServiceCollection services)
	{
		services.AddTransient<IStartupFilter, DatabaseMigrationStartupFilter<ShiftPlannerContext>>();
	}
}