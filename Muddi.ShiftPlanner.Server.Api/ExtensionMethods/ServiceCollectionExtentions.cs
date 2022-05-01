using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Muddi.ShiftPlanner.Server.Api.ExtensionMethods;

public static class ServiceCollectionExtentions
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
		
	*/
	public static void AddAuthenticationMuddiConnect(this IServiceCollection services, IConfiguration configuration)
	{
		var muddiConfig = configuration.GetSection("MuddiConnect");
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(o =>
			{
				var authority = muddiConfig["Authority"];

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
						c.NoResult();

						c.Response.StatusCode = 500;
						c.Response.ContentType = "text/plain";


						// if (Environment.IsDevelopment())
						// {
						// 	return c.Response.WriteAsync(c.Exception.ToString());
						// }

						return c.Response.WriteAsync(c.Exception.Message);
					}
				};
			});
	}
}