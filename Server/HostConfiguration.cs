using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common {

	public class HostConfiguration {

		public string Name { get; set; }
		public string Host { get; set; }

	}

	public static class ServiceCollectionExtensions {

		public static HostConfiguration UseHostConfiguration(this IServiceCollection services, IConfiguration configuration) {
			HostConfiguration host = new HostConfiguration();
			configuration.GetSection("host").Bind(host);
			services.AddSingleton(host);
			return host;
		}

	}

}
