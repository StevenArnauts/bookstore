using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Utilities.Web {

	public class WebConfiguration {

		public string Host { get; set; }

	}

	public static class ServiceCollectionExtensions {

		public static WebConfiguration UseHostConfiguration(this IServiceCollection services, IConfiguration configuration, Type implementation = null) {
			Type type = implementation ?? typeof(WebConfiguration);
			WebConfiguration config = Activator.CreateInstance(type) as WebConfiguration;
			configuration.GetSection("Web").Bind(config);
			services.AddSingleton(type, config);
			return config;
		}

		public static TConfiguration UseHostConfiguration<TConfiguration>(this IServiceCollection services, IConfiguration configuration) where TConfiguration : WebConfiguration 		{
			return (TConfiguration)UseHostConfiguration(services, configuration, typeof(TConfiguration));
		}

	}

}
