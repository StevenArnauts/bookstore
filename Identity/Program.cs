using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Utilities;
using Utilities.Mvc;

namespace Bookstore.Identity {

	public class Program {

		public static void Main(string[] args) {
			Console.Title = "Identity";
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) {
			// IWebHostBuilder builder = new DefaultWebHostBuilderFactory().Build<Startup>(args);
			IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
			X509Certificate2 cert = Certificate.FromFile("local.pfx", "pencil");
			builder.UseKestrel(options => {
				options.ConfigureHttpsDefaults(httpsOptions => {
					httpsOptions.ServerCertificate = cert;
				});
			});
			return builder.Build();
		}

	}

}
