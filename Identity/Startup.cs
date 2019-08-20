using Bookstore.Identity.Entities;
using Bookstore.Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Utilities;
using Utilities.Entities;
using Utilities.Logging;
using Utilities.Web;

namespace Bookstore.Identity {

	public class Startup {

		private const string cors = "local-clients";

		public Startup(IConfiguration configuration) {
			this.Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {

			services.Configure<CookiePolicyOptions>(options => {
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddCors(options => {
				options.AddPolicy(cors, policy => {
					policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
				});
			});

			services
				.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
			;

			WebConfiguration host = services.UseHostConfiguration(this.Configuration);

			services
				.AddAuthentication(options => options.DefaultAuthenticateScheme = Authentication.Scheme)
				.AddCookie(Authentication.Scheme, options => options.LoginPath = "/Account/Login")
			;

			IIdentityServerBuilder builder = services.AddIdentityServer();
			builder.AddInMemoryClients(new Clients(host).Get());
			builder.AddInMemoryIdentityResources(new IdentityResources().Get());
			builder.AddInMemoryApiResources(new ApiResources().Get());
			builder.AddProfileService<ProfileService>();
			builder.AddSigningCredential(Certificate.FromFile("local.pfx", "pencil"));
			
			services.AddEntities(typeof(User).Assembly);

			string connectionString = Configuration.GetConnectionString("identity");
			services.AddDbContext<IdentityContext>(options => { options.UseNpgsql(connectionString); });
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			loggerFactory.AddNLog();
			app.UseDeveloperExceptionPage();
			app.UseAuthentication();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseCors(cors);
			app.UseIdentityServer();
			app.UseMvc(routes => {
				routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
			});
			app.UseSeed<Seed>();
			Logger.Info(this, "Data seeded");
		}

	}

}
