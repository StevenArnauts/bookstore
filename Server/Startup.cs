using System.IdentityModel.Tokens.Jwt;
using Common;
using Bookstore.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Text;

namespace Bookstore {

	public static class JwtSecurityKey {

		public static SymmetricSecurityKey Create(string secret) {
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
		}

	}

	public class Startup {

		public Startup(IConfiguration configuration) {
			this.Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {

			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			services.Configure<CookiePolicyOptions>(options => {
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddCors(options => {
				options.AddPolicy("me", builder => {
					builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
				});
			});

			HostConfiguration host = services.UseHostConfiguration(this.Configuration);

			//services.AddDbContext<BookstoreContext>(options => options.UseSqlServer(Configuration.GetConnectionString("bookstore")));
			services.AddDbContext<BookstoreContext>(options => { options.UseNpgsql(Configuration.GetConnectionString("bookstore")); });

			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie()
			 .AddJwtBearer("bsoa", options => {
				 options.Authority = "https://localhost:6103";
				 options.Audience = "bookstore";
			 })
			.AddOpenIdConnect("bsid", options => {
				// options.Authority = "http://" + host.Host + ":6003";
				options.Authority = "https://" + host.Host + ":6103";
				options.ClientId = "bookstore.orders";
				options.ClientSecret = "secret";
				options.ResponseType = "code";

				options.Scope.Clear();
				options.Scope.Add("openid");
				options.Scope.Add("profile");
				options.Scope.Add("email");

				options.SignedOutCallbackPath = "/signout-callback";
				options.SignedOutRedirectUri = "/signedout";
				options.CallbackPath = new PathString("/callback");

				options.ClaimsIssuer = "bsid";
				options.RequireHttpsMetadata = false;
				options.ClaimActions.Clear();

				options.ClaimActions.MapUniqueJsonKey("name", "name");

				options.GetClaimsFromUserInfoEndpoint = true;

				options.TokenValidationParameters = new TokenValidationParameters {
					NameClaimType = "name",
					RoleClaimType = "role"
				};

				options.SaveTokens = true;

			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddEntities();

		}	

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			loggerFactory.AddNLog();
			app.UseDeveloperExceptionPage();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();
			app.UseCors("me");
			app.UseMvc(routes => {
				routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
			});
			app.UseSeed();
		}

	}

}