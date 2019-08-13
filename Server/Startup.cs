using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Common;
using Bookstore.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using Utilities.Logging;

namespace Bookstore {

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

			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddOpenIdConnect("dora", options => {
				// options.Authority = "http://" + host.Host + ":6003";
				options.Authority = "https://" + host.Host + ":6103";
				options.ClientId = "bookstore";
				options.ClientSecret = "secret";
				options.ResponseType = "code";

				options.Scope.Clear();
				options.Scope.Add("openid");
				options.Scope.Add("profile");
				options.Scope.Add("email");

				options.SignedOutCallbackPath = "/signout-callback";
				options.SignedOutRedirectUri = "/signedout";
				options.CallbackPath = new PathString("/callback");

				options.ClaimsIssuer = "dora";
				options.RequireHttpsMetadata = false;
				options.ClaimActions.Clear();

				options.GetClaimsFromUserInfoEndpoint = true;

				options.TokenValidationParameters = new TokenValidationParameters {
					NameClaimType = "name",
					RoleClaimType = "role"
				};

				options.SaveTokens = true;

				this.SetupPkce(options);

			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.UseEntities();

		}

		private void SetupPkce(OpenIdConnectOptions options) {

			options.Events.OnRedirectToIdentityProvider = context => {
				if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication) {
					var codeVerifier = CryptoRandom.CreateUniqueId(32);
					context.Properties.Items.Add("code_verifier", codeVerifier);
					string codeChallenge;
					using (var sha256 = SHA256.Create()) {
						var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
						codeChallenge = Base64Url.Encode(challengeBytes);
					}
					context.ProtocolMessage.Parameters.Add("code_challenge", codeChallenge);
					context.ProtocolMessage.Parameters.Add("code_challenge_method", "S256");
				}
				return Task.CompletedTask;
			};

			options.Events.OnAuthorizationCodeReceived = context => {
				if (context.TokenEndpointRequest?.GrantType == OpenIdConnectGrantTypes.AuthorizationCode) {
					if (context.Properties.Items.TryGetValue("code_verifier", out var codeVerifier)) {
						context.TokenEndpointRequest.Parameters.Add("code_verifier", codeVerifier);
					}
				}
				return Task.CompletedTask;
			};

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
			Seed seed = app.ApplicationServices.GetService<Seed>();
			seed.Run();
			Logger.Info(this, "Data seeded");
		}
	}
}