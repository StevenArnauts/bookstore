using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Utilities.Logging;

namespace Utilities.Mvc {

	public class DefaultWebHostBuilderFactory {

		public IWebHostBuilder Build<TStartup>(string[] args, params string[] configurationFiles) where TStartup : class {
			IWebHostBuilder host = WebHost.CreateDefaultBuilder(args);
			host.UseContentRoot(Directory.GetCurrentDirectory());
			host.ConfigureAppConfiguration((builderContext, config) => {
				var env = builderContext.HostingEnvironment;
				Logger.Info(this, "Using environment " + env.EnvironmentName);
				LoadSettings(config, "appsettings.json");
				LoadSettings(config, "appsettings." + env.EnvironmentName + ".json", false);
				foreach(string file in configurationFiles) {
					LoadSettings(config, file, true);
				}
				var userSettingsFile = "appsettings.json.user";
				if (File.Exists(userSettingsFile)) {
					Logger.Warn(this, "Using user settings file " + userSettingsFile);
					LoadSettings(config, userSettingsFile, false);
				}
			});
			host.UseStartup<TStartup>();
			host.ConfigureLogging((hostingContext, logging) => { logging.ClearProviders(); });
			return host;
		}

		private void LoadSettings(IConfigurationBuilder config, string file, bool required = false) {
			string path = Path.GetFullPath(file);
			Logger.Info(this, "Using settings file " + path);
			if (File.Exists(path)) {
				config.AddJsonFile(path, false, true);
			} else {
				if (required) throw new Exception("Required configuration file " + path + " not found");
				Logger.Warn(this, "Settings file " + path + " does not exist");
			}
		}

	}

}